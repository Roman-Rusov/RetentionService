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
        /// Concatenates the items of the <paramref name="values"/> sequence
        /// using the specified <paramref name="separator"/> between each item.
        /// </summary>
        /// <typeparam name="T">
        /// The type of an item in the <paramref name="values"/> argument.
        /// </typeparam>
        /// <param name="values"> The sequence of items. </param>
        /// <param name="separator">
        /// The string to use as a separator.
        /// It's included in the returned string only if <paramref name="values"/> sequence has more than one item.
        /// </param>
        /// <returns>
        /// A string that consists of the items of the <paramref name="values"/> sequence
        /// delimited by the <paramref name="separator"/> string.
        /// If <paramref name="values"/> sequence has no items, the method returns <see cref="string.Empty"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> or <paramref name="separator"/> is <see langword="null"/>.
        /// </exception>
        public static string JoinThrough<T>(
            [NotNull] this IEnumerable<T> values,
            [NotNull] string separator)
        {
            AssertArg.NotNull(values, nameof(values));
            AssertArg.NotNull(separator, nameof(values));

            return string.Join(separator, values);
        }

        /// <summary>
        /// Determines whether the items of the <paramref name="values"/> sequence are unique.
        /// </summary>
        /// <typeparam name="T">
        /// The type of an item in the <paramref name="values"/> argument.
        /// </typeparam>
        /// <param name="values"> The sequence of items. </param>
        /// <param name="comparer">
        /// The comparer to use for comparing items of the <paramref name="values"/> sequence.
        /// If <see langword="null"/> is provided, then default comparer will be used.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if all the items of the <paramref name="values"/> sequence are unique,
        /// otherwise <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <see langword="null"/>.
        /// </exception>
        public static bool AreUnique<T>(
            [NotNull] this IEnumerable<T> values,
            [CanBeNull] IEqualityComparer<T> comparer = null)
        {
            AssertArg.NotNull(values, nameof(values));

            var uniqueSet = new HashSet<T>(comparer);

            return values.All(item => uniqueSet.Add(item));
        }

        /// <summary>
        /// Determines whether the items of the <paramref name="values"/> sequence are ordered.
        /// </summary>
        /// <typeparam name="T">
        /// The type of an item in the <paramref name="values"/> argument.
        /// </typeparam>
        /// <param name="values"> The sequence of items. </param>
        /// <returns>
        /// <see langword="true"/> if all the items of the <paramref name="values"/> sequence
        /// are not less then their previous item, otherwise <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is <see langword="null"/>.
        /// </exception>
        public static bool AreOrdered<T>(
            [NotNull] this IReadOnlyCollection<T> values)
            where T : IComparable<T>
        {
            AssertArg.NotNull(values, nameof(values));

            return values
                .Zip(
                    values.Skip(1),
                    (prev, next) => next.CompareTo(prev) >= 0)
                .All(IsTrue);
        }

        /// <summary>
        /// Returns a new immutable, readonly list that contains items
        /// from the <paramref name="source"/> sequence.
        /// </summary>
        /// <typeparam name="T">
        /// The type of an item in the <paramref name="source"/> argument.
        /// </typeparam>
        /// <param name="source"> The sequence of items. </param>
        /// <returns>
        /// A new immutable, readonly list that contains items
        /// from the <paramref name="source"/> sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public static IReadOnlyList<T> ToReadOnlyList<T>(
            [NotNull] this IEnumerable<T> source)
        {
            AssertArg.NotNull(source, nameof(source));

            return new ReadOnlyCollection<T>(source.ToArray());
        }

        private static bool IsTrue(bool condition) => condition;
    }
}