using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace File_sorter
{

    public class Organizer
    {
        private readonly AppConfig _config;
        private readonly BlockingCollection<string> _workQueue = new BlockingCollection<string>();

        public Organizer(AppConfig config)
        {
            _config = config;
        }

        public async Task RunAsync()
        {
            Logger.Initialize(_config.TargetPath);
            Logger.Log("Startuji Organizer Engine...");

            // 1. Spuštění Producenta (hledá soubory)
            Task producer = Task.Run(() => Produce());

            // 2. Spuštění Konzumentů (třídí soubory)
            var consumers = new List<Task>();
            for (int i = 0; i < _config.ThreadCount; i++)
            {
                int id = i + 1;
                consumers.Add(Task.Run(() => Consume(id)));
            }

            // 3. Čekání na dokončení
            await producer;
            await Task.WhenAll(consumers);
            
            Logger.Log("Vše hotovo. Engine končí.");
        }

        private void Produce()
        {
            try
            {
                if (!Directory.Exists(_config.SourcePath))
                {
                    Logger.Log($"CHYBA: Zdrojová složka neexistuje: {_config.SourcePath}");
                    return;
                }

                foreach (var file in Directory.GetFiles(_config.SourcePath))
                {
                    _workQueue.Add(file);
                }
            }
            catch (Exception ex)
            {
                 Logger.Log($"CHYBA PRODUCENTA: {ex.Message}");
            }
            finally
            {
                _workQueue.CompleteAdding(); // Signál, že je hotovo
            }
        }

        private void Consume(int workerId)
        {
            foreach (var filePath in _workQueue.GetConsumingEnumerable())
            {
                try
                {
                    string ext = Path.GetExtension(filePath).TrimStart('.').ToUpper();
                    if (string.IsNullOrEmpty(ext)) ext = "NEZNAMY";

                    // Cílová složka podle přípony
                    string targetFolder = Path.Combine(_config.TargetPath, ext);
                    Directory.CreateDirectory(targetFolder);

                    string fileName = Path.GetFileName(filePath);
                    string destPath = Path.Combine(targetFolder, fileName);

                    if (!File.Exists(destPath))
                    {
                        File.Move(filePath, destPath);
                        Logger.Log($"Worker {workerId}: PŘESUNUTO {fileName} -> {ext}");
                    }
                    else
                    {
                        Logger.Log($"Worker {workerId}: {fileName} již existuje, přeskakuji.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log($"Worker {workerId} CHYBA: {ex.Message}");
                }
            }
        }
    }
}