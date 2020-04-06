using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class DayTests
    {
        [TestMethod]
        public void ParseDayTest()
        {
            ParseDayTest(2019, 11, 12);
            ParseDayTest(0001, 1, 1);
            ParseDayTest(2000, 2, 29);

            Day day;
            Assert.IsFalse(Day.TryParse("0000-12-12", out day));
            Assert.AreEqual(default, day);
            Assert.IsFalse(Day.TryParse("2001-02-29".GetBytes(), out _));
            Assert.IsFalse(Day.TryParse("29", out _));
            Assert.IsFalse(Day.TryParse("2011-02".GetBytes(), out _));
            Assert.IsFalse(Day.TryParse("2001".GetBytes(), out _));
            Assert.IsFalse(Day.TryParse("abc".GetBytes(), out _));
            Assert.IsFalse(Day.TryParse("/*-;", out _));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseDayWithInvalidCharsTest() => Day.Parse("12");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseDayWithInvalidBytesTest() => Day.Parse("@".GetBytes());

        [TestMethod]
        public void CreateDayTest()
        {
            Day day = new Day();
            Assert.AreEqual(0, day.Year);
            Assert.AreEqual(0, day.Month.Number);
            Assert.AreEqual(0, day.Number);
            Assert.AreEqual("0000-00-00", day.ToString());

            day = new Day(new Month(new Year(2019), 6), 17);
            Assert.AreEqual(2019, day.Year);
            Assert.AreEqual(6, day.Month.Number);
            Assert.AreEqual(17, day.Number);
            Assert.AreEqual("2019-06-17", day.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateDayWithInvalidDayNumberTest() => new Day(new Month(new Year(2001), 2), 29);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateDayWithInvalidDayNumberTest2() => new Day(new Month(new Year(2005), 6), 31);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareDayToObjectOfAnotherTypeTest() => Day.Parse("2019-12-12").CompareTo(new DateTime(2019, 12, 12));

        [TestMethod]
        public void CompareDaysTest()
        {
            Day day = Day.Parse("2019-11-12");
            Assert.AreEqual(1, day.CompareTo(Day.Parse("2019-11-11")));
            Assert.IsTrue(day > Day.Parse("2019-11-11"));
            Assert.IsTrue(day >= Day.Parse("2019-11-11"));
            Assert.IsFalse(day < Day.Parse("2019-11-11"));
            Assert.IsFalse(day <= Day.Parse("2019-11-11"));
            Assert.AreEqual(0, day.CompareTo(Day.Parse("2019-11-12".GetBytes())));
            Assert.AreEqual(-1, day.CompareTo(Day.Parse("2019-12-12")));
            Assert.IsTrue(day < Day.Parse("2019-12-12"));
            Assert.IsTrue(day <= Day.Parse("2019-12-12"));
            Assert.IsFalse(day > Day.Parse("2019-12-12"));
            Assert.AreEqual(1, day.CompareTo(Day.Parse("2018-11-12")));
            Assert.AreEqual(1, day.CompareTo(null));

            Assert.IsTrue(Day.Parse("2019-11-12").Equals(Day.Parse("2019-11-12")));
            Assert.AreEqual(Day.Parse("2019-11-12").GetHashCode(), Day.Parse("2019-11-12").GetHashCode());
            Assert.IsTrue(Day.Parse("2019-11-12") == Day.Parse("2019-11-12"));
            Assert.IsFalse(Day.Parse("2019-11-12") != Day.Parse("2019-11-12"));
            Assert.IsFalse(Day.Parse("2019-11-12").Equals(Day.Parse("2019-11-11")));
            Assert.AreNotEqual(Day.Parse("2019-11-12").GetHashCode(), Day.Parse("2019-11-11").GetHashCode());
            Assert.IsTrue(Day.Parse("2019-11-12") != Day.Parse("2019-11-11"));
            Assert.IsFalse(Day.Parse("2020-11-12").Equals(Day.Parse("2019-11-12")));
            Assert.IsTrue(Day.Parse("2020-11-12") != Day.Parse("2019-11-12"));
            Assert.IsFalse(Day.Parse("2020-11-12").Equals(Day.Parse("2020-10-12")));
            Assert.IsTrue(Day.Parse("2020-11-12") != Day.Parse("2020-10-12"));
            Assert.IsFalse(Day.Parse("2019-11-12").Equals(new DateTime(2019, 11, 12)));
        }

        private static void ParseDayTest(int year, int month, int dayNumber)
        {
            const char PaddingChar = '0';
            string s = $"{year.ToString().PadLeft(4, PaddingChar)}-{month.ToString().PadLeft(2, PaddingChar)}-{dayNumber.ToString().PadLeft(2, PaddingChar)}";
            
            Day day;
            Assert.IsTrue(Day.TryParse(s.GetBytes(), out day));
            Assert.AreEqual(year, day.Year);
            Assert.AreEqual(month, day.Month.Number);
            Assert.AreEqual(dayNumber, day.Number);
            Assert.AreEqual(s, day.ToString());
        }
    }
}