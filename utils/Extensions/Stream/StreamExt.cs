using System.Buffers;

namespace Ajiva.Utils.Extensions.Stream
{
    public static class StreamExt
    {
        public static void BufferCopy(this System.IO.Stream stream, System.IO.Stream destination, long length, int bufferSize)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                int read;
                while ((read = stream.Read(buffer, 0, length < bufferSize ? (int)length : bufferSize)) != 0)
                {
                    destination.Write(buffer, 0, read);
                    length -= read;
                    if (length == 0)
                    {
                        break;
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}
