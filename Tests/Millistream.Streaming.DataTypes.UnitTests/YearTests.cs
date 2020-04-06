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
            Year year;
            Assert.IsTrue(Year.TryParse("2020", out year) && year == 2020 && year.ToString() == "2020");
            Assert.IsTrue(Year.TryParse("0001", out year) && year == 1 && year.ToString() == "0001");
            Assert.IsTrue(Year.TryParse("9999", out year) && year == 9999 && year.ToString() == "9999");
            Assert.IsFalse(Year.TryParse("0000", out year) && year == default);
            Assert.IsFalse(Year.TryParse("1", out year) && year == default);
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