using System;

namespace Task2_1.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                var str = Console.ReadLine();
                Console.WriteLine(string.IsNullOrEmpty(str) ? "Error: empty string" : $"First: {str[0]}");
            }
        }
    }
}
