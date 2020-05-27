using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class MonthTests
    {
        [TestMethod]
        public void ParseMonthTest()
        {
            ParseMonthTest(2019, 11);
            ParseMonthTest(0001, 1);
            ParseMonthTest(2000, 2);
            ParseMonthTest(2020, 12);
            ParseMonthTest(9999, 12);

            Assert.IsFalse(Month.TryParse("0000-12", out Month month));
            Assert.AreEqual(default, month);
            Assert.IsFalse(Month.TryParse("2001-13".GetBytes(), out _));
            Assert.IsFalse(Month.TryParse("29", out _));
            Assert.IsFalse(Month.TryParse("2011-02-01".GetBytes(), out _));
            Assert.IsFalse(Month.TryParse("2001".GetBytes(), out _));
            Assert.IsFalse(Month.TryParse("abc".GetBytes(), out _));
            Assert.IsFalse(Month.TryParse("/*-;", out _));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseMonthWithInvalidCharsTest() => Month.Parse("1");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseMonthWithInvalidBytesTest() => Month.Parse(",.".GetBytes());

        [TestMethod]
        public void CreateMonthTest()
        {
            Month month = new Month();
            Assert.AreEqual(0, month.Year);
            Assert.AreEqual(0, month.Number);
            Assert.AreEqual("0000-00", month.ToString());

            month = new Month(new Year(2020), 9);
            Assert.AreEqual(2020, month.Year);
            Assert.AreEqual(9, month.Number);
            Assert.AreEqual("2020-09", month.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateMonthWithInvalidDayNumberTest() => new Month(new Year(2001), 13);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateMonthWithInvalidDayNumberTest2() => new Month(new Year(2005), 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareMonthToObjectOfAnotherTypeTest() => Month.Parse("2019-12").CompareTo(new DateTime(2019, 12, 01));

        [TestMethod]
        public void CompareMonthsTest()
        {
            Month month = Month.Parse("2019-03");
            Assert.AreEqual(1, month.CompareTo(Month.Parse("2019-02")));
            Assert.IsTrue(month > Month.Parse("2018-03"));
            Assert.IsTrue(month > Month.Parse("2019-02"));
            Assert.IsTrue(month >= Month.Parse("2019-02"));
            Assert.IsFalse(month <= Month.Parse("2019-02"));
            Assert.IsFalse(month < Month.Parse("2019-02"));
            Assert.IsTrue(month < Month.Parse("2019-11"));
            Assert.IsTrue(month <= Month.Parse("2019-11"));
            Assert.AreEqual(0, month.CompareTo(Month.Parse("2019-03".GetBytes())));
            Assert.AreEqual(-1, month.CompareTo(Month.Parse("2019-12")));
            Assert.IsTrue(month < Month.Parse("2019-12"));
            Assert.IsTrue(month <= Month.Parse("2019-03"));
            Assert.IsFalse(month > Month.Parse("2019-03"));
            Assert.AreEqual(1, month.CompareTo(Month.Parse("2018-11")));
            Assert.AreEqual(1, month.CompareTo(null));

            Assert.IsTrue(Month.Parse("2019-11").Equals(Month.Parse("2019-11")));
            Assert.AreEqual(Month.Parse("2019-11").GetHashCode(), Month.Parse("2019-11").GetHashCode());
            Assert.IsTrue(Month.Parse("2019-11") == Month.Parse("2019-11"));
            Assert.IsFalse(Month.Parse("2019-11") != Month.Parse("2019-11"));
            Assert.IsFalse(Month.Parse("2019-10").Equals(Month.Parse("2019-11")));
            Assert.AreNotEqual(Month.Parse("2019-11").GetHashCode(), Month.Parse("2019-10").GetHashCode());
            Assert.IsTrue(Month.Parse("2019-10") != Month.Parse("2019-08"));
            Assert.IsFalse(Month.Parse("2020-11").Equals(Month.Parse("2019-11")));
            Assert.IsTrue(Month.Parse("2020-11") != Month.Parse("2019-11"));
            Assert.IsFalse(Month.Parse("2020-11").Equals(Month.Parse("2020-10")));
            Assert.IsTrue(Month.Parse("2020-11") != Month.Parse("2020-10"));
            Assert.IsFalse(Month.Parse("2019-11").Equals(new DateTime(2019, 11, 1)));
        }

        private static void ParseMonthTest(int year, int monthNumber)
        {
            const char PaddingChar = '0';
            string s = $"{year.ToString().PadLeft(4, PaddingChar)}-{monthNumber.ToString().PadLeft(2, PaddingChar)}";

            Assert.IsTrue(Month.TryParse(s.GetBytes(), out Month month));
            Assert.AreEqual(year, month.Year);
            Assert.AreEqual(monthNumber, month.Number);
            Assert.AreEqual(s, month.ToString());
        }
    }
}