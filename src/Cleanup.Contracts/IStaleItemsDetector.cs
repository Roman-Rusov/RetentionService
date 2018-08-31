using System;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace RetentionService.Cleanup.Contracts
{
    /// <summary>
    /// Represents a mean to detect stale items.
    /// </summary>
    public interface IStaleItemsDetector
    {
        /// <summary>
        /// Finds and returns stale items.
        /// </summary>
        /// <typeparam name="T">The type of an item.</typeparam>
        /// <param name="items">The sequence of pairs of an item and its age.</param>
        /// <returns>The sequence of stale items.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is <see langword="null"/>.
        /// </exception>
        [NotNull]
        IEnumerable<T> FindStaleItems<T>([NotNull] IEnumerable<(T item, TimeSpan age)> items);
    }
}