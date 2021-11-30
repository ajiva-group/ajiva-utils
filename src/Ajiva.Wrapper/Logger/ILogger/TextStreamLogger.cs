using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Ajiva.Wrapper.Logger;

public class TextStreamLogger : ILogger
{
    private readonly bool fileInfo;
    private readonly TextWriter output;

    public void Write(string value)
    {
        lock (WriteLock)
            output.Write(value);
    }

    public void WriteLine(string value)
    {
        lock (WriteLock)
            output.WriteLine(value);
    }

    public TextStreamLogger(TextWriter output, bool fileInfo)
    {
        this.fileInfo = fileInfo;
        this.output = output;
    }

    public void Log(ALogLevel level, object value) => LogAndFormats(level, value, new StackFrame(1, fileInfo));
    public void Trace(object value) => LogAndFormats(ALogLevel.Trace, value, new StackFrame(1, fileInfo));
    public void Debug(object value) => LogAndFormats(ALogLevel.Debug, value, new StackFrame(1, fileInfo));
    public void Info(object value) => LogAndFormats(ALogLevel.Info, value, new StackFrame(1, fileInfo));
    public void Warn(object value) => LogAndFormats(ALogLevel.Warning, value, new StackFrame(1, fileInfo));
    public void Error(object value) => LogAndFormats(ALogLevel.Error, value, new StackFrame(1, fileInfo));
    public void Error(Exception e) => LogAndFormats(ALogLevel.Error, e, new StackFrame(1, fileInfo));
    private void LogAndFormats(ALogLevel level, object value, StackFrame stackFrame) => WriteLine($"{DateTime.Now.ToString("hh:mm:ss", CultureInfo.InvariantCulture)}, {level}, {stackFrame.GetMethod()},{(fileInfo ? $"{stackFrame.GetFileName()}:{stackFrame.GetFileLineNumber()}, " : "")}{value}");

    private object WriteLock { get; } = new object();
}