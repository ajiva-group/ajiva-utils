namespace Ajiva.Wrapper.Logger
{
    public static class ConsoleExtensions
    {
        public static string FillUp(this string toPad, int length)
        {
            //toPad += new string(Enumerable.Repeat(' ', length - toPad.Length).ToArray());
            //return toPad;
            while (toPad.Length < length)
            {
                toPad += '\0';
            }
            return toPad;
        }
    }
}
