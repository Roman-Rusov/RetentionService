using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace RetentionService.Cleanup.Contracts
{
    /// <summary>
    /// Represents a storage of arbitrary resources.
    /// </summary>
    /// <typeparam name="T">
    /// The type of an identifier of a resource.
    /// </typeparam>
    public interface IResourceStorage<T>
    {
        /// <summary>
        /// Gets all the resources being stored in the storage.
        /// </summary>
        /// <returns> A sequence of resources. </returns>
        [NotNull]
        Task<IEnumerable<IResource<T>>> GetResourceDetails();

        /// <summary>
        /// Deletes resources specified by the <paramref name="resourceIds"/> from the storage.
        /// </summary>
        /// <param name="resourceIds">
        /// The identifiers of the resources to delete.
        /// </param>
        Task DeleteResources([NotNull, ItemNotNull] IEnumerable<T> resourceIds);
    }
}