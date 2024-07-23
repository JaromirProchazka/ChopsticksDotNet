using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideToolStarter
{
    /// <summary>
    /// An abstract class defining a manager of running outside tools. It is mostly used for processes running indefinitely until terminated. Override the .StartTool() method to define what method to start. The Process instance is also provided for the method.
    /// <code>
    /// startProcess(processStartInfo);
    /// process.StandardInput.WriteLine($"npx @acala-network/chopsticks@latest");
    /// </code>
    /// </example>
    /// </summary>
    public abstract record class ConsoleToolRunner : IDisposable
    {
        /// <summary>
        /// The process instance, that will be started with the .Run() method.
        /// </summary>
        public Process Process;
        /// <summary>
        /// The Process starting Information, with which the Process field uses to start.
        /// </summary>
        public ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = "cmd",
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        private Thread processBackgroundThread;
        /// <summary>
        /// State of the Process running.
        /// </summary>
        private bool processStopped = true;


        /// <summary>
        /// Starts the Chopsticks script using Config field.
        /// </summary>
        public void Run()
        {
            processBackgroundThread = new Thread(startCommand);
            processBackgroundThread.IsBackground = true;   // Make the thread end on program termination

            processBackgroundThread.Start();
        }

        /// <summary>
        /// Ends the process.
        /// </summary>
        public virtual void Dispose()
        {
            if (processStopped) return; // if not running, no closing needed.

            Process.CloseMainWindow(); // try closing
            if (!Process.WaitForExit(3000))
            {
                // Forcefully kill the process if it didn't exit yet
                Process.Kill();
            }

            processStopped = true;
        }

        /// <summary>
        /// The user defined method for starting the process.
        /// </summary>
        public abstract void ProcessStarter();

        /// <summary>
        /// The Tool starter. Can be Used as a delegate for Thread.
        /// </summary>
        private void startCommand()
        {
            startProcess(processStartInfo); // Start base process
            ProcessStarter(); // put in user commands
            WaitForExit();
        }

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
            Process = Process.Start(processStartInfo);
            if (Process == null) throw new Exception("Console process couldn't be started!");
            processStopped = false;
        }

        /// <summary>
        /// Waits for the Tools running finish. Either for the natural finish or for the Dispose.
        /// </summary>
        private void WaitForExit()
        {
            Process.WaitForExit();
            processStopped = true; // Change process running state, for .Dispose() method
        }

        ~ConsoleToolRunner()
        {
            Dispose();
        }
    }
}
