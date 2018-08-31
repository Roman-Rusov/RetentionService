using System;
using System.Linq;

using RetentionService.Cleanup.Contracts;
using RetentionService.RetentionRules;

using static System.StringSplitOptions;

namespace RetentionService.Tests.Common
{
    /// <summary>
    /// Represents the set of extension methods to facilitate parsing of test parameters.
    /// </summary>
    public static class TestParamsParsingHelper
    {
        /// <summary>
        /// Parses the string that represents a set of retention rules.
        /// </summary>
        /// <param name="rulesData">The string that represents a set of retention rules.</param>
        /// <returns>The sequence of <see cref="RetentionRule"/> instances.</returns>
        /// <remarks>
        /// Each rule is represented by a colon separated pair of integers, where
        /// the first number represents <see cref="RetentionRule.OlderThan"/> value in days
        /// and the second one represents <see cref="RetentionRule.AllowedAmount"/>.
        /// </remarks>
        /// <example>
        /// "1:10 5:4 10:2 14:1 21:0".ParseRules() should return five retention rules.
        /// </example>
        public static RetentionRule[] ParseRules(this string rulesData) =>
            rulesData
                .Split(new[] { ' ' }, RemoveEmptyEntries)
                .Select(r => r.Split(':'))
                .Select(a => new RetentionRule(
                    TimeSpan.FromDays(int.Parse(a[0])),
                    int.Parse(a[1])))
                .ToArray();

        /// <summary>
        /// Parses the string that represents a sequence of space separated resource details items.
        /// Each item is considered to be a comma separated pair of a resource's address and age.
        /// Item address shouldn't contain neither space symbols and nor comma (:).
        /// Item ages are expressed in days and represented as <see langword="double"/>.
        /// It is allowed to omit resource address, then the age will be considered as the address.
        /// </summary>
        /// <param name="itemAges"> The set of resources' addresses and ages. </param>
        /// <returns> The sequence of resource details. </returns>
        /// <example>
        /// "1:0.2 2:0.9 3:1.7 4:2 5:3.3 19 1250.01 30 an_address:100.00".ParseResourceDetails()
        /// </example>
        public static ResourceDetails[] ParseResourceDetails(this string itemAges) =>
            itemAges
                .Split(new[] { ' ' }, RemoveEmptyEntries)
                .Select(n =>
                {
                    var addressAgePair = n.Contains(":")
                        ? n.Split(':')
                        : new[] { n, n };

                    return new ResourceDetails
                    {
                        Address = addressAgePair[0],
                        Age = TimeSpan.FromDays(double.Parse(addressAgePair[1]))
                    };
                })
                .ToArray();
    }
}