using System;

namespace Zenko
{
    public static class Logger
    {
        static Action<object> logAction = (object message) => Console.WriteLine(message);
        public static void Log(object message)
        {
            // Console.ResetColor();
            logAction(message);
            // Console.ResetColor();
        }
        public static void LogWarning(object message)
        {
            // Console.ResetColor();
            // Console.BackgroundColor = ConsoleColor.Yellow;
            //             Console.ForegroundColor = ConsoleColor.Black;
            logAction(message);
            // Console.ResetColor();
        }
        public static void LogError(object message)
        {
            // Console.ResetColor();
            // Console.BackgroundColor = ConsoleColor.Red;
            logAction(message);
            // Console.ResetColor();
        }

        public static void SetLogAction(Action<object> action)
        {
            logAction = action;
        }
    }
}