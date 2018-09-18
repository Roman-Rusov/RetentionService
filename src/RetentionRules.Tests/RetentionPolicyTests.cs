using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using NUnit.Framework;

using RetentionService.Cleanup.Contracts;
using RetentionService.Tests.Common;

using static System.StringSplitOptions;

namespace RetentionService.RetentionRules.Tests
{
    /// <summary>
    /// Represents the set of tests of the <see cref="RetentionPolicy"/> class.
    /// </summary>
    public class RetentionPolicyTests
    {
        [Test]
        public void Ctor_should_throw_ArgumentNullException_if_rules_argument_is_null() =>
            AssertConstructorThrows<ArgumentNullException>(null);

        [Test]
        public void Ctor_should_throw_ArgumentException_if_rules_argument_is_an_empty_collection() =>
            AssertConstructorThrows<ArgumentException>(new RetentionRule[0]);

        [Test]
        public void Ctor_should_throw_ArgumentException_if_rules_argument_contains_null_element() =>
            AssertConstructorThrows<ArgumentException>(new RetentionRule[] { null });

        [Test]
        public void FindExpiredResources_should_throw_ArgumentNullException_if_resources_argument_is_null() =>
            AssertFindExpiredResourcesThrows<ArgumentNullException>(null);

        [Test]
        public void FindExpiredResources_should_throw_ArgumentException_if_resources_argument_contains_null_element() =>
            AssertFindExpiredResourcesThrows<ArgumentException>(new IResource<string>[] { null });

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
        public void FindExpiredResources_should_find_expired_resources_in_accordance_with_retention_rules(
            string rulesData,
            string resourceDetails,
            string expectedExpiredSrourcesIdsData)
        {
            // Arrange.
            var rules = rulesData.ParseRules();
            var policy = new RetentionPolicy(rules);
            var resources = resourceDetails.ParseResources();
            var expectedExpiredResourceIds = expectedExpiredSrourcesIdsData.Split(" ", RemoveEmptyEntries);

            // Act.
            var actualExpiredResourceIds = policy.FindExpiredResources(resources).ToArray();

            // Assert.
            actualExpiredResourceIds.Should().BeEquivalentTo(expectedExpiredResourceIds);
        }

        private static void AssertConstructorThrows<TException>(
            IReadOnlyCollection<RetentionRule> rules)
            where TException : Exception
        {
            // Act + Assert.
            Assert.Throws<TException>(() => new RetentionPolicy(rules));
        }

        private static void AssertFindExpiredResourcesThrows<TException>(
            IReadOnlyCollection<IResource<string>> resources)
            where TException : Exception
        {
            // Arrange.
            var validRules = "5:4 10:2".ParseRules();
            var policy = new RetentionPolicy(validRules);

            // Act + Assert.
            Assert.Throws<TException>(
                () => policy.FindExpiredResources(resources));
        }
    }
}
