using System;

using RetentionService.Cleanup.Contracts;

namespace RetentionService.FileSystemStorage
{
    /// <summary>
    /// Represents an class that describes a file resource.
    /// </summary>
    public class FileResource : IResource<string>
    {
        /// <summary>
        /// Gets or sets the full path to the file.
        /// </summary>
        /// <value>
        /// Not <see langword="null"/> path in a file system.
        /// </value>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets or sets the time passed from the last modification of the file.
        /// </summary>
        /// <value>
        /// Positive time interval.
        /// </value>
        public TimeSpan Age { get; internal set; }

        /// <summary>
        /// Returns a string that represents details of the file resource.
        /// </summary>
        /// <returns> A string that represents details of the file resource. </returns>
        public override string ToString() =>
            $"{{ {Age:c}; {Id} }}";
    }
}