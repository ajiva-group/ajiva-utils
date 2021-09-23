using System;
using System.Diagnostics;

namespace ajiva.Utils
{
    public interface IDisposingLogger : IDisposable
    {
        public bool Disposed { get; }

        [DebuggerStepThrough]
        public void DisposeIn(int delayMs);

        /// <inheritdoc />
        [DebuggerStepThrough]
        abstract void IDisposable.Dispose();
    }
}
