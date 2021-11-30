using System;

namespace Ajiva.Wrapper.Logger;

public interface ILogger
{
    void Write(string value);
    void WriteLine(string value);
    void Log(ALogLevel level, object value);
    void Trace(object value);
    void Debug(object value);
    void Info(object value);
    void Warn(object value);
    void Error(object value);
    void Error(Exception e);
}