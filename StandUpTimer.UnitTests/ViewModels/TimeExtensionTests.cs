using System;
using NUnit.Framework;
using StandUpTimer.ViewModels;

namespace StandUpTimer.UnitTests.ViewModels
{
    [TestFixture]
    public class TimeExtensionTests
    {
        [TestCase(-1, "")]
        [TestCase(0, "")]
        [TestCase(1, "0\nsec")]
        [TestCase(1000, "1\nsec")]
        [TestCase(59000, "59\nsec")]
        [TestCase(60000, "1\nmin")]
        [TestCase(3600000, "60\nmin")]
        [TestCase(7200000, "120\nmin")]
        public void Format_remaining_time_tests(int milliseconds, string result)
        {
            Assert.That(TimeSpan.FromMilliseconds(milliseconds).FormatRemainingTime(), Is.EqualTo(result));
        }

        [TestCase(-1, 1, 0)]
        [TestCase(0, 1, 0)]
        [TestCase(10, 100, 10)]
        [TestCase(1, 1, 100)]
        [TestCase(2, 1, 200)]
        [TestCase(1, 0, double.PositiveInfinity)]
        public void Percentage_tests(int fractionMilliseconds, int baseMilliseconds, double result)
        {
            Assert.That(TimeSpan.FromMilliseconds(fractionMilliseconds).PercentageTo(TimeSpan.FromMilliseconds(baseMilliseconds)), Is.EqualTo(result));
        }

        [TestCase(-1, 1, 0)]
        [TestCase(0, 1, 0)]
        [TestCase(10, 100, 10)]
        [TestCase(1, 1, 100)]
        [TestCase(2, 1, 200)]
        [TestCase(2, 0, double.PositiveInfinity)]
        public void Fraction_tests(int fractionMilliseconds, int baseMilliseconds, double result)
        {
            Assert.That(TimeSpan.FromMilliseconds(fractionMilliseconds).PercentageTo(TimeSpan.FromMilliseconds(baseMilliseconds)), Is.EqualTo(result));
        }
    }
}