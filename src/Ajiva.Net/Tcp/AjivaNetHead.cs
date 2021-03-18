using System.Runtime.InteropServices;

namespace Ajiva.Net.Tcp
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct AjivaNetHead
    {
        public int DataLength;
        public int Version;
        public int Index;
        public int ClientId;
        public int Type;

        public T GetType<T>() => (T)(object)Type;
    }
}
