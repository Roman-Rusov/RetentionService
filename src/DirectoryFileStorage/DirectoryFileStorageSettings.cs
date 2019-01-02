using System;

using Common;
using JetBrains.Annotations;

namespace RetentionService.FileSystemStorage
{
    /// <summary>
    /// Represents a set of settings of the <see cref="DirectoryFileStorage"/>.
    /// </summary>
    public class DirectoryFileStorageSettings
    {
        /// <summary>
        /// The directory where to perform a cleanup.
        /// </summary>
        /// <value>
        /// A path in a filesystem.
        /// </value>
        [NotNull]
        public string DirectoryPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryFileStorageSettings"/> class.
        /// </summary>
        /// <param name="directoryPath">
        /// The directory where to perform a cleanup.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="directoryPath"/> is <see langword="null"/> or empty, or whitespace.
        /// </exception>
        public DirectoryFileStorageSettings([NotNull] string directoryPath)
        {
            AssertArg.NotNullOrWhiteSpace(directoryPath, nameof(directoryPath));

            DirectoryPath = directoryPath;
        }
    }
}