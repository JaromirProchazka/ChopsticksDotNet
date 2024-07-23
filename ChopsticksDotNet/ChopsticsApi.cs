using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;

namespace ChopsticksDotNet
{
    /// <summary>
    /// Given the configuration, it Starts and manages the Chopsticks. The Chopsticks are stopped on calling the .Dispose() method, or on destruction.
    /// </summary>
    /// <param name="Config">Provides a config to chopstics. Use @IChopsticksConfigBuilder to initialize.</param>
    public record class ChopsticksApi(IChopsticksConfigManager Config) : IDisposable
    {
        public string ChopsticksVersion = "latest"; // on WSL works "0.12.4", otherwise use "latest"
        
        private Process process = new Process();
        private Thread chopsticsBackgroundThread;
        private bool processStopped = true;

        /// <summary>
        /// Starts empty process and changes the internal state. The Command you want to run must be started separately after this method ended.
        /// <example>
        /// Example:
        /// <code>
        /// startProcess(processStartInfo);
        /// process.StandardInput.WriteLine($"npx @acala-network/chopsticks@latest");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="processStartInfo">Config to the started process</param>
        private void startProcess(ProcessStartInfo processStartInfo)
        {
            process = Process.Start(processStartInfo);
            if (process == null) throw new Exception("Console process couldn't be started!");
            processStopped = false;
        }

        /// <summary>
        /// Delegate for a thread, that starts Chopstics
        /// </summary>
        private void StartChopsticks()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd",
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = false
            };

            startProcess(processStartInfo);
            process.StandardInput.WriteLine($"npx @acala-network/chopsticks@{ChopsticksVersion} " + Config.Arguments);

            //string result = process.StandardOutput.ReadToEnd();
            //Console.WriteLine("Result: " + result);

            process.WaitForExit();
            processStopped = true; // Change process running state, for .Dispose() method
        }

        /// <summary>
        /// Starts the Chopsticks script using Config field.
        /// </summary>
        public void Run()
        {
            chopsticsBackgroundThread = new Thread(StartChopsticks);
            chopsticsBackgroundThread.IsBackground = true;   // Make the thread end on program termination

            chopsticsBackgroundThread.Start();
        }

        public virtual void Dispose() 
        {
            if (processStopped) return; // if not running, no closing needed.

            process.CloseMainWindow(); // try closing
            if (!process.WaitForExit(3000))
            {
                // Forcefully kill the process if it didn't exit yet
                process.Kill();
            }

            processStopped = true;
        }

        ~ChopsticksApi() {
            Dispose();
        }
    }
}
