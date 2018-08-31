using System;
using System.Configuration;
using System.Linq;

using Common;
using JetBrains.Annotations;

using RetentionService.RetentionRules;

namespace RetentionService.ConsoleApp.Configuration
{
    /// <summary>
    /// Represents the builder of configuration.
    /// </summary>
    public class ConfigurationBuilder
    {
        [CanBeNull] private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationBuilder"/> class.
        /// </summary>
        public ConfigurationBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationBuilder"/> class.
        /// </summary>
        /// <param name="log">
        /// A log where to write log messages into.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="log"/> is <see langword="null"/>.
        /// </exception>
        public ConfigurationBuilder([NotNull] ILog log) : this()
        {
            AssertArg.NotNull(log, nameof(log));

            _log = log;
        }

        /// <summary>
        /// Reads configuration settings and builds a new instance of the <see cref="Config"/> class.
        /// </summary>
        /// <returns> An instance of <see cref="Config"/> class. </returns>
        [NotNull]
        public Config BuildConfig()
        {
            var cleanupDirectoryPath = ConfigurationManager.AppSettings["CleanupDirectoryPath"];

            _log?.Debug($@"Config: CleanupDirectoryPath: ""{cleanupDirectoryPath}""");

            var retentionRules = ConfigurationManager
                .AppSettings["RetentionRules"]
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Split(':'))
                .Select(s => new RetentionRule(
                    olderThan: TimeSpan.FromDays(int.Parse(s[0])),
                    allowedAmount: int.Parse(s[1])))
                .ToArray();

            _log?.Debug($"Config: RetentionRules: {string.Join(" ", retentionRules.AsEnumerable())}");

            return new Config(
                cleanupDirectoryPath,
                retentionRules);
        }
    }
}