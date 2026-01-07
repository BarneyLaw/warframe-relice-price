using System;
using System.IO;

namespace warframe_relice_price.Utils
{
    public static class Logger
    {
        private static readonly string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "overlay.log");

        public static void Log(string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] - {message}{Environment.NewLine}";
                File.AppendAllText(LogFile, logEntry);
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}
