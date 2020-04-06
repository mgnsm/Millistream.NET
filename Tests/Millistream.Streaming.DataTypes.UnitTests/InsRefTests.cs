using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class InsRefTests
    {
        [TestMethod]
        public void TryParseInsRefTest()
        {
            InsRef insRef;
            Assert.IsTrue(InsRef.TryParse("1", out insRef));
            Assert.AreEqual(1ul, insRef);
            Assert.IsTrue(InsRef.TryParse("1".GetBytes(), out insRef));
            Assert.AreEqual(1ul, insRef);
            Assert.IsTrue(InsRef.TryParse("0", out insRef));
            Assert.AreEqual(0ul, insRef);
            Assert.IsTrue(InsRef.TryParse("0".GetBytes(), out insRef));
            Assert.AreEqual(0ul, insRef);
            Assert.IsTrue(InsRef.TryParse(ulong.MaxValue.ToString(), out insRef));
            Assert.AreEqual(ulong.MaxValue, insRef);
            Assert.IsTrue(InsRef.TryParse(ulong.MaxValue.ToString().GetBytes(), out insRef));
            Assert.AreEqual(ulong.MaxValue, insRef);
            Assert.IsFalse(InsRef.TryParse("-1", out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsFalse(InsRef.TryParse("-1".GetBytes(), out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsFalse(InsRef.TryParse(".", out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsFalse(InsRef.TryParse(".".GetBytes(), out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsFalse(InsRef.TryParse("abc", out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsFalse(InsRef.TryParse("abc".GetBytes(), out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsFalse(InsRef.TryParse("1.1", out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsFalse(InsRef.TryParse("1.1".GetBytes(), out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsTrue(InsRef.TryParse("10 ", out insRef));
            Assert.AreEqual(10ul, insRef);
            Assert.IsTrue(InsRef.TryParse("10 ".GetBytes(), out insRef));
            Assert.AreEqual(10ul, insRef);
            Assert.IsFalse(InsRef.TryParse("0 1", out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsFalse(InsRef.TryParse("0 1".GetBytes(), out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsFalse(InsRef.TryParse(".1", out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsFalse(InsRef.TryParse(".1".GetBytes(), out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsTrue(InsRef.TryParse(" 5", out insRef));
            Assert.AreEqual(5ul, insRef);
            Assert.IsTrue(InsRef.TryParse(" 5".GetBytes(), out insRef));
            Assert.AreEqual(5ul, insRef);
            Assert.IsFalse(InsRef.TryParse("184467440737095516150", out insRef));
            Assert.AreEqual(default, insRef);
            Assert.IsFalse(InsRef.TryParse("184467440737095516150".GetBytes(), out insRef));
            Assert.AreEqual(default, insRef);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInsRefWithInvalidCharsTest() => InsRef.Parse("1.3");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseInsRefWithInvalidBytesTest() => InsRef.Parse("-1".GetBytes());

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareInsRefToObjectOfAnotherTypeTest() => new InsRef(1).CompareTo(1ul);

        [TestMethod]
        public void CompareInsRefsTest()
        {
            InsRef insRef = new InsRef(10);
            Assert.AreEqual(1, insRef.CompareTo(null));
            Assert.AreEqual(0, insRef.CompareTo(InsRef.Parse("10".GetBytes())));
            Assert.AreEqual(-1, insRef.CompareTo(new InsRef(11)));
            Assert.AreEqual(1, insRef.CompareTo(new InsRef(9)));

            Assert.IsTrue(new InsRef(1).Equals(new InsRef(1)));
            Assert.IsTrue(new InsRef(1) == new InsRef(1));
            Assert.IsFalse(new InsRef(1).Equals(new InsRef(2)));
            Assert.IsFalse(new InsRef(1) == new InsRef(2));
            Assert.IsTrue(new InsRef(1) != new InsRef(2));
            Assert.IsTrue(new InsRef(10) == 10ul);
            Assert.IsTrue(20ul == InsRef.Parse("20".GetBytes()));
            Assert.IsFalse(new InsRef(100).Equals(100ul));
            Assert.IsFalse(new InsRef(100).Equals(100));
            Assert.AreEqual(new InsRef(50).GetHashCode(), InsRef.Parse("50".GetBytes()).GetHashCode());
            Assert.AreNotEqual(InsRef.Parse("1".GetBytes()).GetHashCode(), new InsRef(2).GetHashCode());

            Assert.IsTrue(new InsRef(1) > new InsRef(0));
            Assert.IsTrue(new InsRef(5) < 6ul);
            Assert.IsTrue(new InsRef(6) <= 6ul);
            Assert.IsTrue(new InsRef(6) >= new InsRef(4));
            Assert.IsTrue(new InsRef(6) >= new InsRef(6));
            Assert.IsFalse(new InsRef(6) >= new InsRef(7));
            Assert.IsFalse(new InsRef(10) > new InsRef(11));
            Assert.IsTrue(new InsRef(10) > 9ul);
        }

        [TestMethod]
        public void InsRefToStringTest()
        {
            InsRef insref = new InsRef(100);
            Assert.AreEqual(insref.ToString(), 100ul.ToString());
            Assert.AreEqual(insref.ToString("D2", CultureInfo.InvariantCulture), 100ul.ToString("D2", CultureInfo.InvariantCulture));
            CultureInfo cultureInfo = new CultureInfo("sv");
            Assert.AreEqual(insref.ToString("N0", cultureInfo), 100ul.ToString("N0", cultureInfo));
        }

        [TestMethod]
        public void InsRefToUInt64Test()
        {
            InsRef insref = new InsRef(100);
            ulong ul = insref;
            Assert.AreEqual(100ul, ul);

            ul = new InsRef(1000);
            Assert.AreEqual(1000ul, ul);
        }
    }
}