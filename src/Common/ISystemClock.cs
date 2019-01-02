using System;

namespace Common
{
    /// <summary>
    /// Represents the source of system clock.
    /// </summary>
    public interface ISystemClock
    {
        /// <summary>
        /// Gets the current system time in UTC.
        /// </summary>
        DateTime UtcNow { get; }
    }
}