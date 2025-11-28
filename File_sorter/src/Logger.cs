using System;
using System.IO;
using System.Text;

namespace File_sorter 
{
    public static class Logger
    {
        private static readonly object _lock = new object();
        private static string _logPath;

        public static void Initialize(string targetDir)
        {
            _logPath = Path.Combine(targetDir, "process_log.txt"); 
        }

        public static void Log(string message)
        {
            string msg = $"{DateTime.Now:HH:mm:ss} [TID:{System.Threading.Thread.CurrentThread.ManagedThreadId}] {message}";
            Console.WriteLine(msg); 
            
            if (_logPath != null)
            {
                lock (_lock)
                {
                    File.AppendAllText(_logPath, msg + Environment.NewLine, Encoding.UTF8);
                }
            }
        }
    }
}