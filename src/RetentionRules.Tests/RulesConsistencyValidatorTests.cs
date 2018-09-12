using System;

using NUnit.Framework;

using RetentionService.Tests.Common;

namespace RetentionService.RetentionRules.Tests
{
    /// <summary>
    /// Represents the set of tests of the <see cref="RulesConsistencyValidator"/> class.
    /// </summary>
    public class RulesConsistencyValidatorTests
    {
        [Test]
        public void Validate_should_throw_ArgumentNullException_if_rules_argument_is_null() =>
            Assert.Throws<ArgumentNullException>(
                () => CreateValidator().Validate(null));

        [Test]
        public void Validate_should_throw_ArgumentException_if_rules_argument_is_an_empty_collection() =>
            Assert.Throws<ArgumentException>(
                () => CreateValidator().Validate(new RetentionRule[0]));

        [Test]
        public void Validate_should_throw_ArgumentException_if_rules_argument_contains_null_element() =>
            Assert.Throws<ArgumentException>(
                () => CreateValidator().Validate(new RetentionRule[] { null }));

        [Test(Description =
            "Constructor should throw ArgumentException if allowed limit of older items " +
            "is greater than allowed limit of younger ones.")]
        [TestCase("1:1 5:2")]
        [TestCase("1:10 2:9 3:10")]
        [TestCase("1:1 14:1 21:2")]
        public void Validate_should_throw_ArgumentException_if_rules_argument_contains_contradictive_rules(
            string rulesData) =>
            AssertValidateThrows<ArgumentException>(rulesData);

        [TestCase("5:12 5:4")]
        [TestCase("5:10 5:5 5:0")]
        [TestCase("1:10 5:5 10:3 10:1 11:2")]
        public void Validate_should_throw_ArgumentException_if_rules_argument_contains_duplicate_rules(
            string rulesData) =>
            AssertValidateThrows<ArgumentException>(rulesData);

        private static void AssertValidateThrows<TException>(string rulesData)
            where TException : Exception
        {
            // Arrange.
            var rules = rulesData.ParseRules();
            var validator = CreateValidator();

            // Act + Assert.
            Assert.Throws<TException>(() => validator.Validate(rules));
        }

        private static RulesConsistencyValidator CreateValidator() =>
            new RulesConsistencyValidator();
    }
}
