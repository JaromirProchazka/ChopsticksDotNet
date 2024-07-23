using System;

namespace ChopsticksDotNet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var conf = new DefaultConfigBuilder("moonbeam").GetManager();
            var process = new ChopsticksApi(conf);
            process.Run();
        }
    }
}