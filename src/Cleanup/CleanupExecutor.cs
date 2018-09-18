using System;
using System.Linq;
using System.Threading.Tasks;

using Common;
using JetBrains.Annotations;

using RetentionService.Cleanup.Contracts;

namespace RetentionService.Cleanup
{
    /// <summary>
    /// Represents the object that performs cleaning up in a storage of resources.
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
        /// Finds out expired resources in the <paramref name="resourceStorage"/> and deletes them.
        /// </summary>
        /// <typeparam name="T"> The type of an identifier of a resource. </typeparam>
        /// <param name="resourceStorage">
        /// The storage where to perform cleanup in.
        /// </param>
        /// <param name="expirationPolicy">
        /// The resource expiration policy.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resourceStorage"/> is <see langword="null"/> or
        /// <paramref name="expirationPolicy"/> is <see langword="null"/>.
        /// </exception>
        public async Task ExecuteStorageCleanup<T>(
            [NotNull] IResourceStorage<T> resourceStorage,
            [NotNull] IResourceExpirationPolicy expirationPolicy)
        {
            AssertArg.NotNull(resourceStorage, nameof(resourceStorage));
            AssertArg.NotNull(expirationPolicy, nameof(expirationPolicy));

            _log?.Debug($"Calling {resourceStorage.GetType().Name} storage for resource details.");

            var resources = (await resourceStorage.GetResourceDetails()).ToArray();

            _log?.Debug($"Fetched details about {resources.Length} resources.");

            var resourceIdsToDelete = expirationPolicy
                .FindExpiredResources(resources)
                .ToArray();

            if (!resourceIdsToDelete.Any())
            {
                _log?.Info("No expired resources were found thus no resource is deleted.");

                return;
            }

            _log?.Debug($"Calling {resourceStorage.GetType().Name} storage for expired resources deletion.");

            await resourceStorage.DeleteResources(resourceIdsToDelete);

            _log?.Debug($"Deleted {resourceIdsToDelete.Length} expired resources.");
        }
    }
}
