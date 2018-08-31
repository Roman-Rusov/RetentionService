using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using JetBrains.Annotations;

namespace RetentionService.RetentionRules
{
    /// <summary>
    /// Represents validator of consistency of retention rules.
    /// </summary>
    public class RulesConsistencyValidator
    {
        /// <summary>
        /// Performs validation of the <paramref name="rules"/>.
        /// </summary>
        /// <param name="rules"> The set of retention rules to validate. </param>
        public void Validate([NotNull] [ItemNotNull] IReadOnlyCollection<RetentionRule> rules)
        {
            AssertArg.NotNullOrEmpty(rules, nameof(rules));
            AssertArg.NoNullItems(rules, nameof(rules));

            AssertNoDuplications(rules);

            AssertNoCntradictions(rules);
        }

        private static void AssertNoDuplications(IReadOnlyCollection<RetentionRule> rules)
        {
            var uniqueRuleCount = new HashSet<TimeSpan>(rules.Select(r => r.OlderThan)).Count;
            if (uniqueRuleCount != rules.Count)
            {
                throw new ArgumentException("Duplicate rules are not allowed.", nameof(rules));
            }
        }

        private static void AssertNoCntradictions(IReadOnlyCollection<RetentionRule> rules)
        {
            var orderedRules = rules.OrderBy(r => r.OlderThan).ToArray();
            var shorterAllowedAmount = orderedRules.First().AllowedAmount;
            orderedRules.Skip(1).ForEach(r =>
            {
                var currentAllowedAmount = r.AllowedAmount;

                if (currentAllowedAmount > shorterAllowedAmount)
                    throw new ArgumentException(
                        "Longer retention rules cannot allow retaining more items " +
                        "than shorter retention rules allow.",
                        nameof(rules));

                shorterAllowedAmount = currentAllowedAmount;
            });
        }
    }
}