using System;
using System.Linq;
using System.Threading.Tasks;

using Common;
using JetBrains.Annotations;

using RetentionService.Cleanup.Contracts;

namespace RetentionService.Cleanup
{
    /// <summary>
    /// Represents the objects that performs cleaning up in a storage of resources.
    /// </summary>
    public class CleanupExecutor
    {
        [CanBeNull] private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanupExecutor"/> class.
        /// </summary>
        public CleanupExecutor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanupExecutor"/> class.
        /// </summary>
        /// <param name="log">
        /// A log where to write log messages into.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="log"/> is <see langword="null"/>.
        /// </exception>
        public CleanupExecutor([NotNull] ILog log) : this()
        {
            AssertArg.NotNull(log, nameof(log));

            _log = log;
        }

        /// <summary>
        /// Finds out stale resources in the <paramref name="resourceStorage"/>
        /// and then deletes them.
        /// </summary>
        /// <param name="resourceStorage">
        /// The storage where to perform cleanup in.
        /// </param>
        /// <param name="staleItemsDetector">
        /// The mean to find out which resources are stale.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resourceStorage"/> is <see langword="null"/> or
        /// <paramref name="staleItemsDetector"/> is <see langword="null"/>.
        /// </exception>
        public async Task ExecuteStorageCleanup(
            [NotNull] IResourceStorage resourceStorage,
            [NotNull] IStaleItemsDetector staleItemsDetector)
        {
            AssertArg.NotNull(resourceStorage, nameof(resourceStorage));
            AssertArg.NotNull(staleItemsDetector, nameof(staleItemsDetector));

            _log?.Debug($"Calling {resourceStorage.GetType().Name} storage for resource details.");

            var resourceDetails = (await resourceStorage.GetResourceDetails()).ToArray();

            _log?.Debug($"Fetched details about {resourceDetails.Length} resources.");

            var itemsToExamine = resourceDetails
                .OrderByDescending(rd => rd.Age)
                .Select(rd => (rd.Address, rd.Age));

            var resourceAddressesToDelete = staleItemsDetector
                .FindStaleItems(itemsToExamine)
                .ToArray();

            if (!resourceAddressesToDelete.Any())
            {
                _log?.Info("No stale resources were found thus no resource is deleted.");

                return;
            }

            _log?.Debug($"Calling {resourceStorage.GetType().Name} storage for stale resources deletion.");

            await resourceStorage.DeleteResources(resourceAddressesToDelete);

            _log?.Debug($"Deleted {resourceAddressesToDelete.Length} stale resources.");
        }
    }
}