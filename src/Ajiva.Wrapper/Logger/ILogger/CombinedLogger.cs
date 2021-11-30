using System;
using System.Collections.Generic;

namespace Ajiva.Wrapper.Logger;

public class CombinedLogger : ILogger
{
    public CombinedLogger(List<ILogger> loggers)
    {
        Loggers = loggers;
    }

    private List<ILogger> Loggers { get; }

    /// <inheritdoc />
    public void Write(string value)
    {
        foreach (var loggerImplementation in Loggers)
            loggerImplementation.Write(value);
    }

    /// <inheritdoc />
    public void WriteLine(string value)
    {
        foreach (var loggerImplementation in Loggers)
            loggerImplementation.WriteLine(value);
    }

    /// <inheritdoc />
    public void Log(ALogLevel level, object value)
    {
        foreach (var loggerImplementation in Loggers)
            loggerImplementation.Log(level, value);
    }

    /// <inheritdoc />
    public void Trace(object value)
    {
        foreach (var loggerImplementation in Loggers)
            loggerImplementation.Trace(value);
    }

    /// <inheritdoc />
    public void Debug(object value)
    {
        foreach (var loggerImplementation in Loggers)
            loggerImplementation.Debug(value);
    }

    /// <inheritdoc />
    public void Info(object value)
    {
        foreach (var loggerImplementation in Loggers)
            loggerImplementation.Info(value);
    }

    /// <inheritdoc />
    public void Warn(object value)
    {
        foreach (var loggerImplementation in Loggers)
            loggerImplementation.Warn(value);
    }

    /// <inheritdoc />
    public void Error(object value)
    {
        foreach (var loggerImplementation in Loggers)
            loggerImplementation.Error(value);
    }

    /// <inheritdoc />
    public void Error(Exception e)
    {
        foreach (var loggerImplementation in Loggers)
            loggerImplementation.Error(e);
    }
}
