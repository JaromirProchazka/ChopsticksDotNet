using System;
using System.Diagnostics;

namespace ChopsticksDotNet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var conf = new DefaultConfigBuilder("acala").GetManager();
            var process = new ChopsticksApi(conf);
            process.Run();

            Thread.Sleep(1000_000);
        }
    }
}