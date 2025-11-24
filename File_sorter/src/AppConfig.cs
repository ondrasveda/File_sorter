using System;
using System.IO;

namespace FileOrganizer
{
    public class AppConfig
    {
        public string SourcePath { get; private set; }
        public string TargetPath { get; private set; }
        public int ThreadCount { get; private set; }

        public static AppConfig Load(string filePath)
        {
            var config = new AppConfig();
            try
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;
                    
                    var parts = line.Split('=');
                    if (parts.Length != 2) continue;

                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    if (key == "SourcePath") config.SourcePath = value;
                    else if (key == "TargetPath") config.TargetPath = value;
                    else if (key == "ThreadCount") config.ThreadCount = int.Parse(value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL: Chyba při načítání konfigurace: {ex.Message}");
                throw;
            }
            return config;
        }
    }
}