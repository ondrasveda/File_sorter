using System;
using System.Threading.Tasks;

namespace File_sorter 
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var config = AppConfig.Load("settings.txt");
                
                Console.WriteLine($"Nastavení načteno.");
                Console.WriteLine($"Zdroj: {config.SourcePath}");
                Console.WriteLine($"Cíl:   {config.TargetPath}");

                var organizer = new Organizer(config); 
                await organizer.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KRITICKÁ CHYBA APLIKACE: {ex.Message}");
            }

            Console.WriteLine("Stiskni ENTER pro ukončení.");
            Console.ReadLine();
        }
    }
}