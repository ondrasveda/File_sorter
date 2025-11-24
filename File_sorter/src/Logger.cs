using System;
using System.IO;
using System.Text;

namespace FileOrganizer
{
    public static class Logger
    {
        private static readonly object _fileLock = new object();
        private static string _logPath = "organizer_log.txt";

        public static void Initialize(string baseDir)
        {
            _logPath = Path.Combine(baseDir, "organizer_log.txt");
            File.WriteAllText(_logPath, $"--- Logování zahájeno: {DateTime.Now} ---{Environment.NewLine}");
        }

        public static void Log(string message)
        {
            string logMessage = $"{DateTime.Now.ToString("HH:mm:ss.fff")} [TID: {Environment.CurrentManagedThreadId}] {message}";
            
            Console.WriteLine(logMessage);

            lock (_fileLock)
            {
                File.AppendAllText(_logPath, logMessage + Environment.NewLine, Encoding.UTF8);
            }
        }
    }
}