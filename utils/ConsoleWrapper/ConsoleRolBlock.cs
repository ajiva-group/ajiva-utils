using System;

namespace utils.ConsoleExt
{
    public sealed class ConsoleRolBlock
    {
        private readonly int count;
        private readonly bool highlight;
        private const int HeadSize = 1;
        private readonly ConsoleBlock block;
        private int index;

        private readonly string head;

        public ConsoleRolBlock(int count, string? name, bool highlight = true)
        {
            this.count = count;
            this.highlight = highlight;
            block = new(count + HeadSize);
            head = (string.IsNullOrEmpty(name) ? GetHashCode().ToString("X8") : name) + $" block - {count} / ";
        }

        private readonly object mutex = new();
        private string? last;

        public void WriteNext(string msg)
        {
            if (!highlight)
            {
                WriteCore(msg);
                return;
            }

            lock (mutex)
            {
                LogHelper.TransactWithColor(() => block.WriteAt(head + index, 0),
                    ConsoleColor.Red, null);

                if (last is not null)
                    WriteCore(last);

                ++index;

                LogHelper.TransactWithColor(() => WriteCore(msg),
                    ConsoleColor.Red, null);

                last = msg;
            }
        }

        private void WriteCore(string data)
        {
            lock (LogHelper.WriteLock)
            {
                // ReSharper disable once UseFormatSpecifierInInterpolation
                block.WriteAt($"[{index.ToString("X8")}] {data}", index % count + HeadSize);
            }
        }
    }
}
