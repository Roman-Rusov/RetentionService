using System;

using JetBrains.Annotations;

namespace RetentionService.Cleanup.Contracts
{
    /// <summary>
    /// Represents an interface that describes a resource.
    /// </summary>
    /// <typeparam name="T">
    /// The type of an identifier of a resource.
    /// </typeparam>
    public interface IResource<out T>
    {
        /// <summary>
        /// Gets the identifier of the resource.
        /// </summary>
        /// <value>
        /// Any arbitrary not <see langword="null"/> value that can uniquely identify the resource.
        /// </value>
        [NotNull]
        T Id { get; }

        /// <summary>
        /// Gets the age of the resource.
        /// </summary>
        /// <value>
        /// Positive time interval.
        /// </value>
        TimeSpan Age { get; }
    }
}