using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class SemiAnnualTests
    {
        [TestMethod]
        public void ParseMonthTest()
        {
            ParseSemiAnnualTest(1, 1);
            ParseSemiAnnualTest(2019, 1);
            ParseSemiAnnualTest(0001, 1);
            ParseSemiAnnualTest(2000, 2);
            ParseSemiAnnualTest(2020, 1);
            ParseSemiAnnualTest(9999, 2);

            Assert.IsFalse(SemiAnnual.TryParse("0000-H1", out SemiAnnual semiAnnual));
            Assert.AreEqual(default, semiAnnual);
            Assert.IsFalse(SemiAnnual.TryParse("2001-H3".GetBytes(), out _));
            Assert.IsFalse(SemiAnnual.TryParse("2001-H0".GetBytes(), out _));
            Assert.IsFalse(SemiAnnual.TryParse("2001-H5".GetBytes(), out _));
            Assert.IsFalse(SemiAnnual.TryParse("H2", out _));
            Assert.IsFalse(SemiAnnual.TryParse("2012-4".GetBytes(), out _));
            Assert.IsFalse(SemiAnnual.TryParse("2002".GetBytes(), out _));
            Assert.IsFalse(SemiAnnual.TryParse("xyz".GetBytes(), out _));
            Assert.IsFalse(SemiAnnual.TryParse(".,---,;", out _));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSemiAnnualWithInvalidCharsTest() => SemiAnnual.Parse("2020-H3");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseSemiAnnualWithInvalidBytesTest() => SemiAnnual.Parse(".-/".GetBytes());

        [TestMethod]
        public void CreateSemiAnnualTest()
        {
            SemiAnnual semiAnnual = new SemiAnnual();
            Assert.AreEqual(0, semiAnnual.Year);
            Assert.AreEqual(0, semiAnnual.Number);
            Assert.AreEqual("0000-H0", semiAnnual.ToString());

            semiAnnual = new SemiAnnual(new Year(2020), 2);
            Assert.AreEqual(2020, semiAnnual.Year);
            Assert.AreEqual(2, semiAnnual.Number);
            Assert.AreEqual("2020-H2", semiAnnual.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateSemiAnnualWithInvalidArgumentTest() => new SemiAnnual(new Year(2020), 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateSemiAnnualInvalidArgumentTest2() => new SemiAnnual(new Year(2020), 3);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareSemiAnnualToObjectOfAnotherTypeTest() => SemiAnnual.Parse("2019-H1").CompareTo(new DateTime(2019, 01, 01));

        [TestMethod]
        public void CompareSemiAnnualsTest()
        {
            SemiAnnual semiAnnual = SemiAnnual.Parse("2019-H2");
            Assert.AreEqual(1, semiAnnual.CompareTo(SemiAnnual.Parse("2019-H1")));
            Assert.IsTrue(semiAnnual > SemiAnnual.Parse("2019-H1"));
            Assert.IsTrue(semiAnnual > SemiAnnual.Parse("2018-H2"));
            Assert.IsTrue(semiAnnual >= SemiAnnual.Parse("2019-H2"));
            Assert.IsFalse(semiAnnual <= SemiAnnual.Parse("2019-H1"));
            Assert.IsFalse(semiAnnual < SemiAnnual.Parse("2019-H1"));
            Assert.IsTrue(semiAnnual < SemiAnnual.Parse("2020-H2"));
            Assert.AreEqual(0, semiAnnual.CompareTo(SemiAnnual.Parse("2019-H2".GetBytes())));
            Assert.AreEqual(-1, semiAnnual.CompareTo(SemiAnnual.Parse("2020-H1")));
            Assert.AreEqual(1, semiAnnual.CompareTo(SemiAnnual.Parse("2018-H2")));
            Assert.AreEqual(1, semiAnnual.CompareTo(null));

            Assert.IsTrue(SemiAnnual.Parse("2019-H2").Equals(SemiAnnual.Parse("2019-H2".GetBytes())));
            Assert.AreEqual(SemiAnnual.Parse("2019-H1").GetHashCode(), SemiAnnual.Parse("2019-H1".GetBytes()).GetHashCode());
            Assert.IsTrue(SemiAnnual.Parse("2019-H2") == SemiAnnual.Parse("2019-H2"));
            Assert.IsFalse(SemiAnnual.Parse("2019-H2") != SemiAnnual.Parse("2019-H2"));
            Assert.IsFalse(SemiAnnual.Parse("2019-H1").Equals(SemiAnnual.Parse("2019-H2")));
            Assert.AreNotEqual(SemiAnnual.Parse("2019-H1").GetHashCode(), SemiAnnual.Parse("2019-H2").GetHashCode());
            Assert.IsTrue(SemiAnnual.Parse("2019-H1") != SemiAnnual.Parse("2019-H2"));
            Assert.IsFalse(SemiAnnual.Parse("2020-H1").Equals(SemiAnnual.Parse("2019-H1")));
            Assert.IsTrue(SemiAnnual.Parse("2020-H2") != SemiAnnual.Parse("2019-H2"));
            Assert.IsFalse(SemiAnnual.Parse("2020-H1").Equals(SemiAnnual.Parse("2020-H2")));
            Assert.IsFalse(SemiAnnual.Parse("2019-H2").Equals(new DateTime(2019, 12, 1)));
        }

        private static void ParseSemiAnnualTest(int year, int half)
        {
            string s = $"{year.ToString().PadLeft(4, '0')}-H{half}";

            Assert.IsTrue(SemiAnnual.TryParse(s.GetBytes(), out SemiAnnual semiAnnual));
            Assert.AreEqual(year, semiAnnual.Year);
            Assert.AreEqual(half, semiAnnual.Number);
            Assert.AreEqual(s, semiAnnual.ToString());
        }
    }
}