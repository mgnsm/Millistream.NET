using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Numerics;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class NumberTests
    {
        [TestMethod]
        public void ParseNumberTest()
        {
            ParseNumberTest("123.45");
            ParseNumberTest("-123.45");
            ParseNumberTest("10");
            ParseNumberTest("-20");
            ParseNumberTest("0.00000001");
            ParseNumberTest("0.001");
            ParseNumberTest("001");
            ParseNumberTest("005.012");
            ParseNumberTest("010101");
            ParseNumberTest("-10000.4");
            ParseNumberTest("9999999999999999999999999999999999999999.9999999999");
            ParseNumberTest("-9999999999999999999999999999999999999999.9999999999");
            ParseNumberTest("NULL");

            const string Utf8EncodedNumber = "190.09";
            Assert.IsTrue(Number.TryParse(Utf8EncodedNumber.GetBytes(), out Number number));
            Assert.AreEqual("190,09", number.ToString(new CultureInfo("sv")));
        }

        [TestMethod]
        public void NumberPrecisionAndScaleTest()
        {
            Number number1 = CreateNumber("123.456");
            Assert.IsFalse(number1.IsNegative);
            Assert.AreEqual(6, number1.GetPrecision());
            Assert.AreEqual(3, number1.Scale);

            Number number2 = CreateNumber("-1000");
            Assert.IsTrue(number2.IsNegative);
            Assert.AreEqual(4, number2.GetPrecision());
            Assert.AreEqual(0, number2.Scale);

            Number number3 = CreateNumber("0.001");
            Assert.IsFalse(number3.IsNegative);
            Assert.AreEqual(4, number3.GetPrecision());
            Assert.AreEqual(3, number3.Scale);

            Number number4 = new Number(new BigInteger(4005), 2);
            Assert.IsFalse(number4.IsNegative);
            Assert.AreEqual(4, number4.GetPrecision());
            Assert.AreEqual(2, number4.Scale);
            Assert.AreEqual("40.05", number4.ToString());

            Number number5 = new Number(new BigInteger(1010000007), 7);
            Assert.IsFalse(number5.IsNegative);
            Assert.AreEqual(10, number5.GetPrecision());
            Assert.AreEqual(7, number5.Scale);
            Assert.AreEqual("101.0000007", number5.ToString());

            Number number6 = new Number(new BigInteger(0), 0);
            Assert.IsFalse(number6.IsNegative);
            Assert.AreEqual(1, number6.GetPrecision());
            Assert.AreEqual(0, number6.Scale);
            Assert.AreEqual("0", number6.ToString());

            Number number7 = Number.Null;
            Assert.IsFalse(number7.IsNegative);
            Assert.AreEqual(0, number7.GetPrecision());
            Assert.AreEqual(0, number7.Scale);
            Assert.AreEqual("NULL", number7.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseNumberWithInvalidCharsTest() => Day.Parse("abc");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseNumberWithInvalidBytesTest() => Day.Parse("@,".GetBytes());

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareNumberToObjectOfAnotherTypeTest() => Number.Parse("10").CompareTo(new BigInteger(10));

        [TestMethod]
        public void CompareNumbersTest()
        {
            Assert.AreEqual(CreateNumber("123.456"), CreateNumber("123.456"));

            Number number = CreateNumber("-1000");
            object @object = CreateNumber("-1000");
            Assert.AreEqual(number, @object);

            Number number2 = CreateNumber("-1000");
            Assert.IsTrue(number.Equals(number2));
            Assert.AreEqual(number.GetHashCode(), number2.GetHashCode());

            Number number3 = CreateNumber("1000");
            Assert.IsFalse(number.Equals(number3));
            Assert.AreNotEqual(number.GetHashCode(), number3.GetHashCode());

            Assert.AreEqual(Number.Null, Number.Parse("null"));
            Assert.AreNotEqual(Number.Null, Number.Parse("0"));

            number = CreateNumber("1000");
            number2 = CreateNumber("2.5");
            Assert.AreEqual(1, number.CompareTo(null));
            Assert.AreEqual(1, number.CompareTo(Number.Null));
            Assert.AreEqual(-1, Number.Null.CompareTo(number));
            Assert.AreEqual(1, number.CompareTo(number2));
            Assert.AreEqual(-1, number2.CompareTo(number));
            number = CreateNumber("-1000");
            Assert.AreEqual(-1, number.CompareTo(number2));
            Assert.AreEqual(1, number2.CompareTo(number));

            Assert.IsTrue(CreateNumber("0.1") > CreateNumber("0.002"));
            Assert.IsTrue(CreateNumber("11.1") > CreateNumber("10"));
            Assert.IsTrue(CreateNumber("100") > CreateNumber("99"));
            Assert.IsTrue(CreateNumber("1.9999999999999") < CreateNumber("2"));
            Assert.IsTrue(CreateNumber("5.25000") == CreateNumber("5.25"));
            Assert.IsFalse(CreateNumber("5.25000") != CreateNumber("5.25"));
            Assert.IsTrue(CreateNumber("0") > Number.Null);
            Assert.IsTrue(CreateNumber("0") >= Number.Null);
            Assert.IsTrue(Number.Null < CreateNumber("-100"));

            Assert.IsTrue(CreateNumber("000000.1").Equals(CreateNumber("0.1")));
            Assert.IsFalse(CreateNumber("10").Equals(new BigInteger(10)));
            Assert.IsTrue(CreateNumber("000000.1") == CreateNumber("0.1"));
            Assert.IsFalse(CreateNumber("000000.1").Equals(CreateNumber("0.11")));
            Assert.IsTrue(Number.Null == Number.Parse("nUlL"));
            Assert.IsFalse(Number.Parse("0") == Number.Null);

            Assert.IsTrue(CreateNumber("0.001") < CreateNumber("0.01"));
            Assert.IsTrue(CreateNumber("0.001") <= CreateNumber("0.01"));
            Assert.IsFalse(CreateNumber("0.001") > CreateNumber("0.01"));
            Assert.IsFalse(CreateNumber("0.001") >= CreateNumber("0.01"));
        }

        [TestMethod]
        public void ArithmeticalNumberTest()
        {
            Assert.AreEqual(CreateNumber("5.5") + CreateNumber("6.67"), CreateNumber("12.17"));
            Assert.AreEqual(CreateNumber("5.5") + CreateNumber("6.67"), Number.Add(CreateNumber("5.5"), CreateNumber("6.67")));
            Assert.AreEqual(CreateNumber("3") + CreateNumber("2.123456789"), CreateNumber("5.123456789"));
            Assert.AreEqual(CreateNumber("1000000000000000") + CreateNumber("1.999999999"), CreateNumber("1000000000000001.999999999"));
            Assert.AreEqual(CreateNumber("1") + CreateNumber("1"), CreateNumber("2"));
            Assert.AreEqual(CreateNumber("1") + Number.Null, Number.Null);
            Assert.AreEqual(Number.Null + CreateNumber("10"), Number.Null);

            Assert.AreEqual(CreateNumber("5.67") - CreateNumber("6.67"), CreateNumber("-1"));
            Assert.AreEqual(CreateNumber("5.67") - CreateNumber("6.67"), Number.Subtract(CreateNumber("5.67"), CreateNumber("6.67")));
            Assert.AreEqual(CreateNumber("5.67") - CreateNumber("6.67"), CreateNumber("-1.000000000"));
            Assert.AreEqual(CreateNumber("33") - CreateNumber("2.123456789"), CreateNumber("30.876543211"));
            Assert.AreEqual(CreateNumber("100000") - CreateNumber("1.999999999"), CreateNumber("99998.000000001"));
            Assert.AreEqual(CreateNumber("1") - CreateNumber("1"), CreateNumber("0.0"));
            Assert.AreEqual(CreateNumber("1") - Number.Null, Number.Null);
            Assert.AreEqual(Number.Null - CreateNumber("10"), Number.Null);

            Assert.AreEqual(-CreateNumber("1"), CreateNumber("-1"));
            Assert.AreEqual(+CreateNumber("123.456"), CreateNumber("123.456"));
            Assert.AreEqual(-Number.Null, Number.Null);
            Assert.AreEqual(+Number.Null, Number.Null);

            Number number = CreateNumber("5.123");
            Assert.AreEqual(++number, CreateNumber("6.123"));
            number++;
            Assert.AreEqual(number, CreateNumber("7.123"));
            Assert.AreEqual(--number, CreateNumber("6.123"));
            number = CreateNumber("8");
            Assert.AreEqual(++number, CreateNumber("9.0"));
            Assert.AreEqual(--number, CreateNumber("8"));
            number--;
            Assert.AreEqual(number, CreateNumber("7"));
            number = Number.Null;
            Assert.AreEqual(++number, Number.Null);
            Assert.AreEqual(number++, Number.Null);
            Assert.AreEqual(--number, Number.Null);
            Assert.AreEqual(number--, Number.Null);

            Assert.AreEqual(CreateNumber("1.25") * CreateNumber("3"), CreateNumber("3.75"));
            Assert.AreEqual(CreateNumber("1.25") * CreateNumber("3"), Number.Multiply(CreateNumber("1.25"), CreateNumber("3")));
            Assert.AreEqual(CreateNumber("-10") * CreateNumber("-10"), CreateNumber("100"));
            Assert.AreEqual(CreateNumber("-10.5") * CreateNumber("40.123"), CreateNumber("-421.2915"));
            Assert.AreEqual(CreateNumber("-10.5") * Number.Null, Number.Null);
            Assert.AreEqual(Number.Null * Number.Null, Number.Null);

            Assert.AreEqual(CreateNumber("0.539") / CreateNumber("0.11"), CreateNumber("4.9"));
            Assert.AreEqual(CreateNumber("0.539") / CreateNumber("0.11"), Number.Divide(CreateNumber("0.539"), CreateNumber("0.11")));
            Assert.AreEqual(CreateNumber("9.1") / CreateNumber("7"), CreateNumber("1.3"));
            Assert.AreEqual(CreateNumber("6.4") / CreateNumber("0.4"), CreateNumber("16"));
            Assert.AreEqual(CreateNumber("15") / CreateNumber("0.2"), CreateNumber("75"));
            Assert.AreEqual(CreateNumber("15") / Number.Null, Number.Null);
            Assert.AreEqual(Number.Null / CreateNumber("3"), Number.Null);
            number = CreateNumber("1") / CreateNumber("3");
            Assert.AreEqual(number, CreateNumber("0.3333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333"));
            Assert.AreEqual(100, number.Scale);
            number = Number.Divide(CreateNumber("1"), CreateNumber("3"), 10);
            Assert.AreEqual(number, CreateNumber("0.3333333333"));
            Assert.AreEqual(10, number.Scale);
            Assert.AreEqual(CreateNumber("100000") / CreateNumber("-10"), CreateNumber("-10000"));
            Assert.AreEqual(Number.Divide(CreateNumber("-754.5"), CreateNumber("34.7"), 10), CreateNumber("-21.7435158501"));

            Assert.AreEqual(Number.Abs(CreateNumber("-123.456")), CreateNumber("123.456"));
            Assert.AreEqual(Number.Abs(CreateNumber("123")), CreateNumber("123"));
            Assert.AreEqual(Number.Abs(CreateNumber("-1")), CreateNumber("1"));
            Assert.AreEqual(Number.Abs(CreateNumber("123.4")), CreateNumber("123.4"));
            Assert.AreEqual(Number.Abs(Number.Null), Number.Null);
        }

        private static void ParseNumberTest(string s)
        {
            Number number;
            Assert.IsTrue(Number.TryParse(s, out number));
            Assert.AreEqual(s, number.ToString());
            Assert.IsTrue(Number.TryParse(s.GetBytes(), out number));
            Assert.AreEqual(s, number.ToString());
        }

        private static Number CreateNumber(string s) => Number.Parse(s.GetBytes());
    }
}