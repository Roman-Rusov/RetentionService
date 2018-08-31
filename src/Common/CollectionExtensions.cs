using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using JetBrains.Annotations;

namespace Common
{
    /// <summary>
    /// Represents the set of extensions methods to collection types.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Performs the <paramref name="action"/> specified on each item
        /// in the <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The type of an item in the <paramref name="source"/> argument.
        /// </typeparam>
        /// <param name="source">
        /// The sequence of items that the <paramref name="action"/> is to be performed on.
        /// </param>
        /// <param name="action">
        /// The action to perform on each item in the <paramref name="source"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/> or
        /// <paramref name="action"/> is <see langword="null"/>.
        /// </exception>
        public static void ForEach<T>(
            [NotNull] this IEnumerable<T> source,
            [NotNull] Action<T> action)
        {
            AssertArg.NotNull(source, nameof(source));
            AssertArg.NotNull(action, nameof(action));

            foreach (var item in source) action(item);
        }

        /// <summary>
        /// Returns a new immutable, readonly list that contains items
        /// from the <paramref name="source"/> sequence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of an item in the <paramref name="source"/> argument.
        /// </typeparam>
        /// <param name="source"> The sequence of items. </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public static IReadOnlyList<T> ToReadOnlyList<T>(
            [NotNull] this IEnumerable<T> source)
        {
            AssertArg.NotNull(source, nameof(source));

            return new ReadOnlyCollection<T>(source.ToArray());
        }
    }
}