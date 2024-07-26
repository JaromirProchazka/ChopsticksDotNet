using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChopsticksDotNet
{
    /// <summary>
    /// Provides the Config needed for the Starting of Chopstics.
    /// </summary>
    public interface IChopsticksConfigManager
    {
        string Arguments { get; }
    }

    /// <summary>
    /// Config is sourced from AcalaNetwork/chopsticks/configs default .yaml file.
    /// </summary>
    /// <param name="Chain"></param>
    public record class DefaultConfig(string Chain) : IChopsticksConfigManager
    {
        public string Arguments { get => $"-c {Chain}"; }
    }

    /// <summary>
    /// Config sourced from a local file path to a config file.
    /// </summary>
    /// <param name="ConfigFile">Path to a config file.</param>
    public record class FileConfig(string ConfigFile) : IChopsticksConfigManager
    {
        public string Arguments { get => $"--config={ConfigFile}"; }
    }
}
