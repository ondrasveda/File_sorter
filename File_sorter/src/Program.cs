using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace FileOrganizer
{
    class Program
    {
        private static BlockingCollection<string> _workQueue = new BlockingCollection<string>();

        static async Task Main(string[] args)
        {
            Console.Title = "File Organizer (Producer-Consumer)";
            
            string configPath = "C:\\Programovani\\C#\\File_sorter\\File_sorter\\config\\settings.txt"; 
            if (!File.Exists(configPath))
            {
                Console.WriteLine($"Chyba: Nenalezen konfigurační soubor na cestě: {Path.GetFullPath(configPath)}");
                Console.WriteLine("Prosím vytvořte ho podle návodu.");
                return;
            }
            
            AppConfig config = AppConfig.Load(configPath);
            Logger.Initialize(config.TargetPath); 
            Logger.Log($"Konfigurace načtena. Zdroj: {config.SourcePath}, Cíl: {config.TargetPath}, Vláken: {config.ThreadCount}");


            Task producerTask = Task.Run(() => ProduceFiles(config.SourcePath));

            List<Task> consumerTasks = new List<Task>();
            for (int i = 0; i < config.ThreadCount; i++)
            {
                int workerId = i + 1;
                consumerTasks.Add(Task.Run(() => ConsumeFiles(workerId, config.TargetPath)));
            }

            await producerTask;
            
            await Task.WhenAll(consumerTasks);

            Logger.Log("------------------------------------------------------------------");
            Logger.Log("Všechny úkoly dokončeny. Stiskněte ENTER pro ukončení.");
            Console.ReadLine();
        }

        static void ProduceFiles(string sourcePath)
        {
            try 
            {
                Logger.Log("PRODUCENT: Zahajuji skenování...");
                string[] files = Directory.GetFiles(sourcePath);
                
                foreach (var file in files.OrderBy(f => f))
                {
                    _workQueue.Add(file);
                }
                Logger.Log($"PRODUCENT: Nalezeno a přidáno {_workQueue.Count} souborů do fronty.");
            }
            catch (DirectoryNotFoundException)
            {
                Logger.Log($"CHYBA PRODUCENTA: Zdrojová složka '{sourcePath}' nebyla nalezena.");
            }
            catch (Exception ex)
            {
                Logger.Log($"CHYBA PRODUCENTA: {ex.Message}");
            }
            finally 
            {
                _workQueue.CompleteAdding(); 
            }
        }

        static void ConsumeFiles(int workerId, string targetBaseDir)
        {
            foreach (var filePath in _workQueue.GetConsumingEnumerable())
            {
                try
                {
                    string fileName = Path.GetFileName(filePath);
                    string extension = Path.GetExtension(filePath).TrimStart('.').ToUpper();
                    
                    if (string.IsNullOrEmpty(extension)) extension = "NEPRIRAZENO";

                    string targetFolder = Path.Combine(targetBaseDir, extension);
                    Directory.CreateDirectory(targetFolder);

                    string destPath = Path.Combine(targetFolder, fileName);
                    
                    if (File.Exists(destPath))
                    {
                         Logger.Log($"[Worker {workerId}] VAROVÁNÍ: {fileName} již existuje v cíli, přeskakuji.");
                    }
                    else
                    {
                        File.Move(filePath, destPath);
                        Logger.Log($"[Worker {workerId}] PŘESUNUTO: {fileName} -> {extension}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"[Worker {workerId}] CHYBA ZPRACOVÁNÍ SOUBORU '{filePath}': {ex.Message}");
                }
            }
            Logger.Log($"[Worker {workerId}] Dokončil práci a končí.");
        }
    }
}