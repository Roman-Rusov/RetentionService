using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace RetentionService.Cleanup.Contracts
{
    /// <summary>
    /// Represents a policy that evaluates expiration of resources.
    /// </summary>
    public interface IResourceExpirationPolicy
    {
        /// <summary>
        /// Finds expired resources among the <paramref name="resources"/> specified.
        /// </summary>
        /// <typeparam name="T">The type of an identifier of a resource.</typeparam>
        /// <param name="resources">The sequence resources.</param>
        /// <returns>The sequence of identifiers of expired resources.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resources"/> is <see langword="null"/>.
        /// </exception>
        [NotNull]
        IEnumerable<T> FindExpiredResources<T>(
            [NotNull][ItemNotNull] IReadOnlyCollection<IResource<T>> resources);
    }
}