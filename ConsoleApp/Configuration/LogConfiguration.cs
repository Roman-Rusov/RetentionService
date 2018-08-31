using System.Configuration;
using System.IO;
using System.Reflection;

using Common;
using JetBrains.Annotations;
using Logging;

namespace RetentionService.ConsoleApp.Configuration
{
    /// <summary>
    /// Represents the set of configuration options required to setup logging.
    /// </summary>
    public class LogConfiguration : ILogConfiguration
    {
        /// <summary>
        /// Gets the path of the log configuration file.
        /// </summary>
        /// <value>
        /// Not <see langword="null"/> name of a file.
        /// </value>
        public string ConfigFilePath { get; }

        /// <summary>
        /// Reads and builds an instance of the <see cref="LogConfiguration"/> class.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="LogConfiguration"/> class.
        /// </returns>
        [NotNull]
        public static LogConfiguration BuildConfig()
        {
            var configFileName = ConfigurationManager.AppSettings["LogConfigFilePath"];

            if (string.IsNullOrWhiteSpace(configFileName))
            {
                configFileName = LoggingModule.DefaultConfigFileName;
            }

            if (!Path.IsPathRooted(configFileName))
            {
                var assemblyLocation = Assembly.GetEntryAssembly().Location;
                var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

                configFileName = Path.Combine(assemblyDirectory, configFileName);
            }

            return new LogConfiguration(configFileName);
        }

        private LogConfiguration(string configFilePath)
        {
            AssertArg.NotNull(configFilePath, nameof(configFilePath));

            ConfigFilePath = configFilePath;
        }
    }
}