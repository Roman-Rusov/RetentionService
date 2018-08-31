using System;

namespace RetentionService.RetentionRules
{
    /// <summary>
    /// Represents a retention rule.
    /// </summary>
    public class RetentionRule
    {
        /// <summary>
        /// The condition of applying the rule. If age of an object is greater than the value
        /// of the property then the rule should be applied to the object.
        /// </summary>
        /// <value>
        /// Zero or positive time interval.
        /// </value>
        public TimeSpan OlderThan { get; }

        /// <summary>
        /// The maximal allowed amount of retained objects whose retention is covered by the rule.
        /// </summary>
        /// <value>
        /// Zero or positive integer.
        /// </value>
        public int AllowedAmount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetentionRule" /> class.
        /// </summary>
        /// <param name="olderThan">
        /// The condition of applying the rule. If age of an object is greater than the value
        /// of the property then the rule should be applied to the object.
        /// </param>
        /// <param name="allowedAmount">
        /// The maximal allowed amount of retained objects whose retention is covered by the rule.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="olderThan"/> is less than <see cref="TimeSpan.Zero"/> or
        /// <paramref name="allowedAmount"/> is less than <c>0</c>.
        /// </exception>
        public RetentionRule(TimeSpan olderThan, int allowedAmount)
        {
            AssertRetentionTimeout(olderThan);
            AssertRetentionAmount(allowedAmount);

            OlderThan = olderThan;
            AllowedAmount = allowedAmount;
        }

        /// <summary>
        /// Returns a string that represents the rule.
        /// </summary>
        /// <returns>A string that represents the rule.</returns>
        public override string ToString() =>
            $"{OlderThan.Days}:{AllowedAmount}";

        private static void AssertRetentionAmount(int allowedAmount)
        {
            if (allowedAmount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(allowedAmount),
                    "Amount of items to allow being retained cannot be less than zero.");
        }

        private static void AssertRetentionTimeout(TimeSpan olderThan)
        {
            if (olderThan < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(
                    nameof(olderThan),
                    "Retention period cannot be less than zero.");
        }
    }
}