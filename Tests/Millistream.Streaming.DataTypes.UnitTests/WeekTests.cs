using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class WeekTests
    {
        [TestMethod]
        public void ParseWeekTest()
        {
            ParseWeekTest(1, 1);
            ParseWeekTest(2019, 52);
            ParseWeekTest(0001, 2);
            ParseWeekTest(2000, 22);
            ParseWeekTest(2020, 45);
            ParseWeekTest(9999, 53);

            Week week;
            Assert.IsTrue(Week.TryParse("2020-W3".GetBytes(), out week));
            Assert.AreEqual(2020, week.Year);
            Assert.AreEqual(3, week.Number);
            Assert.AreEqual("2020-W03", week.ToString());

            Assert.IsFalse(Week.TryParse("0000-W01", out week));
            Assert.AreEqual(default, week);
            Assert.IsFalse(Week.TryParse("2020-W54".GetBytes(), out _));
            Assert.IsFalse(Week.TryParse("2020-W00".GetBytes(), out _));
            Assert.IsFalse(Week.TryParse("W05", out _));
            Assert.IsFalse(Week.TryParse("2012-04".GetBytes(), out _));
            Assert.IsFalse(Week.TryParse("2002".GetBytes(), out _));
            Assert.IsFalse(Week.TryParse("abc".GetBytes(), out _));
            Assert.IsFalse(Week.TryParse("@", out _));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseWeekWithInvalidCharsTest() => Week.Parse("()/");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseWeekWithInvalidBytesTest() => Week.Parse(".-/".GetBytes());

        [TestMethod]
        public void CreateWeekTest()
        {
            Week week = new Week();
            Assert.AreEqual(0, week.Year);
            Assert.AreEqual(0, week.Number);
            Assert.AreEqual("0000-W00", week.ToString());

            week = new Week(new Year(2020), 1);
            Assert.AreEqual(2020, week.Year);
            Assert.AreEqual(1, week.Number);
            Assert.AreEqual("2020-W01", week.ToString());

            week = new Week(new Year(2021), 40);
            Assert.AreEqual(2021, week.Year);
            Assert.AreEqual(40, week.Number);
            Assert.AreEqual("2021-W40", week.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateWeekWithInvalidArgumentTest() => new Week(new Year(2020), 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateWeekInvalidArgumentTest2() => new Week(new Year(2020), 54);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareWeekToObjectOfAnotherTypeTest() => Week.Parse("2019-W01").CompareTo(new DateTime(2019, 01, 01));

        [TestMethod]
        public void CompareWeeksTest()
        {
            Week week = Week.Parse("2019-W22");
            Assert.AreEqual(1, week.CompareTo(Week.Parse("2019-W20")));
            Assert.AreEqual(1, week.CompareTo(Week.Parse("2018-W22")));
            Assert.AreEqual(0, week.CompareTo(Week.Parse("2019-W22".GetBytes())));
            Assert.AreEqual(-1, week.CompareTo(Week.Parse("2019-W23")));
            Assert.AreEqual(-1, week.CompareTo(Week.Parse("2020-W01")));

            Assert.IsTrue(week > Week.Parse("2019-W20"));
            Assert.IsTrue(week >= Week.Parse("2019-W20"));
            Assert.IsTrue(week > Week.Parse("2018-W22"));
            Assert.IsTrue(week >= Week.Parse("2018-W22"));
            Assert.IsFalse(week < Week.Parse("2018-W22"));
            Assert.IsFalse(week <= Week.Parse("2018-W22"));
            Assert.IsTrue(week <= Week.Parse("2019-W23"));
            Assert.IsTrue(week <= Week.Parse("2019-W22"));

            Assert.IsTrue(Week.Parse("2019-W22").Equals(Week.Parse("2019-W22".GetBytes())));
            Assert.AreEqual(Week.Parse("2019-W11").GetHashCode(), Week.Parse("2019-W11".GetBytes()).GetHashCode());
            Assert.IsTrue(Week.Parse("2019-W02") == Week.Parse("2019-W02"));
            Assert.IsFalse(Week.Parse("2019-W02") != Week.Parse("2019-W02"));
            Assert.IsFalse(Week.Parse("2019-W02").Equals(Week.Parse("2019-W22")));
            Assert.AreNotEqual(Week.Parse("2019-W02").GetHashCode(), Week.Parse("2019-W01").GetHashCode());
            Assert.IsFalse(Week.Parse("2020-W01").Equals(Week.Parse("2019-W01")));
            Assert.IsTrue(Week.Parse("2020-W22") != Week.Parse("2019-W22"));
            Assert.IsFalse(Week.Parse("2020-W01").Equals(Week.Parse("2020-W02")));
            Assert.IsFalse(Week.Parse("2019-W01").Equals(new DateTime(2019, 1, 1)));
        }

        private static void ParseWeekTest(int year, int number)
        {
            const char PaddingChar = '0';
            string s = $"{year.ToString().PadLeft(4, PaddingChar)}-W{number.ToString().PadLeft(2, PaddingChar)}";

            Week week;
            Assert.IsTrue(Week.TryParse(s.GetBytes(), out week));
            Assert.AreEqual(year, week.Year);
            Assert.AreEqual(number, week.Number);
            Assert.AreEqual(s, week.ToString());
        }
    }
}