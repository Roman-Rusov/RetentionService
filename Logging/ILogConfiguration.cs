using JetBrains.Annotations;

namespace Logging
{
    /// <summary>
    /// Represents the set of configuration options required to setup logging.
    /// </summary>
    public interface ILogConfiguration
    {
        /// <summary>
        /// Gets the full path of the log configuration file.
        /// </summary>
        /// <value>
        /// Not <see langword="null"/> name of a file.
        /// </value>
        [NotNull] string ConfigFilePath { get; }
    }
}