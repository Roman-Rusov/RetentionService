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
        private readonly ISystemClock _systemClock;
        private readonly DirectoryFileStorageSettings _settings;
        [CanBeNull] private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryFileStorage" /> class.
        /// </summary>
        /// <param name="systemClock">
        /// The source of system clock.
        /// </param>
        /// <param name="settings">
        /// The settings of the storage.
        /// </param>
        /// <param name="log">
        /// A log where to write log messages into.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="systemClock"/> or
        /// <paramref name="settings"/> or
        /// <paramref name="log"/> is <see langword="null"/>.
        /// </exception>
        public DirectoryFileStorage(
            [NotNull] ISystemClock systemClock,
            [NotNull] DirectoryFileStorageSettings settings,
            [NotNull] ILog log)
            : this(systemClock, settings)
        {
            AssertArg.NotNull(log, nameof(log));

            _log = log;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryFileStorage" /> class.
        /// </summary>
        /// <param name="systemClock">
        /// The source of system clock.
        /// </param>
        /// <param name="settings">
        /// The settings of the storage.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="systemClock"/> or
        /// <paramref name="settings"/> is <see langword="null"/>.
        /// </exception>
        public DirectoryFileStorage(
            [NotNull] ISystemClock systemClock,
            [NotNull] DirectoryFileStorageSettings settings)
        {
            AssertArg.NotNull(systemClock, nameof(systemClock));
            AssertArg.NotNull(settings, nameof(settings));

            _systemClock = systemClock;
            _settings = settings;
        }

        /// <summary>
        /// Gets details on all the file resources being stored in the storage.
        /// </summary>
        /// <returns> A sequence of details of file resources. </returns>
        public Task<IEnumerable<IResource<string>>> GetResourceDetails()
        {
            var utcNow = _systemClock.UtcNow;

            var fileDetailsQuery =
                from fullFilePath
                    in Directory.EnumerateFiles(_settings.DirectoryPath)
                let lastWriteTimeUtc = File.GetLastWriteTimeUtc(fullFilePath)
                select new FileResource
                    {
                        Id = fullFilePath,
                        Age = utcNow - lastWriteTimeUtc
                    };

            var fileResources = fileDetailsQuery.ToArray();

            _log?.Debug(
                fileResources.Any()
                    ? $"The following files are found:{NewLine}" +
                      fileResources.Select(fr => $"\t{fr}").JoinThrough(NewLine)
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
            AssertArg.NotNull(resourceIds, nameof(resourceIds));

            var fileNamesToDelete = resourceIds.ToArray();

            fileNamesToDelete.ForEach(File.Delete);

            _log?.Debug(
                $"The following files were removed:{NewLine}" +
                fileNamesToDelete.Select(fn => $"\t{fn}").JoinThrough(NewLine));

            return Task.CompletedTask;
        }
    }
}