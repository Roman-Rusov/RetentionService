using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using NUnit.Framework;

using RetentionService.Tests.Common;

using static System.StringSplitOptions;

namespace RetentionService.RetentionRules.Tests
{
    /// <summary>
    /// Represents the set of tests of the <see cref="RetentionRuleSet"/> class.
    /// </summary>
    public class RetentionRuleSetTests
    {
        [Test]
        public void Ctor_should_throw_ArgumentNullException_on_null_rules_collection() =>
            AssertConstructorThrows<ArgumentNullException>(null);

        [Test]
        public void Ctor_should_throw_ArgumentException_on_empty_rules_collection() =>
            AssertConstructorThrows<ArgumentException>(new RetentionRule[0]);

        [Test]
        public void Ctor_should_throw_ArgumentException_on_null_rule_in_collection() =>
            AssertConstructorThrows<ArgumentException>(new RetentionRule[] { null });

        [TestCase(
            "5:1",
            "4.9 5.1",
            "",
            Description = "Retention rule shouldn't take an item into account if its age is out of scope of the rule.")]
        [TestCase(
            "5:4 10:2",
            "6 7 8 9 11 12",
            "11 12",
            Description = "Retention rule regarding older items should take into account all the rules regarding younger items.")]
        [TestCase(
            "5:4 10:2",
            "",
            "",
            Description = "There should be no items to remove from empty item sequence.")]
        [TestCase(
            "2:5 5:3 7:3 10:1 20:0",
            "1.9 2.5 3.3 7.1 22.4",
            "22.4")]
        [TestCase(
            "10:0",
            "10",
            "",
            Description =
                "A rule spans upon items that are older than the rule's date. " +
                "Items having age equal to the rule's date are not in the scope of the rule.")]
        public void FindStaleItems_should_find_stale_items_in_accordance_with_retention_rules(
            string rulesData,
            string resourceDetails,
            string expectedStaleItemsData)
        {
            // Arrange.
            var rules = rulesData.ParseRules();
            var ruleSet = new RetentionRuleSet(rules);
            var resources = resourceDetails.ParseResourceDetails();
            var items = resources.Select(r => (r.Address, r.Age));
            var expectedStaleItems = expectedStaleItemsData.Split(new []{' '}, RemoveEmptyEntries);

            // Act.
            var actualStaelItems = ruleSet.FindStaleItems(items).ToArray();

            // Assert.
            actualStaelItems.Should().BeEquivalentTo(expectedStaleItems);
        }

        private static void AssertConstructorThrows<TException>(
            IReadOnlyCollection<RetentionRule> rules)
            where TException : Exception
        {
            // Act + Assert.
            Assert.Throws<TException>(() => new RetentionRuleSet(rules));
        }
    }
}
