using System;
using System.Collections.Generic;

using Common;
using JetBrains.Annotations;

using RetentionService.RetentionRules;

namespace RetentionService.ConsoleApp.Configuration
{
    /// <summary>
    /// Represents a set of values of configuration settings.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        /// <param name="cleanupDirectoryPath">
        /// The path to the directory to cleanup.
        /// </param>
        /// <param name="retentionRules">
        /// The sequence of retention rules.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cleanupDirectoryPath"/> is <see langword="null"/> or
        /// <paramref name="retentionRules"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="retentionRules"/> contains a <see langword="null"/> item.
        /// </exception>
        public Config(
            [NotNull] string cleanupDirectoryPath,
            [NotNull, ItemNotNull] IReadOnlyCollection<RetentionRule> retentionRules)
        {
            AssertArg.NotNull(cleanupDirectoryPath, nameof(cleanupDirectoryPath));
            AssertArg.NotNull(retentionRules, nameof(retentionRules));
            AssertArg.NoNullItems(retentionRules, nameof(retentionRules));

            CleanupDirectoryPath = cleanupDirectoryPath;
            RetentionRules = retentionRules.ToReadOnlyList();
        }

        /// <summary>
        /// Gets the path to the directory to cleanup specified in the configuration.
        /// </summary>
        /// <value>
        /// Not <see langword="null"/> or filesystem-path to a directory.
        /// </value>
        public string CleanupDirectoryPath { get; }

        /// <summary>
        /// Gets the set of retention rules specified in the configuration.
        /// </summary>
        /// <value>
        /// An immutable, readonly collection of items of the <see cref="RetentionRule"/> class.
        /// </value>
        public IReadOnlyList<RetentionRule> RetentionRules { get; }
    }
}