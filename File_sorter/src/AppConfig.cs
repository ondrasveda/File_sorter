using System;
using System.IO;

namespace File_sorter
{
    public class AppConfig
    {
        public string SourcePath { get; private set; }
        public string TargetPath { get; private set; }
        public int ThreadCount { get; private set; }

        public static AppConfig Load(string fileName)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory; 
            
            string projectRoot = Path.Combine(baseDirectory, "..", "..", "..");

            string fullConfigPath = Path.Combine(projectRoot, "config", fileName); 

            if (!File.Exists(fullConfigPath))
                throw new FileNotFoundException($"Konfigurační soubor nenalezen: {fullConfigPath}");

            var config = new AppConfig();
            foreach (var line in File.ReadAllLines(fullConfigPath))
            {
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split('=');
                if (parts.Length != 2) continue;

                var key = parts[0].Trim();
                var val = parts[1].Trim();

                if (key == "ThreadCount") config.ThreadCount = int.Parse(val);
                else if (key == "SourcePath") config.SourcePath = ResolvePath(val, projectRoot); 
                else if (key == "TargetPath") config.TargetPath = ResolvePath(val, projectRoot);
            }
            return config;
        }

        private static string ResolvePath(string path, string baseDir)
        {
            if (path.StartsWith("."))
            {
                return Path.GetFullPath(Path.Combine(baseDir, path)); 
            }
            return path;
        }
    }
}