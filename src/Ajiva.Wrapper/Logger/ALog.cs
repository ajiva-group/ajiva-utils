using System;
using System.Diagnostics;
using System.IO;

namespace Ajiva.Wrapper.Logger
{
    public static class ALog
    {
        public static string ALogLevelToString(ALogLevel level)
        {
            return level switch
            {
                ALogLevel.Trace => "TRA",
                ALogLevel.Debug => "DBG",
                ALogLevel.Info => "INF",
                ALogLevel.Warning => "WRN",
                ALogLevel.Error => "ERR",
                ALogLevel.Fatal => "FAT",
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
        }

        public static void LogTo(string file, object value) => LogFile(file, value);
        public static void Log(ALogLevel level, object value) => LogAndFormats(level, value, new StackFrame(1, true));
        public static void Trace(object value) => LogAndFormats(ALogLevel.Trace, value, new StackFrame(1, true));
        public static void Debug(object value) => LogAndFormats(ALogLevel.Debug, value, new StackFrame(1, true));
        public static void Info(object value) => LogAndFormats(ALogLevel.Info, value, new StackFrame(1, true));
        public static void Warn(object value) => LogAndFormats(ALogLevel.Warning, value, new StackFrame(1, true));
        public static void Error(object value) => LogAndFormats(ALogLevel.Error, value, new StackFrame(1, true));
        public static void Error(Exception e) => LogAndFormats(ALogLevel.Error, e, new StackFrame(1, true));

        private static void LogAndFormats(ALogLevel level, object value, StackFrame stackFrame)
        {
            var mainColor = level switch
            {
                ALogLevel.Trace => ConsoleColor.DarkGray,
                ALogLevel.Debug => ConsoleColor.Gray,
                ALogLevel.Info => ConsoleColor.White,
                ALogLevel.Warning => ConsoleColor.Yellow,
                ALogLevel.Error => ConsoleColor.Red,
                ALogLevel.Fatal => ConsoleColor.DarkRed,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
            TransactWithColor(Console.Write,
                new LogSegment("[", ConsoleColor.Gray),
                new LogSegment(DateTime.Now.ToString("hh:mm:ss"), ConsoleColor.DarkCyan),
                new LogSegment("]", ConsoleColor.Gray),
                new LogSegment($" {ALogLevelToString(level)} ", mainColor),
                new LogSegment($"{stackFrame.GetFileName()}:{stackFrame.GetFileLineNumber()}", ConsoleColor.Gray),
                new LogSegment($" ({stackFrame.GetMethod()?.Name}) ", ConsoleColor.Blue),
                new LogSegment(value + "\n", mainColor)
            );

            //return $"[{DateTime.Now:hh:mm:ss}] {level} ({stackFrame.GetMethod()}) {stackFrame.GetFileName()}:{stackFrame.GetFileLineNumber()}{Environment.NewLine}{value}";
        }

        private static void TransactWithColor(Action<string> logTo, params LogSegment[] logSegments)
        {
            lock (WriteLock)
            {
                var cbF = Console.ForegroundColor;
                var cbB = Console.BackgroundColor;
                foreach (var logSegment in logSegments)
                {
                    if (logSegment.Foreground.HasValue)
                        Console.ForegroundColor = logSegment.Foreground.Value;
                    if (logSegment.Background.HasValue)
                        Console.BackgroundColor = logSegment.Background.Value;

                    logTo.Invoke(logSegment.Value);
                }
                if (Console.ForegroundColor != cbF)
                    Console.ForegroundColor = cbF;
                if (Console.BackgroundColor != cbB)
                    Console.BackgroundColor = cbB;
            }
        }

        private static void LogFile(string file, object value)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
                var fs = File.AppendText(file);
                fs.WriteLine(value);
                fs.Close();
            }
            catch (Exception e)
            {
                Error(e);
            }
        }

        private static object WriteLock { get; } = new object();

        public static void WriteLine(object value)
        {
            lock (WriteLock)
                Console.WriteLine(value);
        }
    }
    public readonly struct LogSegment
    {
        public LogSegment(string value, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            Value = value;
            Foreground = foreground;
            Background = background;
        }

        public static implicit operator LogSegment(string value) => new LogSegment(value);
        public string Value { get; }
        public ConsoleColor? Foreground { get; }
        public ConsoleColor? Background { get; }
    }
}
