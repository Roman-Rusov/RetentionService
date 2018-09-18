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
        public void Ctor_should_be_successfully_completed_with_valid_arguments(
            double olderThanDays,
            int allowedAmount)
        {
            var olderThan = TimeSpan.FromDays(olderThanDays);

            Assert.DoesNotThrow(() => new RetentionRule(olderThan, allowedAmount));
        }

        [TestCase(-1)]
        [TestCase((int)-1e6)]
        [TestCase(-int.MaxValue)]
        public void Ctor_should_throw_ArgumentOutOfRangeException_if_allowedAmount_argument_is_negative(
            int allowedAmount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new RetentionRule(TimeSpan.Zero, allowedAmount));
        }

        [TestCase(-1e-8)]
        [TestCase(-5D)]
        [TestCase(-1e6)]
        public void Ctor_should_throw_ArgumentOutOfRangeException_if_olderThan_argument_is_negative(
            double olderThanDays)
        {
            var olderThan = TimeSpan.FromDays(olderThanDays);

            Assert.Throws<ArgumentOutOfRangeException>(() => new RetentionRule(olderThan, 0));
        }
    }
}
