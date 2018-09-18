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
    public class RetentionPolicy : IResourceExpirationPolicy
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
        /// Finds expired resources among the <paramref name="resources"/> specified.
        /// </summary>
        /// <typeparam name="T"> The type of an identifier of a resource. </typeparam>
        /// <param name="resources">The sequence resources.</param>
        /// <returns> The sequence of identifiers of expired resources. </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resources"/> is <see langword="null"/>.
        /// </exception>
        public IEnumerable<T> FindExpiredResources<T>(IReadOnlyCollection<IResource<T>> resources)
        {
            AssertArg.NotNull(resources, nameof(resources));
            AssertArg.NoNullItems(resources, nameof(resources));

            var orderedResources = resources.OrderBy(i => i.Age).ToArray();
            var examineScopeStartIndex = 0;
            var examineScopeLength = orderedResources.Length;

            foreach (var rule in _orderedRules)
            {
                if (examineScopeLength <= 0) break;

                var examineScopeResources = orderedResources
                    .Skip(examineScopeStartIndex)
                    .Take(examineScopeLength)
                    .ToArray();

                var outOfRuleCount = examineScopeResources
                    .TakeWhile(i => i.Age <= rule.OlderThan)
                    .Count();

                var retainCount = examineScopeResources
                    .Skip(outOfRuleCount)
                    .Take(rule.AllowedAmount)
                    .Count();

                examineScopeStartIndex += outOfRuleCount;
                examineScopeLength = retainCount;
            }

            var totalRetainCount =
                examineScopeStartIndex +
                examineScopeLength;

            return orderedResources
                .Skip(totalRetainCount)
                .Select(i => i.Id);
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