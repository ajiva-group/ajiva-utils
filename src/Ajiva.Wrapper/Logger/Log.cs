using System;

namespace Ajiva.Wrapper.Logger
{
    public static class LogHelper
    {
        public static bool UseConsoleCursorPos { get; set; } = TestConsoleCursorSetSupport();

        public static bool TestConsoleCursorSetSupport()
        {
            try
            {
                lock (WriteLock)
                {
                    var (left, top) = Console.GetCursorPosition();

                    Console.SetCursorPosition(left, top);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static void Log(object obj)
        {
            lock (WriteLock)
                Console.WriteLine(obj.ToString());
        }

        public static void LogNoBreak(object obj)
        {
            lock (WriteLock)
                Console.Write(obj.ToString());
        }

        public static void WriteLine(object obj) => Log(obj);

        public static void Write(string msg, int position)
        {
            lock (WriteLock)
            {
                if (UseConsoleCursorPos)
                {
                    Console.Write(msg);
                    return;
                }

                var (left, top) = Console.GetCursorPosition();

                if (top != position) Console.SetCursorPosition(0, position);

                Console.Write(msg);

                if (top != position) Console.SetCursorPosition(left, top);

                //var (leftN, topN) = Console.GetCursorPosition();
            }
        }

        public static void TransactWithColor(Action action, ConsoleColor? foreground, ConsoleColor? background)
        {
            lock (WriteLock)
            {
                var cbF = Console.ForegroundColor;
                var cbB = Console.BackgroundColor;

                if (foreground.HasValue)
                    Console.ForegroundColor = foreground.Value;
                if (background.HasValue)
                    Console.BackgroundColor = background.Value;

                action.Invoke();

                if (Console.ForegroundColor != cbF)
                    Console.ForegroundColor = cbF;
                if (Console.BackgroundColor != cbB)
                    Console.BackgroundColor = cbB;
            }
        }

        internal static readonly object WriteLock = new();

        public static string GetInput(string message)
        {
            Console.WriteLine(message);
            return Console.ReadLine() ?? "";
        }

        private static ConsoleBlock? yesNo;

        public static bool YesNow(string s)
        {
            lock (WriteLock)
            {
                int left = default;
                int top = default;
                if (UseConsoleCursorPos)
                    (left, top) = Console.GetCursorPosition();
                var msg = s + " [y/n]";
                yesNo ??= new(1);
                while (true)
                {
                    yesNo.WriteAtNoBreak(msg, 0);
                    if (UseConsoleCursorPos)
                        Console.SetCursorPosition(msg.Length + 1, yesNo.Top);
                    var key = Console.ReadKey();
                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (key.Key)
                    {
                        case ConsoleKey.Y:
                            if (UseConsoleCursorPos)
                                Console.SetCursorPosition(left, top + 1);
                            return true;
                        case ConsoleKey.N:
                            if (UseConsoleCursorPos)
                                Console.SetCursorPosition(left, top + 1);
                            return false;
                        default:
                            msg = s + " [Y/N]! ";
                            break;
                    }
                }
            }
        }
    }
}
