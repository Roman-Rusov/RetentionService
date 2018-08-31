using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace RetentionService.Cleanup.Contracts
{
    /// <summary>
    /// Represents a storage of arbitrary resources.
    /// </summary>
    public interface IResourceStorage
    {
        /// <summary>
        /// Gets details on all the resources being stored in the storage.
        /// </summary>
        /// <returns> A sequence of details of resources. </returns>
        [NotNull]
        Task<IEnumerable<ResourceDetails>> GetResourceDetails();

        /// <summary>
        /// Deletes resources specified by the <paramref name="resourceAddresses"/>
        /// from the storage.
        /// </summary>
        /// <param name="resourceAddresses">
        /// The addresses that identify the resources to delete.
        /// </param>
        Task DeleteResources([NotNull, ItemNotNull] IEnumerable<string> resourceAddresses);
    }
}