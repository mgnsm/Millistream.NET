using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class QuarterTests
    {
        [TestMethod]
        public void ParseMonthTest()
        {
            ParseQuarterTest(1, 1);
            ParseQuarterTest(2019, 1);
            ParseQuarterTest(0001, 1);
            ParseQuarterTest(2000, 2);
            ParseQuarterTest(2020, 4);
            ParseQuarterTest(9999, 4);

            Assert.IsFalse(Quarter.TryParse("0000-Q1", out Quarter quarter));
            Assert.AreEqual(default, quarter);
            Assert.IsFalse(Quarter.TryParse("2001-Q5".GetBytes(), out _));
            Assert.IsFalse(Quarter.TryParse("Q4", out _));
            Assert.IsFalse(Quarter.TryParse("2011-4".GetBytes(), out _));
            Assert.IsFalse(Quarter.TryParse("2001".GetBytes(), out _));
            Assert.IsFalse(Quarter.TryParse("xyz".GetBytes(), out _));
            Assert.IsFalse(Quarter.TryParse("@(-;", out _));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseQuarterWithInvalidCharsTest() => Quarter.Parse("2020-Q5");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseQuarterWithInvalidBytesTest() => Quarter.Parse("---//".GetBytes());

        [TestMethod]
        public void CreateQuarterTest()
        {
            Quarter quarter = new Quarter();
            Assert.AreEqual(0, quarter.Year);
            Assert.AreEqual(0, quarter.Number);
            Assert.AreEqual("0000-Q0", quarter.ToString());

            quarter = new Quarter(new Year(2020), 3);
            Assert.AreEqual(2020, quarter.Year);
            Assert.AreEqual(3, quarter.Number);
            Assert.AreEqual("2020-Q3", quarter.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateQuarterWithInvalidArgumentTest() => new Quarter(new Year(2026), 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateQuarterWithInvalidArgumentTest2() => new Quarter(new Year(2015), 5);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareQuarterToObjectOfAnotherTypeTest() => Quarter.Parse("2019-Q4").CompareTo(new DateTime(2019, 12, 01));

        [TestMethod]
        public void CompareQuartersTest()
        {
            Quarter quarter = Quarter.Parse("2019-Q3");
            Assert.AreEqual(1, quarter.CompareTo(Quarter.Parse("2019-Q2")));
            Assert.IsTrue(quarter > Quarter.Parse("2018-Q4"));
            Assert.IsTrue(quarter > Quarter.Parse("2019-Q2"));
            Assert.IsTrue(quarter >= Quarter.Parse("2019-Q2"));
            Assert.IsFalse(quarter <= Quarter.Parse("2019-Q2"));
            Assert.IsFalse(quarter < Quarter.Parse("2019-Q2"));
            Assert.IsTrue(quarter < Quarter.Parse("2019-Q4"));
            Assert.IsTrue(quarter <= Quarter.Parse("2019-Q4"));
            Assert.AreEqual(0, quarter.CompareTo(Quarter.Parse("2019-Q3".GetBytes())));
            Assert.AreEqual(-1, quarter.CompareTo(Quarter.Parse("2019-Q4")));
            Assert.IsTrue(quarter <= Quarter.Parse("2019-Q3"));
            Assert.IsFalse(quarter > Quarter.Parse("2019-Q3"));
            Assert.AreEqual(1, quarter.CompareTo(Quarter.Parse("2018-Q4")));
            Assert.AreEqual(1, quarter.CompareTo(null));

            Assert.IsTrue(Quarter.Parse("2019-Q4").Equals(Quarter.Parse("2019-Q4".GetBytes())));
            Assert.AreEqual(Quarter.Parse("2019-Q3").GetHashCode(), Quarter.Parse("2019-Q3".GetBytes()).GetHashCode());
            Assert.IsTrue(Quarter.Parse("2019-Q4") == Quarter.Parse("2019-Q4"));
            Assert.IsFalse(Quarter.Parse("2019-Q4") != Quarter.Parse("2019-Q4"));
            Assert.IsFalse(Quarter.Parse("2019-Q3").Equals(Quarter.Parse("2019-Q4")));
            Assert.AreNotEqual(Quarter.Parse("2019-Q3").GetHashCode(), Quarter.Parse("2019-Q4").GetHashCode());
            Assert.IsTrue(Quarter.Parse("2019-Q2") != Quarter.Parse("2019-Q3"));
            Assert.IsFalse(Quarter.Parse("2020-Q1").Equals(Quarter.Parse("2019-Q1")));
            Assert.IsTrue(Quarter.Parse("2020-Q2") != Quarter.Parse("2019-Q2"));
            Assert.IsFalse(Quarter.Parse("2020-Q1").Equals(Quarter.Parse("2020-Q2")));
            Assert.IsTrue(Quarter.Parse("2020-Q1") != Quarter.Parse("2020-Q4"));
            Assert.IsFalse(Quarter.Parse("2019-Q4").Equals(new DateTime(2019, 12, 1)));
        }

        private static void ParseQuarterTest(int year, int quarterNumber)
        {
            string s = $"{year.ToString().PadLeft(4, '0')}-Q{quarterNumber}";

            Assert.IsTrue(Quarter.TryParse(s.GetBytes(), out Quarter quarter));
            Assert.AreEqual(year, quarter.Year);
            Assert.AreEqual(quarterNumber, quarter.Number);
            Assert.AreEqual(s, quarter.ToString());
        }
    }
}