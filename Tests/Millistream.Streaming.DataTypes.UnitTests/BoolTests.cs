using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class BoolTests
    {
        [TestMethod]
        public void TryParseBoolTest()
        {
            Bool @bool;
            Assert.IsTrue(Bool.TryParse("1", out @bool));
            Assert.IsTrue(@bool);
            Assert.IsTrue(Bool.TryParse("1".GetBytes(), out @bool));
            Assert.IsTrue(@bool);

            Assert.IsTrue(Bool.TryParse("0", out @bool));
            Assert.IsFalse(@bool);
            Assert.IsTrue(Bool.TryParse("0", out @bool));
            Assert.IsFalse(@bool);

            Assert.IsFalse(Bool.TryParse("2", out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("2".GetBytes(), out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("a", out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("a".GetBytes(), out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("/", out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("/".GetBytes(), out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse(".", out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse(".".GetBytes(), out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("true", out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("true".GetBytes(), out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("True", out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("True".GetBytes(), out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("false", out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("false".GetBytes(), out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("False", out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("False".GetBytes(), out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("11", out @bool));
            Assert.AreEqual(default, @bool);
            Assert.IsFalse(Bool.TryParse("11", out @bool));
            Assert.AreEqual(default, @bool);
        }

        [TestMethod]
        public void ParseBoolTest()
        {
            Bool @bool = Bool.Parse("1".GetBytes());
            Assert.IsTrue(@bool);

            @bool = Bool.Parse("0".GetBytes());
            Assert.IsFalse(@bool);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseBoolWithInvalidCharsTest() => Bool.Parse("true");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseBoolWithInvalidBytesTest() => Bool.Parse("false".GetBytes());

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareBoolToObjectOfAnotherTypeTest()
        {
            Bool @bool = Bool.Parse("0".GetBytes());
            @bool.CompareTo(1);
        }

        [TestMethod]
        public void CompareBoolsTest()
        {
            Bool @bool = Bool.Parse("1".GetBytes());
            Assert.AreEqual(1, @bool.CompareTo(null));
            Assert.AreEqual(0, @bool.CompareTo(new Bool(true)));
            Assert.AreEqual(1, @bool.CompareTo(new Bool(false)));
            Assert.AreEqual(-1, new Bool(false).CompareTo(@bool));

            Assert.IsTrue(new Bool(true).Equals(Bool.Parse("1".GetBytes())));
            Assert.IsFalse(new Bool(true).Equals(Bool.Parse("0".GetBytes())));
            Assert.IsTrue(new Bool(false).Equals(new Bool(false)));
            Assert.AreEqual(new Bool(true).GetHashCode(), new Bool(true).GetHashCode());
            Assert.AreNotEqual(new Bool(true).GetHashCode(), new Bool(false).GetHashCode());
            Assert.IsFalse(new Bool(false).Equals(false));
            Assert.IsFalse(new Bool(true).Equals(false));
            Assert.IsFalse(new Bool(true).Equals(true));

            Assert.IsTrue(new Bool(true) == new Bool(true));
            Assert.IsTrue(new Bool(true) != new Bool(false));
            Assert.IsFalse(new Bool(true) == new Bool(false));
            Assert.IsTrue(new Bool(true) != new Bool(false));
            Assert.IsFalse(new Bool(true) == new Bool(false));
        }

        [TestMethod]
        public void BoolToStringTest()
        {
            Bool @bool = new Bool(true);
            Assert.AreEqual(true.ToString(), @bool.ToString());
            @bool = new Bool(false);
            Assert.AreEqual(false.ToString(), @bool.ToString());
        }

        [TestMethod]
        public void BoolToBooleanTest()
        {
            Bool @bool = new Bool(true);
            bool b = @bool;
            Assert.IsTrue(b);

            @bool = (Bool)false;
            Assert.IsFalse(@bool);
        }
    }
}