using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BOT_Marvin___Counter_Strike_Discord_Bot
{
    public static class Logger
    {
        public static void InitializeLogging()
        {
            logMessages();
            Log(LogLevel.INFO, "Started Logging...");
        }

        private static Queue<string> msgQueue = new Queue<string>();

        public static void Log(LogLevel lvl, string msg)
        {
           msgQueue.Enqueue("[" + lvl.ToString() + "]: " + msg);
        }

        private static async void logMessages()
        {
            await Task.Run(() => {
                while (true)
                {
                    if (msgQueue.Count > 0)
                        Console.WriteLine(msgQueue.Dequeue());
                }
            });
        }
    }

    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR,
        SEVERE
    }
}
