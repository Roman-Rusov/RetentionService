using System.IO;
using System.Reflection;

using JetBrains.Annotations;
using Logging;
using Microsoft.Extensions.Configuration;

namespace RetentionService.ConsoleApp.Configuration
{
    /// <summary>
    /// Represents the builder of logging configuration.
    /// </summary>
    public class LogConfigBuilder
    {
        private const string LoggingSectionName = "log4net";
        private const string LogConfigFilePathPropertyName = "ConfigFilePath";

        private static string AssemblyDirectory =>
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        /// <summary>
        /// Reads logging configuration file and builds a new instance of the <see cref="LogConfig"/> class.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="LogConfig"/> class.
        /// </returns>
        [NotNull]
        public static LogConfig Build()
        {
            var logConfigFilePath = ReadLogConfigFilePath();

            if (string.IsNullOrWhiteSpace(logConfigFilePath))
            {
                logConfigFilePath = LoggingModule.DefaultConfigFileName;
            }

            if (!Path.IsPathRooted(logConfigFilePath))
            {
                logConfigFilePath = Path.Combine(AssemblyDirectory, logConfigFilePath);
            }

            return new LogConfig(logConfigFilePath);
        }

        private static string ReadLogConfigFilePath()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AssemblyDirectory)
                .AddIniFile("log.config.ini", optional: true)
                .Build();

            return config[$"{LoggingSectionName}:{LogConfigFilePathPropertyName}"];
        }
    }
}