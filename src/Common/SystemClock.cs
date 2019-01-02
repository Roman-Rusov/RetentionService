using System;

namespace Common
{
    /// <summary>
    /// Represents the source of system clock.
    /// </summary>
    public class SystemClock : ISystemClock
    {
        /// <summary>
        /// Gets the current system time in UTC.
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;
    }
}