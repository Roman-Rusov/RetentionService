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

            var resources = (await resourceStorage.GetResourceDetails()).ToArray();

            var resourceIdsToDelete = expirationPolicy
                .FindExpiredResources(resources)
                .ToArray();

            if (!resourceIdsToDelete.Any())
            {
                return;
            }

            await resourceStorage.DeleteResources(resourceIdsToDelete);
        }
    }
}
