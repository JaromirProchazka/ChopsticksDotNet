using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using OutsideToolStarter;

namespace ChopsticksDotNet
{
    /// <summary>
    /// Given the configuration, it Starts and manages the Chopsticks. The Chopsticks are stopped on calling the .Dispose() method, or on destruction.
    /// </summary>
    /// <param name="Config">Provides a config to chopsticks. Use @IChopsticksConfigBuilder to initialize.</param>
    public record class ChopsticksApi(IChopsticksConfigManager Config) : ConsoleToolRunner
    {
        public string ChopsticksVersion = "latest"; // on WSL works "0.12.0", otherwise use "latest"

        /// <summary>
        /// Delegate for a thread, that starts Chopsticks.
        /// </summary>
        public override void ProcessStarter()
        {
            string command = "wsl " + $"npx @acala-network/chopsticks@{ChopsticksVersion} " + Config.Arguments;
            Console.WriteLine(command);
            Process.StandardInput.WriteLine(command);

            //string result = process.StandardOutput.ReadToEnd();
            //Console.WriteLine("Result: " + result);
        }
    }
}
