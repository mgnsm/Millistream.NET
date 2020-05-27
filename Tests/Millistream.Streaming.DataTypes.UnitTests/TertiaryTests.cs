using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class TertiaryTests
    {
        [TestMethod]
        public void ParseTertiaryTest()
        {
            ParseTertiaryTest(1, 1);
            ParseTertiaryTest(2019, 1);
            ParseTertiaryTest(0001, 3);
            ParseTertiaryTest(2000, 2);
            ParseTertiaryTest(2020, 1);
            ParseTertiaryTest(9999, 3);

            Assert.IsFalse(Tertiary.TryParse("0000-T1", out Tertiary tertiary));
            Assert.AreEqual(default, tertiary);
            Assert.IsFalse(Tertiary.TryParse("2001-T4".GetBytes(), out _));
            Assert.IsFalse(Tertiary.TryParse("2001-T0".GetBytes(), out _));
            Assert.IsFalse(Tertiary.TryParse("2001-T5".GetBytes(), out _));
            Assert.IsFalse(Tertiary.TryParse("T2", out _));
            Assert.IsFalse(Tertiary.TryParse("2020-4".GetBytes(), out _));
            Assert.IsFalse(Tertiary.TryParse("2020".GetBytes(), out _));
            Assert.IsFalse(Tertiary.TryParse("zzz".GetBytes(), out _));
            Assert.IsFalse(Tertiary.TryParse("()-", out _));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseTertiaryWithInvalidCharsTest() => Tertiary.Parse("2020-T4");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseTertiaryWithInvalidBytesTest() => Tertiary.Parse("{".GetBytes());

        [TestMethod]
        public void CreateTertiaryTest()
        {
            Tertiary tertiary = new Tertiary();
            Assert.AreEqual(0, tertiary.Year);
            Assert.AreEqual(0, tertiary.Number);
            Assert.AreEqual("0000-T0", tertiary.ToString());

            tertiary = new Tertiary(new Year(2020), 3);
            Assert.AreEqual(2020, tertiary.Year);
            Assert.AreEqual(3, tertiary.Number);
            Assert.AreEqual("2020-T3", tertiary.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateTertiaryWithInvalidArgumentTest() => new Tertiary(new Year(2020), 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreateTertiaryInvalidArgumentTest2() => new Tertiary(new Year(2020), 4);

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareTertiaryToObjectOfAnotherTypeTest() => Tertiary.Parse("2019-T1").CompareTo(new DateTime(2019, 01, 01));

        [TestMethod]
        public void CompareTertiarysTest()
        {
            Tertiary tertiary = Tertiary.Parse("2019-T2");
            Assert.AreEqual(1, tertiary.CompareTo(Tertiary.Parse("2019-T1")));
            Assert.IsTrue(tertiary > Tertiary.Parse("2019-T1"));
            Assert.IsTrue(tertiary > Tertiary.Parse("2018-T2"));
            Assert.IsTrue(tertiary >= Tertiary.Parse("2019-T2"));
            Assert.IsTrue(tertiary <= Tertiary.Parse("2019-T3"));
            Assert.IsTrue(tertiary < Tertiary.Parse("2019-T3"));
            Assert.IsTrue(tertiary < Tertiary.Parse("2020-T2"));
            Assert.AreEqual(0, tertiary.CompareTo(Tertiary.Parse("2019-T2".GetBytes())));
            Assert.AreEqual(-1, tertiary.CompareTo(Tertiary.Parse("2019-T3")));
            Assert.AreEqual(1, tertiary.CompareTo(Tertiary.Parse("2019-T1")));
            Assert.AreEqual(1, tertiary.CompareTo(null));

            Assert.IsTrue(Tertiary.Parse("2020-T2").Equals(Tertiary.Parse("2020-T2".GetBytes())));
            Assert.AreEqual(Tertiary.Parse("2020-T2").GetHashCode(), Tertiary.Parse("2020-T2".GetBytes()).GetHashCode());
            Assert.IsTrue(Tertiary.Parse("2020-T2") == Tertiary.Parse("2020-T2"));
            Assert.IsFalse(Tertiary.Parse("2020-T2") != Tertiary.Parse("2020-T2"));
            Assert.IsFalse(Tertiary.Parse("2020-T2").Equals(Tertiary.Parse("2020-T1")));
            Assert.AreNotEqual(Tertiary.Parse("2020-T2").GetHashCode(), Tertiary.Parse("2020-T1").GetHashCode());
            Assert.IsTrue(Tertiary.Parse("2019-T1") != Tertiary.Parse("2019-T2"));
            Assert.IsFalse(Tertiary.Parse("2020-T1").Equals(Tertiary.Parse("2019-T1")));
            Assert.IsTrue(Tertiary.Parse("2020-T2") != Tertiary.Parse("2019-T2"));
            Assert.IsFalse(Tertiary.Parse("2019-T3").Equals(new DateTime(2019, 12, 1)));
        }

        private static void ParseTertiaryTest(int year, int number)
        {
            string s = $"{year.ToString().PadLeft(4, '0')}-T{number}";

            Assert.IsTrue(Tertiary.TryParse(s.GetBytes(), out Tertiary tertiary));
            Assert.AreEqual(year, tertiary.Year);
            Assert.AreEqual(number, tertiary.Number);
            Assert.AreEqual(s, tertiary.ToString());
        }
    }
}