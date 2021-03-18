using System;
using System.Linq;

namespace Ajiva.Wrapper.Logger
{
    public class ConsoleMenu
    {
        public static void ShowMenu(string title, params ConsoleMenuItem[] items)
        {
            LogHelper.WriteLine(title);
            var format = new string(Enumerable.Repeat('0', items.Length / 10 + 1).ToArray());
            var i = 0;
            foreach (var item in items)
            {
                LogHelper.WriteLine($"[{i.ToString(format)}]: {item.Name}");
                i++;
            }
            int num;
            do
            {
                LogHelper.WriteLine("Chose Action [0...{i}]");
            } while (int.TryParse(System.Console.ReadLine(), out num) && num < 0 && num > i);

            items[num].Action?.Invoke();
        }
    }

    public record ConsoleMenuItem (string Name, Action? Action);
}
