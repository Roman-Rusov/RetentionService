using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Common;
using JetBrains.Annotations;

using RetentionService.Cleanup.Contracts;

using static System.Environment;

namespace RetentionService.FileSystemStorage
{
    /// <summary>
    /// Represents the storage that essentially is a directory containing files.
    /// </summary>
    public class DirectoryFileStorage : IResourceStorage<string>
    {
        private readonly string _directoryPath;
        [CanBeNull] private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryFileStorage" /> class.
        /// </summary>
        /// <param name="directoryPath">
        /// The directory where to perform a cleanup.
        /// </param>
        /// <param name="log">
        /// A log where to write log messages into.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="directoryPath"/> is <see langword="null"/> or empty, or
        /// <paramref name="log"/> is <see langword="null"/>.
        /// </exception>
        public DirectoryFileStorage(string directoryPath, ILog log)
            : this(directoryPath)
        {
            AssertArg.NotNull(log, nameof(log));

            _log = log;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryFileStorage" /> class.
        /// </summary>
        /// <param name="directoryPath">
        /// The directory where to perform a cleanup.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="directoryPath"/> is <see langword="null"/> or empty.
        /// </exception>
        public DirectoryFileStorage(string directoryPath)
        {
            AssertDirectoryPath(directoryPath);

            _directoryPath = directoryPath;
        }

        /// <summary>
        /// Gets details on all the file resources being stored in the storage.
        /// </summary>
        /// <returns> A sequence of details of file resources. </returns>
        public Task<IEnumerable<IResource<string>>> GetResourceDetails()
        {
            var fileDetailsQuery =
                from fullFilePath
                    in Directory.EnumerateFiles(_directoryPath)
                let lastWriteTimeUtc = File.GetLastWriteTimeUtc(fullFilePath)
                select new FileResource
                    {
                        Id = fullFilePath,
                        Age = DateTime.UtcNow - lastWriteTimeUtc
                    };

            var fileResources = fileDetailsQuery.ToArray();

            _log?.Debug(
                fileResources.Any()
                    ? $"The following files are found:{NewLine}" +
                      $"{string.Join(NewLine, fileResources.Select(fr => $"\t{fr}"))}"
                    : "No files are found.");

            var result = fileResources
                .Cast<IResource<string>>()
                .AsEnumerable();

            return Task.FromResult(result);
        }

        /// <summary>
        /// Deletes files that are specified in the <paramref name="resourceIds"/> argument.
        /// </summary>
        /// <param name="resourceIds">
        /// The sequence of full names of files to delete.
        /// </param>
        public Task DeleteResources(IEnumerable<string> resourceIds)
        {
            var fileNamesToDelete = resourceIds.ToArray();

            fileNamesToDelete.ForEach(File.Delete);

            _log?.Debug(
                $"The following files were removed:{NewLine}" +
                $"{string.Join(NewLine, fileNamesToDelete.Select(fn => $"\t{fn}"))}");

            return Task.CompletedTask;
        }

        private static void AssertDirectoryPath(string backupDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(backupDirectoryPath))
                throw new ArgumentNullException(
                    nameof(backupDirectoryPath),
                    "Backup directory path cannot be empty.");
        }
    }
}