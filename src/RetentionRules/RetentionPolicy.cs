using System;
using System.Collections.Generic;
using System.Linq;

using Common;
using JetBrains.Annotations;

using RetentionService.Cleanup.Contracts;

namespace RetentionService.RetentionRules
{
    /// <summary>
    /// Represents the retention policy.
    /// </summary>
    /// <remarks>
    /// The policy is defined by a set of conformed retention rules that
    /// do neither contradict nor duplicate each other.
    /// </remarks>
    public class RetentionPolicy : IStaleItemsDetector
    {
        private static readonly RulesConsistencyValidator Validator = new RulesConsistencyValidator();

        private readonly IReadOnlyList<RetentionRule> _orderedRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetentionPolicy" /> class.
        /// </summary>
        /// <param name="rules">
        /// The collection of rules that define the policy.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rules"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Any item in the <paramref name="rules"/> is <see langword="null"/> or
        /// <paramref name="rules"/> is empty or
        /// <paramref name="rules"/> contains duplicate or mutually contradictory items.
        /// </exception>
        public RetentionPolicy([NotNull][ItemNotNull] IReadOnlyCollection<RetentionRule> rules)
        {
            AssertArg.NotNullOrEmpty(rules, nameof(rules));
            AssertArg.NoNullItems(rules, nameof(rules));

            Validator.Validate(rules);

            _orderedRules = rules
                .OrderBy(r => r.OlderThan)
                .ToReadOnlyList();
        }

        /// <summary>
        /// Finds and returns stale items.
        /// </summary>
        /// <typeparam name="T">The type of an item.</typeparam>
        /// <param name="items">The sequence of pairs of an item and its age.</param>
        /// <returns>The sequence of stale items.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="items" /> is <see langword="null" />.
        /// </exception>
        public IEnumerable<T> FindStaleItems<T>(IEnumerable<(T item, TimeSpan age)> items)
        {
            AssertArg.NotNull(items, nameof(items));

            var orderedItems = items.OrderBy(i => i.age).ToArray();
            var examineScope = (startIndex: 0, length: orderedItems.Length);

            foreach (var rule in _orderedRules)
            {
                if (examineScope.length <= 0) break;

                var examineScopeItems = orderedItems
                    .Skip(examineScope.startIndex)
                    .Take(examineScope.length)
                    .ToArray();

                var outOfRuleCount = examineScopeItems
                    .TakeWhile(i => i.age <= rule.OlderThan)
                    .Count();

                var retainCount = examineScopeItems
                    .Skip(outOfRuleCount)
                    .Take(rule.AllowedAmount)
                    .Count();

                examineScope.startIndex += outOfRuleCount;
                examineScope.length = retainCount;
            }

            var totalRetainCount =
                examineScope.startIndex +
                examineScope.length;

            return orderedItems
                .Skip(totalRetainCount)
                .Select(i => i.item);
        }

        /// <summary>
        /// Returns a string that represents a collection of rules that define the policy.
        /// </summary>
        /// <returns>
        /// A string that represents a collection of rules that define the policy.
        /// </returns>
        public override string ToString() =>
            string.Join(" ", _orderedRules);
    }
}