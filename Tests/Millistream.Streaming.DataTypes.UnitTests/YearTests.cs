using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class YearTests
    {
        [TestMethod]
        public void ParseYearTest()
        {
            static void ParseValidYearTest(string s, int value)
            {
                Assert.IsTrue(Year.TryParse(s, out Year year) && year == value && year.ToString() == s);
                Assert.IsTrue(Year.TryParse(s.GetBytes(), out year) && year == value && year.ToString() == s);
            }

            static void ParseInvalidYearTest(string s )
            {
                Assert.IsFalse(Year.TryParse(s, out _));
                Assert.IsFalse(Year.TryParse(s.GetBytes(), out _));
            }
            ParseValidYearTest("2020", 2020);
            ParseValidYearTest("0001", 1);
            ParseValidYearTest("9999", 9999);

            ParseInvalidYearTest("0000");
            ParseInvalidYearTest("1");
            ParseInvalidYearTest("   1");
            ParseInvalidYearTest("1  1");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseYearWithInvalidCharsTest() => Year.Parse("@7");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseYearWithInvalidBytesTest() => Year.Parse("0".GetBytes());

        [TestMethod]
        public void CreateYearTest()
        {
            Year year = new Year();
            Assert.AreEqual(0, year);
            Assert.AreEqual("0000", year.ToString());

            year = new Year(2020);
            Assert.AreEqual(2020, year);
            Assert.AreEqual("2020", year.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateYearWithInvalidArgumentTest() => new Year(0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateYearInvalidArgumentTest2() => new Year(10000);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareYearToObjectOfAnotherTypeTest() => Year.Parse("2000").CompareTo(2000);

        [TestMethod]
        public void CompareYearsTest()
        {
            Year year = Year.Parse("2019");
            Assert.AreEqual(1, year.CompareTo(Year.Parse("2018")));
            Assert.AreEqual(1, year.CompareTo(Year.Parse("2000")));
            Assert.AreEqual(0, year.CompareTo(Year.Parse("2019".GetBytes())));
            Assert.AreEqual(-1, year.CompareTo(Year.Parse("2020")));
            Assert.AreEqual(-1, year.CompareTo(Year.Parse("9999")));

            Assert.IsTrue(year > Year.Parse("2018"));
            Assert.IsTrue(year >= Year.Parse("2019"));
            Assert.IsTrue(year >= Year.Parse("2017"));
            Assert.IsTrue(year < Year.Parse("2020"));
            Assert.IsTrue(year <= Year.Parse("2020"));
            Assert.IsTrue(year <= Year.Parse("2019"));
            Assert.IsTrue(year < Year.Parse("2020"));

            Assert.IsTrue(Year.Parse("2020").Equals(Year.Parse("2020".GetBytes())));
            Assert.AreEqual(Year.Parse("2020").GetHashCode(), Year.Parse("2020".GetBytes()).GetHashCode());
            Assert.IsTrue(Year.Parse("2020") == Year.Parse("2020".GetBytes()));
            Assert.IsFalse(Year.Parse("2020") != Year.Parse("2020".GetBytes()));
            Assert.IsTrue(Year.Parse("2020") != Year.Parse("2022"));
        }
    }
}