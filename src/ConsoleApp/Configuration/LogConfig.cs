using Common;
using Logging;

namespace RetentionService.ConsoleApp.Configuration
{
    /// <summary>
    /// Represents the set of configuration options required to setup logging.
    /// </summary>
    public class LogConfig : ILogConfiguration
    {
        /// <summary>
        /// Gets the path of the log configuration file.
        /// </summary>
        /// <value>
        /// Not <see langword="null"/> name of a file.
        /// </value>
        public string ConfigFilePath { get; }

        public LogConfig(string configFilePath)
        {
            AssertArg.NotNull(configFilePath, nameof(configFilePath));

            ConfigFilePath = configFilePath;
        }
    }
}