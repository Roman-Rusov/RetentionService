using System;

using RetentionService.Cleanup.Contracts;

namespace RetentionService.Tests.Common
{
    /// <summary>
    /// Represents a fake resource.
    /// </summary>
    public class FakeResource : IResource<string>
    {
        /// <summary>
        /// The identifier of the resource.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The age of the resource.
        /// </summary>
        public TimeSpan Age { get; set; }
    }
}