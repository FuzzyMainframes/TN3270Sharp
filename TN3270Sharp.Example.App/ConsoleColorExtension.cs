using System;
using System.Collections.Generic;
using System.Text;

namespace TN3270Sharp.Example.App
{
    public static class ConsoleColorExtension
    {
        public static void WriteLine(this ConsoleColor foregroundColor, string value)
        {
            var originalForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(value);

            Console.ForegroundColor = originalForegroundColor;
        }
        
        public static void WriteLine(this ConsoleColor foregroundColor, string value, params object?[] paramArray)
        {
            var originalForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(String.Format(value, paramArray));

            Console.ForegroundColor = originalForegroundColor;
        }
    }
}
