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
        public void Validate([NotNull, ItemNotNull] IReadOnlyCollection<RetentionRule> rules)
        {
            AssertArg.NotNullOrEmpty(rules, nameof(rules));
            AssertArg.NoNullItems(rules, nameof(rules));

            AssertNoDuplicates(rules);

            AssertNoContradictions(rules);
        }

        private static void AssertNoDuplicates(IReadOnlyCollection<RetentionRule> rules)
        {
            if (!rules.Select(r => r.OlderThan).AreUnique())
            {
                throw new ArgumentException("Duplicate rules are not allowed.", nameof(rules));
            }
        }

        private static void AssertNoContradictions(IReadOnlyCollection<RetentionRule> rules)
        {
            var allowedAmountGrownWithRetention = !rules
                .OrderByDescending(r => r.OlderThan)
                .Select(r => r.AllowedAmount)
                .ToArray()
                .AreOrdered();

            if (allowedAmountGrownWithRetention)
                throw new ArgumentException(
                    "Longer retention rules cannot allow retaining more items " +
                    "than shorter retention rules allow.",
                    nameof(rules));
        }
    }
}