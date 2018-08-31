using System;

namespace RetentionService.Cleanup.Contracts
{
    /// <summary>
    /// Represents a object that encapsulates set of properties of a resource.
    /// </summary>
    public class ResourceDetails
    {
        /// <summary>
        /// The address of the resource.
        /// </summary>
        /// <value>
        /// A URI, a filepath, or any other arbitrary address that can identify the resource.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// The age of the resource.
        /// </summary>
        /// <value>
        /// Positive time interval.
        /// </value>
        public TimeSpan Age { get; set; }

        /// <summary>
        /// Returns a string that represents the details of the resource.
        /// </summary>
        /// <returns>A string that represents the details of the resource.</returns>
        public override string ToString() =>
            $"{{ {Age:c}; {Address}}}";
    }
}