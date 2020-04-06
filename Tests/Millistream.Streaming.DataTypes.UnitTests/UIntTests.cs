using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class UIntTests
    {
        [TestMethod]
        public void TryParseUIntTest()
        {
            UInt @uint;
            Assert.IsTrue(UInt.TryParse("1", out @uint));
            Assert.AreEqual(1ul, @uint);
            Assert.IsTrue(UInt.TryParse("1".GetBytes(), out @uint));
            Assert.AreEqual(1ul, @uint);
            Assert.IsTrue(UInt.TryParse("0", out @uint));
            Assert.AreEqual(0ul, @uint);
            Assert.IsTrue(UInt.TryParse("0".GetBytes(), out @uint));
            Assert.AreEqual(0ul, @uint);
            Assert.IsFalse(UInt.TryParse(ulong.MaxValue.ToString(), out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse(ulong.MaxValue.ToString().GetBytes(), out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsTrue(UInt.TryParse(uint.MaxValue.ToString(), out @uint));
            Assert.AreEqual(uint.MaxValue, @uint);
            Assert.IsTrue(UInt.TryParse(uint.MaxValue.ToString().GetBytes(), out @uint));
            Assert.AreEqual(uint.MaxValue, @uint);
            Assert.IsFalse(UInt.TryParse("-1", out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse("-1".GetBytes(), out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse(".", out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse(".".GetBytes(), out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse("abc", out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse("abc".GetBytes(), out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse("1.1", out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse("1.1".GetBytes(), out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsTrue(UInt.TryParse("10 ", out @uint));
            Assert.AreEqual(10ul, @uint);
            Assert.IsTrue(UInt.TryParse("10 ".GetBytes(), out @uint));
            Assert.AreEqual(10ul, @uint);
            Assert.IsFalse(UInt.TryParse("0 1", out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse("0 1".GetBytes(), out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse(".1", out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse(".1".GetBytes(), out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsTrue(UInt.TryParse(" 5", out @uint));
            Assert.AreEqual(5ul, @uint);
            Assert.IsTrue(UInt.TryParse(" 5".GetBytes(), out @uint));
            Assert.AreEqual(5ul, @uint);
            Assert.IsFalse(UInt.TryParse("184467440737095516150", out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse("184467440737095516150".GetBytes(), out @uint));
            Assert.AreEqual(default, @uint);
            Assert.IsFalse(UInt.TryParse("42949672951".GetBytes(), out @uint));
            Assert.AreEqual(default, @uint);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseUIntWithInvalidCharsTest() => UInt.Parse("1.3");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseUIntWithInvalidBytesTest() => UInt.Parse("-1".GetBytes());

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareUIntToObjectOfAnotherTypeTest() => new UInt(1).CompareTo(1ul);

        [TestMethod]
        public void CompareUIntsTest()
        {
            UInt UInt = new UInt(10);
            Assert.AreEqual(1, UInt.CompareTo(null));
            Assert.AreEqual(0, UInt.CompareTo(UInt.Parse("10".GetBytes())));
            Assert.AreEqual(-1, UInt.CompareTo(new UInt(11)));
            Assert.AreEqual(1, UInt.CompareTo(new UInt(9)));

            Assert.IsTrue(new UInt(1).Equals(new UInt(1)));
            Assert.IsTrue(new UInt(1) == new UInt(1));
            Assert.IsFalse(new UInt(1).Equals(new UInt(2)));
            Assert.IsFalse(new UInt(1) == new UInt(2));
            Assert.IsTrue(new UInt(1) != new UInt(2));
            Assert.IsTrue(new UInt(10) == 10ul);
            Assert.IsTrue(20ul == UInt.Parse("20".GetBytes()));
            Assert.IsFalse(new UInt(100).Equals(100ul));
            Assert.IsFalse(new UInt(100).Equals(100));
            Assert.AreEqual(new UInt(50).GetHashCode(), UInt.Parse("50".GetBytes()).GetHashCode());
            Assert.AreNotEqual(UInt.Parse("1".GetBytes()).GetHashCode(), new UInt(2).GetHashCode());

            Assert.IsTrue(new UInt(1) > new UInt(0));
            Assert.IsTrue(new UInt(5) < 6ul);
            Assert.IsTrue(new UInt(6) <= 6ul);
            Assert.IsTrue(new UInt(6) >= new UInt(4));
            Assert.IsTrue(new UInt(6) >= new UInt(6));
            Assert.IsFalse(new UInt(6) >= new UInt(7));
            Assert.IsFalse(new UInt(10) > new UInt(11));
            Assert.IsTrue(new UInt(10) > 9ul);
        }

        [TestMethod]
        public void UIntToStringTest()
        {
            UInt UInt = new UInt(100);
            Assert.AreEqual(UInt.ToString(), 100ul.ToString());
            Assert.AreEqual(UInt.ToString("D2", CultureInfo.InvariantCulture), 100ul.ToString("D2", CultureInfo.InvariantCulture));
            CultureInfo cultureInfo = new CultureInfo("sv");
            Assert.AreEqual(UInt.ToString("N0", cultureInfo), 100ul.ToString("N0", cultureInfo));
        }

        [TestMethod]
        public void UIntToUInt32Test()
        {
            UInt UInt = new UInt(100);
            uint ui = UInt;
            Assert.AreEqual(100u, ui);
            ulong l = UInt;
            Assert.AreEqual(100ul, l);
            ui = new UInt(1000);
            Assert.AreEqual(1000u, ui);
        }
    }
}