using System;

namespace Ajiva.Wrapper.Logger
{
    public class ConsoleBlock
    {
        public const int BufferTolerance = 10;

        public readonly int Count;
        public readonly int Top;

        public ConsoleBlock(int count)
        {
            lock (LogHelper.WriteLock)
            {
                Count = count;
                Top = Console.CursorTop;
                Console.SetCursorPosition(0, Top + count);
            }
        }

        public void WriteAt(string msg, int position)
        {
            if (Console.BufferWidth < msg.Length + BufferTolerance)
            {
                Console.BufferWidth = msg.Length + BufferTolerance;
            }
            if (position > Count) throw new ArgumentOutOfRangeException(nameof(position), position, "expected les than count: " + Count);
            LogHelper.Write("".FillUp(Console.BufferWidth), Top + position);
            LogHelper.Write(msg + "\n", Top + position);
        }

        public void RowWriteAt(string msg, int position)
        {
            if (position > Count) throw new ArgumentOutOfRangeException(nameof(position), position, "expected les than count: " + Count);
            LogHelper.Write(msg, Top + position);
        }

        public void WriteAtNoBreak(string msg, int position)
        {
            if (position > Count) throw new ArgumentOutOfRangeException(nameof(position), position, "expected les than count: " + Count);
            LogHelper.Write("".FillUp(Console.BufferWidth), Top + position);
            LogHelper.Write(msg, Top + position);
        }
    }
}
