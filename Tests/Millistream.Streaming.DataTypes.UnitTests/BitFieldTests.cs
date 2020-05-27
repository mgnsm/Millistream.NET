using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class BitFieldTests
    {
        [Flags]
        private enum DaysOfWeek
        {
            Monday = 1,
            Tuesday = 2,
            Wednesday = 4,
            Thursday = 8,
            Friday = 16,
            Saturday = 32,
            Sunday = 64,
        }

        [TestMethod]
        public void HasFlagBitFieldTest()
        {
            DaysOfWeek days = (DaysOfWeek.Monday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Friday);
            BitField bitField = BitField.Parse(((uint)days).ToString());
            Assert.IsTrue(bitField.HasFlag((uint)DaysOfWeek.Monday));
            Assert.IsTrue(bitField.HasFlag((uint)DaysOfWeek.Tuesday));
            Assert.IsTrue(bitField.HasFlag((uint)DaysOfWeek.Wednesday));
            Assert.IsFalse(bitField.HasFlag((uint)DaysOfWeek.Thursday));
            Assert.IsTrue(bitField.HasFlag((uint)DaysOfWeek.Friday));
            Assert.IsFalse(bitField.HasFlag((uint)DaysOfWeek.Saturday));
            Assert.IsFalse(bitField.HasFlag((uint)DaysOfWeek.Sunday));
        }

        [TestMethod]
        public void TryParseBitFieldTest()
        {
            Assert.IsTrue(BitField.TryParse("1", out BitField bitField));
            Assert.AreEqual(1ul, bitField);
            Assert.IsTrue(BitField.TryParse("1".GetBytes(), out bitField));
            Assert.AreEqual(1ul, bitField);
            Assert.IsTrue(BitField.TryParse("0", out bitField));
            Assert.AreEqual(0ul, bitField);
            Assert.IsTrue(BitField.TryParse("0".GetBytes(), out bitField));
            Assert.AreEqual(0ul, bitField);
            Assert.IsFalse(BitField.TryParse(ulong.MaxValue.ToString(), out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse(ulong.MaxValue.ToString().GetBytes(), out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsTrue(BitField.TryParse(uint.MaxValue.ToString(), out bitField));
            Assert.AreEqual(uint.MaxValue, bitField);
            Assert.IsTrue(BitField.TryParse(uint.MaxValue.ToString().GetBytes(), out bitField));
            Assert.AreEqual(uint.MaxValue, bitField);
            Assert.IsFalse(BitField.TryParse("-1", out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse("-1".GetBytes(), out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse(".", out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse(".".GetBytes(), out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse("abc", out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse("abc".GetBytes(), out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse("1.1", out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse("1.1".GetBytes(), out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsTrue(BitField.TryParse("10 ", out bitField));
            Assert.AreEqual(10ul, bitField);
            Assert.IsTrue(BitField.TryParse("10 ".GetBytes(), out bitField));
            Assert.AreEqual(10ul, bitField);
            Assert.IsFalse(BitField.TryParse("0 1", out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse("0 1".GetBytes(), out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse(".1", out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse(".1".GetBytes(), out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsTrue(BitField.TryParse(" 5", out bitField));
            Assert.AreEqual(5ul, bitField);
            Assert.IsTrue(BitField.TryParse(" 5".GetBytes(), out bitField));
            Assert.AreEqual(5ul, bitField);
            Assert.IsFalse(BitField.TryParse("184467440737095516150", out bitField));
            Assert.AreEqual(default, bitField);
            Assert.IsFalse(BitField.TryParse("184467440737095516150".GetBytes(), out bitField));
            Assert.AreEqual(default, bitField);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseBitFieldWithInvalidCharsTest() => BitField.Parse("2.4");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseBitFieldWithInvalidBytesTest() => BitField.Parse("-5".GetBytes());

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareBitFieldToObjectOfAnotherTypeTest() => new BitField(1).CompareTo(1ul);

        [TestMethod]
        public void CompareBitFieldsTest()
        {
            BitField BitField = new BitField(10);
            Assert.AreEqual(1, BitField.CompareTo(null));
            Assert.AreEqual(0, BitField.CompareTo(BitField.Parse("10".GetBytes())));
            Assert.AreEqual(-1, BitField.CompareTo(new BitField(11)));
            Assert.AreEqual(1, BitField.CompareTo(new BitField(9)));

            Assert.IsTrue(new BitField(1).Equals(new BitField(1)));
            Assert.IsTrue(new BitField(1) == new BitField(1));
            Assert.IsFalse(new BitField(1).Equals(new BitField(2)));
            Assert.IsFalse(new BitField(1) == new BitField(2));
            Assert.IsTrue(new BitField(1) != new BitField(2));
            Assert.IsTrue(new BitField(10) == 10ul);
            Assert.IsTrue(20ul == BitField.Parse("20".GetBytes()));
            Assert.IsFalse(new BitField(100).Equals(100ul));
            Assert.IsFalse(new BitField(100).Equals(100));
            Assert.AreEqual(new BitField(50).GetHashCode(), BitField.Parse("50".GetBytes()).GetHashCode());
            Assert.AreNotEqual(BitField.Parse("1".GetBytes()).GetHashCode(), new BitField(2).GetHashCode());

            Assert.IsTrue(new BitField(1) > new BitField(0));
            Assert.IsTrue(new BitField(5) < 6ul);
            Assert.IsTrue(new BitField(6) <= 6ul);
            Assert.IsTrue(new BitField(6) >= new BitField(4));
            Assert.IsTrue(new BitField(6) >= new BitField(6));
            Assert.IsFalse(new BitField(6) >= new BitField(7));
            Assert.IsFalse(new BitField(10) > new BitField(11));
            Assert.IsTrue(new BitField(10) > 9ul);
        }

        [TestMethod]
        public void BitFieldToStringTest()
        {
            BitField BitField = new BitField(100);
            Assert.AreEqual(BitField.ToString(), 100ul.ToString());
            Assert.AreEqual(BitField.ToString("D2", CultureInfo.InvariantCulture), 100ul.ToString("D2", CultureInfo.InvariantCulture));
            CultureInfo cultureInfo = new CultureInfo("sv");
            Assert.AreEqual(BitField.ToString("N0", cultureInfo), 100ul.ToString("N0", cultureInfo));
        }

        [TestMethod]
        public void BitFieldToBitField32Test()
        {
            BitField bitField = new BitField(100);
            uint ui = bitField;
            Assert.AreEqual(100u, ui);
            ulong l = bitField;
            Assert.AreEqual(100ul, l);
            ui = new BitField(1000);
            Assert.AreEqual(1000u, ui);
        }
    }
}