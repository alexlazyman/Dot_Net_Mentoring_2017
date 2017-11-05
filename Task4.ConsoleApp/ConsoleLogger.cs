using System;

namespace Task4.ConsoleApp
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string str, params object[] args)
        {
            Console.WriteLine(str, args);
        }
    }
}