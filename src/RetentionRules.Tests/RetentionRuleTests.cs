using System;

using NUnit.Framework;

namespace RetentionService.RetentionRules.Tests
{
    /// <summary>
    /// Represents the set of tests of the <see cref="RetentionRule"/> class.
    /// </summary>
    public class RetentionRuleTests
    {
        [TestCase(2.2, 3)]
        [TestCase(0, 3)]
        [TestCase(6, 0)]
        [TestCase(0, 0)]
        [TestCase(1e6, int.MaxValue)]
        public void RetentionRule_ctor_be_successfully_constructed_with_valid_arguments(
            double olderThanDays,
            int allowedAmount)
        {
            var olderThan = TimeSpan.FromDays(olderThanDays);

            Assert.DoesNotThrow(() => new RetentionRule(olderThan, allowedAmount));
        }

        [TestCase(-1)]
        [TestCase((int)-1e6)]
        [TestCase(-int.MaxValue)]
        public void RetentionRule_ctor_should_throw_exception_if_allowed_amount_is_negative(
            int allowedAmount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new RetentionRule(TimeSpan.Zero, allowedAmount));
        }

        [TestCase(-1e-8)]
        [TestCase(-5D)]
        [TestCase(-1e6)]
        public void RetentionRule_ctor_should_throw_exception_if_olderThan_interval_is_negative(
            double olderThanDays)
        {
            var olderThan = TimeSpan.FromDays(olderThanDays);

            Assert.Throws<ArgumentOutOfRangeException>(() => new RetentionRule(olderThan, 0));
        }
    }
}
