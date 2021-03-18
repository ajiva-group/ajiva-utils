using System;

namespace Ajiva.Wrapper.Logger
{
    public static class LogHelper
    {
        public static void Log(object obj)
        {
            lock (WriteLock)
                System.Console.WriteLine(obj.ToString());
        }

        public static void LogNoBreak(object obj)
        {
            lock (WriteLock)
                System.Console.Write(obj.ToString());
        }

        public static void WriteLine(object obj) => Log(obj);

        public static void Write(string msg, int position)
        {
            lock (WriteLock)
            {
                var (left, top) = System.Console.GetCursorPosition();

                if (top != position) System.Console.SetCursorPosition(0, position);

                System.Console.Write(msg);

                if (top != position) System.Console.SetCursorPosition(left, top);
            }
            System.Console.Out.WriteLine();
        }

        public static void TransactWithColor(Action action, ConsoleColor? foreground, ConsoleColor? background)
        {
            lock (WriteLock)
            {
                var cbF = System.Console.ForegroundColor;
                var cbB = System.Console.BackgroundColor;

                if (foreground.HasValue)
                    System.Console.ForegroundColor = foreground.Value;
                if (background.HasValue)
                    System.Console.BackgroundColor = background.Value;

                action.Invoke();

                if (System.Console.ForegroundColor != cbF)
                    System.Console.ForegroundColor = cbF;
                if (System.Console.BackgroundColor != cbB)
                    System.Console.BackgroundColor = cbB;
            }
        }

        internal static readonly object WriteLock = new();

        public static string GetInput(string message)
        {
            System.Console.WriteLine(message);
            return System.Console.ReadLine() ?? "";
        }

        private static ConsoleBlock? yesNo;

        public static bool YesNow(string s)
        {
            lock (WriteLock)
            {
                var (left, top) = System.Console.GetCursorPosition();
                var msg = s + " [y/n]";
                yesNo ??= new(1);
                while (true)
                {
                    yesNo.WriteAtNoBreak(msg, 0);
                    System.Console.SetCursorPosition(msg.Length + 1, yesNo.Top);
                    var key = System.Console.ReadKey();
                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (key.Key)
                    {
                        case ConsoleKey.Y:
                            System.Console.SetCursorPosition(left, top + 1);
                            return true;
                        case ConsoleKey.N:
                            System.Console.SetCursorPosition(left, top + 1);
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
