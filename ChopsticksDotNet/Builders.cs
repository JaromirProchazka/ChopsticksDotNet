using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using YamlDotNet;
using System.CodeDom;
using System.CodeDom.Compiler;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace ChopsticksDotNet
{
    /// <summary>
    /// Builder for Chopsticks @IChopsticksConfigManager. Use GetManager() method to get the Config manager.
    /// </summary>
    public interface IChopsticksConfigBuilder
    {
        /// <summary>
        /// Provides the Manager for Chopstics config.
        /// </summary>
        /// <returns>@IChopsticksConfigManager for the concrete builder.</returns>
        public IChopsticksConfigManager GetManager();
    }

    /// <summary>
    /// Given a chain, this builder will take the default chain config from the Chopsticks repo.
    /// </summary>
    /// <param name="Chain">Chain name corresponding to a .yaml file in the AcalaNetwork/chopsticks/configs repo</param>
    public record class DefaultConfigBuilder(string Chain) : IChopsticksConfigBuilder {
        public IChopsticksConfigManager GetManager() => new DefaultConfig(Chain);
    }

    /// <summary>
    /// Config sourced from a local file path to a config file.
    /// </summary>
    /// /// <param name="ConfigFile">Path to a config file.</param>
    public record class FileBuilder(string ConfigFile) : IChopsticksConfigBuilder
    {
        public IChopsticksConfigManager GetManager() => new DefaultConfig(ConfigFile);
    }

    public record class DirectConfigBuilder : IChopsticksConfigBuilder, IDisposable
    {
        private ConfigData data = new ConfigData();
        private TempFileCollection _tempFiles;
        private string _yamlConfigFileName;
        private string? BaseConfigFile = null;

        public DirectConfigBuilder(string? BaseConfigFile = null)
        {
            this.BaseConfigFile = BaseConfigFile;
            string directoryPath = Environment.CurrentDirectory;

            // Initialize TempFileCollection with the specified directory
            _tempFiles = new TempFileCollection(directoryPath, false); // Set to delete files on Dispose

            // Generate a unique filename with a .yaml extension
            _yamlConfigFileName = _tempFiles.AddExtension("yaml");
        }

        public IChopsticksConfigManager GetManager()
        {
            setBaseConfig();

            string yamlString = serializeToYaml(data);
            File.WriteAllText(_yamlConfigFileName, yamlString);

            return new FileConfig(_yamlConfigFileName);
        }

        private string serializeToYaml(ConfigData config)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            return serializer.Serialize(config);
        }

        private void setBaseConfig()
        {
            // if no base config file provided, let the empty config file
            if (BaseConfigFile == null) return;

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            data = deserializer.Deserialize<ConfigData>(BaseConfigFile);
        }

        /// <summary>
        /// The link to a parachain's raw genesis file to build the fork from, instead of an endpoint.
        /// </summary>
        public DirectConfigBuilder SetGenesis(string newVal) {
            data.Genesis = newVal;
            return this; 
        }

        /// <summary>
        /// Timestamp of the block to fork from.
        /// </summary>
        public DirectConfigBuilder SetTimestamp(string newVal) { 
            data.Timestamp = newVal;
            return this; 
        }

        /// <summary>
        /// The endpoint of the parachain to fork.
        /// </summary>
        public DirectConfigBuilder SetEndpoint(string newVal) { 
            data.Endpoint = newVal;
            return this; 
        }

        /// <summary>
        /// Use to specify at which block hash or number to replay the fork.
        /// </summary>
        public DirectConfigBuilder SetBlock(string newVal) { 
            data.Block = newVal;
            return this; 
        }

        /// <summary>
        /// Path of the WASM to use as the parachain runtime, instead of an endpoint's runtime.
        /// </summary>
        public DirectConfigBuilder SetWasmOverride(string newVal) { 
            data.WasmOverride = newVal;
            return this; 
        }

        /// <summary>
        /// Path to the name of the file that stores or will store the parachain's database.
        /// </summary>
        public DirectConfigBuilder SetDb(string newVal) { 
            data.Db = newVal;
            return this; 
        }

        /// <summary>
        /// Path or URL of the config file.
        /// </summary>
        public DirectConfigBuilder SetConfig(string newVal) { 
            data.Config = newVal;
            return this; 
        }

        /// <summary>
        /// The port to expose an endpoint on.
        /// </summary>
        public DirectConfigBuilder SetPort(string newVal) { 
            data.Port = newVal;
            return this; 
        }

        /// <summary>
        /// How blocks should be built in the fork: batch, manual, instant.
        /// </summary>
        public DirectConfigBuilder SetBuildBlockMode(string newVal) { 
            data.BuildBlockMode = newVal;
            return this; 
        }

        /// <summary>
        /// A pre-defined JSON/YAML storage file path to override in the parachain's storage.
        /// </summary>
        public DirectConfigBuilder SetImportStorage(string newVal) { 
            data.ImportStorage = newVal;
            return this; 
        }

        /// <summary>
        /// Whether to allow WASM unresolved imports when using a WASM to build the parachain.
        /// </summary>
        public DirectConfigBuilder SetAllowUnresolvedImports(string newVal) { 
            data.AllowUnresolvedImports = newVal;
            return this; 
        }

        /// <summary>
        /// Include to generate storage diff preview between blocks.
        /// </summary>
        public DirectConfigBuilder SetHtml(string newVal) { 
            data.Html = newVal;
            return this; 
        }

        /// <summary>
        /// Mock signature host so that any signature starts with 0xdeadbeef and filled by 0xcd is considered valid.
        /// </summary>
        public DirectConfigBuilder SetMockSignatureHost(bool newVal) { 
            data.MockSignatureHost = newVal;
            return this; 
        }

        public void Dispose()
        {
            // Delete the temporary file when disposing the class
            _tempFiles.Delete();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// A class holding data relevant to chain config. It is then serialized to config file. Adding new fields might need adding a corresponding setter to builder.
        /// </summary>
        private class ConfigData
        {
            public string? Genesis { get; set; } = null;
            public string? Timestamp { get; set; } = null;
            public string? Endpoint { get; set; } = null;
            public string? Block { get; set; } = null;
            public string? WasmOverride { get; set; } = null;
            public string? Db { get; set; } = null;
            public string? Config { get; set; } = null;
            public string? Port { get; set; } = null;
            public string? BuildBlockMode { get; set; } = null;
            public string? ImportStorage { get; set; } = null;
            public string? AllowUnresolvedImports { get; set; } = null;
            public string? Html { get; set; } = null;
            public bool? MockSignatureHost { get; set; } = null;
        }
    }
}