using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Common
{
    /// <summary>
    /// Represent helper class that provides methods to assert arguments.
    /// </summary>
    public static class AssertArg
    {
        /// <summary>
        /// Asserts that input <paramref name="value"/> ins not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="value">The value of the argument.</param>
        /// <param name="paramName">The name of the argument.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <see langword="null"/>.
        /// </exception>
        [ContractAnnotation("value:null => halt")]
        public static void NotNull<T>([NoEnumeration] T value, string paramName)
            where T : class
        {
            if (value == null) throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Asserts that input <paramref name="collection"/> is not <see langword="null"/> or empty.
        /// </summary>
        /// <typeparam name="T">
        /// The type of an item in the <paramref name="collection"/> argument.
        /// </typeparam>
        /// <param name="collection">The value of the argument.</param>
        /// <param name="paramName">The name of the argument.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="collection"/> is empty.
        /// </exception>
        public static void NotNullOrEmpty<T>(IReadOnlyCollection<T> collection, string paramName)
        {
            NotNull(collection, paramName);

            if (collection.Count <= 0)
                throw new ArgumentException(
                    $"{paramName} cannot be empty.",
                    nameof(paramName));
        }

        /// <summary>
        /// Asserts that input <paramref name="collection"/> contains no <see langword="null"/> items.
        /// </summary>
        /// <typeparam name="T">
        /// The type of an item in the <paramref name="collection"/> argument.
        /// </typeparam>
        /// <param name="collection">The value of the argument.</param>
        /// <param name="paramName">The name of the argument.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="collection"/> contains at least one <see langword="null"/> item.
        /// </exception>
        public static void NoNullItems<T>([NotNull] IReadOnlyCollection<T> collection, string paramName)
            where T : class
        {
            NotNull(collection, paramName);

            if (collection.Any(item => item == null))
                throw new ArgumentException(
                    $"{paramName} cannot contain null items.",
                    nameof(paramName));
        }
    }
}