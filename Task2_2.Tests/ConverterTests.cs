using System;
using NUnit.Framework;

namespace Task2_2.Tests
{
    [TestFixture]
    public class ConverterTests
    {
        [Test]
        [TestCase("2147483647", 2147483647)]
        [TestCase("0", 0)]
        public void ToInt32_PositiveInt_ExpectedConvertedInt(string input, int expectedOutput)
        {
            var actualOutput = input.ToInt32();

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [TestCase("+2147483647", 2147483647)]
        [TestCase("+0", 0)]
        public void ToInt32_PositiveIntWithPlus_ExpectedConvertedInt(string input, int expectedOutput)
        {
            var actualOutput = input.ToInt32();

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [TestCase("-2147483648", -2147483648)]
        [TestCase("-0", 0)]
        public void ToInt32_NegativeInt_ExpectedConvertedInt(string input, int expectedOutput)
        {
            var actualOutput = input.ToInt32();

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [TestCase("   123", 123)]
        [TestCase("   123   ", 123)]
        [TestCase("123   ", 123)]
        public void ToInt32_IntWithSpaces_ExpectedConvertedInt(string input, int expectedOutput)
        {
            var actualOutput = input.ToInt32();

            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [TestCase(" ")]
        [TestCase(" +")]
        [TestCase(" -")]
        [TestCase("")]
        [TestCase("+")]
        [TestCase("-")]
        [TestCase(null)]
        public void ToInt32_EmptyString_ExpectedArgumentException(string input)
        {
            Assert.Throws<ArgumentException>(() => input.ToInt32());
        }

        [Test]
        [TestCase("1a")]
        [TestCase("-1a")]
        [TestCase("+1a")]
        public void ToInt32_DigitsWithLetters_ExpectedFormatException(string input)
        {
            Assert.Throws<FormatException>(() => input.ToInt32());
        }

        [Test]
        [TestCase("1111111111111")]
        [TestCase("-1111111111111")]
        public void ToInt32_LargeInteger_ExpectedOverflowException(string input)
        {
            Assert.Throws<OverflowException>(() => input.ToInt32());
        }
    }
}
