using System;
using System.Diagnostics;

namespace Ajiva.Wrapper.Logger;

public class ConsoleLogger : ILogger
{
    public void Write(string value)
    {
        lock (WriteLock)
        {
            Console.Write(value);
        }
    }

    public void WriteLine(string value)
    {
        lock (WriteLock)
        {
            Console.WriteLine(value);
        }
    }

    public void Log(ALogLevel level, object value) => LogAndFormats(level, value, new StackFrame(1, true));
    public void Trace(object value) => LogAndFormats(ALogLevel.Trace, value, new StackFrame(1, true));
    public void Debug(object value) => LogAndFormats(ALogLevel.Debug, value, new StackFrame(1, true));
    public void Info(object value) => LogAndFormats(ALogLevel.Info, value, new StackFrame(1, true));
    public void Warn(object value) => LogAndFormats(ALogLevel.Warning, value, new StackFrame(1, true));
    public void Error(object value) => LogAndFormats(ALogLevel.Error, value, new StackFrame(1, true));
    public void Error(Exception e) => LogAndFormats(ALogLevel.Error, e, new StackFrame(1, true));

    private void LogAndFormats(ALogLevel level, object value, StackFrame stackFrame)
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

        lock (WriteLock)
        {
            var cbF = Console.ForegroundColor;
            var cbB = Console.BackgroundColor;

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("[");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(DateTime.Now.ToString("hh:mm:ss"));

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("]");

            Console.ForegroundColor = mainColor;
            Console.Write($" {ALog.ALogLevelToString(level)} ");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{stackFrame.GetFileName()}:{stackFrame.GetFileLineNumber()}");

            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($" ({stackFrame.GetMethod()?.Name}) ");

            Console.ForegroundColor = mainColor;
            Console.WriteLine(value);

            if (Console.ForegroundColor != cbF)
                Console.ForegroundColor = cbF;
            if (Console.BackgroundColor != cbB)
                Console.BackgroundColor = cbB;
        }
    }

    private object WriteLock { get; } = new object();
}