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
        public void CreateNumberTest()
        {
            static void Test(Number number, int expectedPrecision, int expectedScale, bool shouldBeNegative = false)
            {
                Assert.AreEqual(expectedPrecision, number.GetPrecision());
                Assert.AreEqual(expectedScale, number.Scale);
                Assert.AreEqual(shouldBeNegative, number.IsNegative);
            }

            Test(new Number(0, 0), 1, 0);
            //10^1..10^18
            long value = 1;
            int expectedPrecision = 1;
            for (int i = 0; i < 18; i++)
            {
                value *= 10;

                Test(new Number(value, 0), ++expectedPrecision, 0); //10..100..1000...1000000000000000000
                Test(new Number(value - 1, 0), expectedPrecision - 1, 0);//9..999..999...999999999999999999
                Test(new Number(value + 1, 0), expectedPrecision, 0); //11..101..1001...1000000000000000001

                //negative values
                Test(new Number(-value, 0), expectedPrecision, 0, true);
                Test(new Number(-value + 1, 0), expectedPrecision - 1, 0, true);
                Test(new Number(-value - 1, 0), expectedPrecision, 0, true);
            }
            Test(new Number(9999999999999999999 + 1, 0), 20, 0);
            Test(new Number(ulong.MaxValue, 0), 20, 0);
            Test(new Number(long.MinValue, 0), 19, 0, true);

            //number(20,x)...number(38,x)
            for (int i = 20; i < 39; i++)
            {
                BigInteger bigInteger = BigInteger.Parse(new string('9', i));
                Test(new Number(bigInteger, 0), i, 0);
                Test(new Number(bigInteger - 1, 0), i, 0);
                Test(new Number(bigInteger + 1, 0), i + 1, 0);

                Test(new Number(-bigInteger, 0), i, 0, true);
                Test(new Number(-bigInteger - 1, 0), i + 1, 0, true);
                Test(new Number(-bigInteger + 1, 0), i, 0, true);
            }

            Test(new Number(1, 10), 11, 10, false);
        }

        [TestMethod]
        public void GetNullNumberTest()
        {
            Number number = Number.Null;
            Assert.AreEqual(0, number.GetPrecision());
            Assert.AreEqual(0, number.Scale);
            Assert.IsFalse(number.IsNegative);
            Assert.AreEqual("NULL", number.ToString());
        }

        [TestMethod]
        public void GetAbsOfNumberTest()
        {
            Assert.AreEqual(Number.Abs(new Number(-123456, 3)), new Number(123456, 3));
            Assert.AreEqual(Number.Abs(new Number(123, 0)), new Number(123, 0));
            Assert.AreEqual(Number.Abs(new Number(-1, 0)), new Number(1, 0));
            Assert.AreEqual(Number.Abs(new Number(1234, 1)), new Number(1234, 1));
            Assert.AreEqual(Number.Abs(Number.Null), Number.Null);
        }

        [TestMethod]
        public void ParseNumberTest()
        {
            static void ParseNumberTest(string s, int expectedPrecision, int expectedScale, bool shouldBeNegative)
            {
                void Test(Number number)
                {
                    Assert.AreEqual(expectedPrecision, number.GetPrecision());
                    Assert.AreEqual(expectedScale, number.Scale);
                    Assert.AreEqual(shouldBeNegative, number.IsNegative);
                }

                Assert.IsTrue(Number.TryParse(s, out Number number));
                Test(number);
                Test(Number.Parse(s));

                ReadOnlySpan<byte> bytes = s.GetBytes();
                Assert.IsTrue(Number.TryParse(bytes, out number));
                Test(number);
                Test(Number.Parse(bytes));
            }

            ParseNumberTest("123.45", 5, 2, false);
            ParseNumberTest("-123.45", 5, 2, true);
            ParseNumberTest("123.4500", 5, 2, false);
            ParseNumberTest("10", 2, 0, false);
            ParseNumberTest("-20", 2, 0, true);
            ParseNumberTest("00", 1, 0, false);
            ParseNumberTest("0", 1, 0, false);
            ParseNumberTest("-0", 1, 0, false);
            ParseNumberTest("-000", 1, 0, false);
            ParseNumberTest("0.00000001", 9, 8, false);
            ParseNumberTest("0.001", 4, 3, false);
            ParseNumberTest("001", 1, 0, false);
            ParseNumberTest("005.012", 4, 3, false);
            ParseNumberTest("010101", 5, 0, false);
            ParseNumberTest("-10000.4", 6, 1, true);
            ParseNumberTest("NULL", 0, 0, false);
            ParseNumberTest("153.10000", 4, 1, false);
            ParseNumberTest("-153.10000", 4, 1, true);
            ParseNumberTest("10.00", 2, 0, false);
            ParseNumberTest("9999999999999999999", 19, 0, false);
            ParseNumberTest("999.9999999999999999", 19, 16, false);
            ParseNumberTest("9.999999999999999999", 19, 18, false);
            ParseNumberTest("123456789123456789.1", 19, 1, false);
            ParseNumberTest("999999999999999999.9", 19, 1, false);
            ParseNumberTest("-9999999999999999999", 19, 0, true);
            ParseNumberTest("-999.9999999999999999", 19, 16, true);
            ParseNumberTest("-9.999999999999999999", 19, 18, true);
            ParseNumberTest("-999999999999999999.9", 19, 1, true);
            ParseNumberTest("18446744073709551615", 20, 0, false);
            ParseNumberTest("1844674407370955161.5", 20, 1, false);
            ParseNumberTest("1.8446744073709551615", 20, 19, false);
            ParseNumberTest("1844674407.3709551615", 20, 10, false);
            ParseNumberTest("-18446744073709551615", 20, 0, true);
            ParseNumberTest("-1844674407370955161.5", 20, 1, true);
            ParseNumberTest("-1.8446744073709551615", 20, 19, true);
            ParseNumberTest("-1844674407.3709551615", 20, 10, true);
            ParseNumberTest("18446744073709551614", 20, 0, false);
            ParseNumberTest("-18446744073709551614", 20, 0, true);
            ParseNumberTest("-18446744073709551616", 20, 0, true);
            ParseNumberTest("184467440737095516.16", 20, 2, false);
            ParseNumberTest("9999999999999999999999999999999999999999.9999999999", 50, 10, false);
            ParseNumberTest("-9999999999999999999999999999999999999999.9999999999", 50, 10, true);
            ParseNumberTest("0010.4400", 4, 2, false);
            ParseNumberTest("-0010.9900", 4, 2, true);
            ParseNumberTest("0.", 1, 0, false);
            ParseNumberTest("10.", 2, 0, false);
            ParseNumberTest(".123", 4, 3, false);
            ParseNumberTest(".0", 1, 0, false);
            ParseNumberTest(".1", 2, 1, false);
            ParseNumberTest("-99999999999999999999999999999999999999999999999999.", 50, 0, true);
            ParseNumberTest("99999999999999999999999999999999999999999999999999.", 50, 0, false);
            ParseNumberTest("1000000", 7, 0, false);
            ParseNumberTest("000000", 1, 0, false);
            ParseNumberTest("1.000000", 1, 0, false);
            ParseNumberTest("-1.000000", 1, 0, true);
            ParseNumberTest("-1.000001", 7, 6, true);
            ParseNumberTest("-1.", 1, 0, true);
            ParseNumberTest("999999999999999.999999999999999", 30, 15, false);

            Assert.IsFalse(Number.TryParse(".", out _));
            Assert.IsFalse(Number.TryParse(".".GetBytes(), out _));
            Assert.IsFalse(Number.TryParse(".99999999999999999999999999999999999999999999999999.", out _));
            Assert.IsFalse(Number.TryParse(".99999999999999999999999999999999999999999999999999.".GetBytes(), out _));
            Assert.IsFalse(Number.TryParse("-.", out _));
            Assert.IsFalse(Number.TryParse("-.".GetBytes(), out _));
            Assert.IsFalse(Number.TryParse("-.1", out _));
            Assert.IsFalse(Number.TryParse("-.1".GetBytes(), out _));
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
            Assert.AreEqual(new Number(123456, 3), new Number(123456, 3));

            Number number = new Number(-1000, 0);
            object @object = new Number(-1000, 0);
            Assert.AreEqual(number, @object);

            Number number2 = new Number(-1000, 0);
            Assert.IsTrue(number.Equals(number2));
            Assert.AreEqual(number.GetHashCode(), number2.GetHashCode());

            Number number3 = new Number(1000, 0);
            Assert.IsFalse(number.Equals(number3));
            Assert.AreNotEqual(number.GetHashCode(), number3.GetHashCode());

            Assert.AreEqual(Number.Null, Number.Parse("null".GetBytes()));
            Assert.AreNotEqual(Number.Null, new Number(0, 0));

            number = new Number(1000, 0);
            number2 = new Number(25, 1);
            Assert.AreEqual(1, number.CompareTo(null));
            Assert.AreEqual(1, number.CompareTo(Number.Null));
            Assert.AreEqual(-1, Number.Null.CompareTo(number));
            Assert.AreEqual(1, number.CompareTo(number2));
            Assert.AreEqual(-1, number2.CompareTo(number));
            number = new Number(-1000, 0);
            Assert.AreEqual(-1, number.CompareTo(number2));
            Assert.AreEqual(1, number2.CompareTo(number));

            Assert.IsTrue(new Number(1, 1) > new Number(2, 3));
            Assert.IsTrue(new Number(111, 1) > new Number(10, 0));
            Assert.IsTrue(new Number(100, 0) > new Number(99, 0));
            Assert.IsTrue(new Number(BigInteger.Parse("19999999999999"), 13) < new Number(2, 0));
            Assert.IsTrue(Number.Parse("5.25000".GetBytes()) == new Number(525, 2));
            Assert.IsFalse(Number.Parse("5.25000".GetBytes()) != new Number(525, 2));
            Assert.IsTrue(new Number(0, 0) > Number.Null);
            Assert.IsTrue(new Number(0, 0) >= Number.Null);
            Assert.IsTrue(Number.Null < new Number(-100, 0));

            Assert.IsTrue(Number.Parse("000000.1".GetBytes()).Equals(new Number(1, 1)));
            Assert.IsFalse(new Number(10, 0).Equals(new BigInteger(10)));
            Assert.IsTrue(Number.Parse("000000.1".GetBytes()) == new Number(1, 1));
            Assert.IsFalse(Number.Parse("000000.1".GetBytes()).Equals(new Number(11, 2)));
            Assert.IsTrue(Number.Null == Number.Parse("nUlL".GetBytes()));
            Assert.IsFalse(Number.Parse("0".GetBytes()) == Number.Null);

            Assert.IsTrue(new Number(1, 3) < new Number(1, 2));
            Assert.IsTrue(new Number(1, 3) <= new Number(1, 2));
            Assert.IsFalse(new Number(1, 3) > new Number(1, 2));
            Assert.IsFalse(new Number(1, 3) >= new Number(1, 2));
        }

        [TestMethod]
        public void AddNumbersTest()
        {
            Assert.AreEqual(new Number(55, 1) + new Number(667, 2), new Number(1217, 2));
            Assert.AreEqual(new Number(55, 0) - new Number(667, 0), new Number(-612, 0));
            Assert.AreEqual(new Number(55, 1) + new Number(667, 2), Number.Add(new Number(55, 1), new Number(667, 2)));
            Assert.AreEqual(new Number(3, 0) + new Number(2123456789, 9), new Number(5123456789, 9));
            Assert.AreEqual(new Number(1000000000000000, 0) + new Number(1999999999, 9), new Number(BigInteger.Parse("1000000000000001999999999"), 9));
            Assert.AreEqual(new Number(1, 0) + new Number(1, 0), new Number(2, 0));
            Assert.AreEqual(new Number(1, 0) + Number.Null, Number.Null);
            Assert.AreEqual(Number.Null + new Number(10, 0), Number.Null);

            Assert.AreEqual(+new Number(123456, 3), new Number(123456, 3));
            Assert.AreEqual(+Number.Null, Number.Null);

            Number number = new Number(5123, 3);
            Assert.AreEqual(++number, new Number(6123, 3));
            number++;
            Assert.AreEqual(number, new Number(7123, 3));
            number = new Number(-1, 0);
            number++;
            Assert.AreEqual(new Number(0, 0), number);
            number = new Number(8, 0);
            Assert.AreEqual(++number, new Number(9, 0));
            number = Number.Null;
            Assert.AreEqual(++number, Number.Null);
            Assert.AreEqual(number++, Number.Null);

            Number negative = new Number(-501, 1);
            Assert.AreEqual(new Number(199, 1), negative + new Number(70, 0));
        }

        [TestMethod]
        public void DivideNumbersTest()
        {
            Assert.AreEqual(new Number(539, 3) / new Number(11, 2), new Number(49, 1));
            Assert.AreEqual(new Number(539, 3) / new Number(11, 2), Number.Divide(new Number(539, 3), new Number(11, 2)));
            Assert.AreEqual(new Number(91, 1) / new Number(7, 0), new Number(13, 1));
            Assert.AreEqual(new Number(64, 1) / new Number(4, 1), new Number(16, 0));
            Assert.AreEqual(new Number(15, 0) / new Number(2, 1), new Number(75, 0));
            Assert.AreEqual(new Number(15, 0) / Number.Null, Number.Null);
            Assert.AreEqual(Number.Null / new Number(3, 0), Number.Null);
            Number number = new Number(1, 0) / new Number(3, 0);
            Assert.AreEqual(100, number.Scale);
            Assert.AreEqual(number, new Number(BigInteger.Parse("3333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333333"), 100));
            number = Number.Divide(new Number(1, 0), new Number(3, 0), 10);
            Assert.AreEqual(number, new Number(3333333333, 10));
            Assert.AreEqual(10, number.Scale);
            Assert.AreEqual(new Number(100000, 0) / new Number(-10, 0), new Number(-10000, 0));
            Assert.AreEqual(Number.Divide(new Number(-7545, 1), new Number(347, 1), 10), new Number(-217435158501, 10));
        }

        [TestMethod]
        public void MultiplyNumbersTest()
        {
            Assert.AreEqual(new Number(125, 2) * new Number(3, 0), new Number(375, 2));
            Assert.AreEqual(new Number(125, 2) * new Number(3, 0), Number.Multiply(new Number(125, 2), new Number(3, 0)));
            Assert.AreEqual(new Number(-10, 0) * new Number(-10, 0), new Number(100, 0));
            Assert.AreEqual(new Number(-105, 1) * new Number(40123, 3), new Number(-4212915, 4));
            Assert.AreEqual(new Number(-105, 1) * Number.Null, Number.Null);
            Assert.AreEqual(Number.Null * Number.Null, Number.Null);
        }

        [TestMethod]
        public void SubtractNumbersTest()
        {
            Assert.AreEqual(new Number(567, 2) - new Number(667, 2), new Number(-1, 0));
            Assert.AreEqual(new Number(567, 2) - new Number(667, 2), Number.Subtract(new Number(567, 2), new Number(667, 2)));
            Assert.AreEqual(new Number(567, 2) - new Number(667, 2), new Number(-1000000000, 9));
            Assert.AreEqual(new Number(33, 0) - new Number(2123456789, 9), new Number(30876543211, 9));
            Assert.AreEqual(new Number(100000, 0) - new Number(1999999999, 9), new Number(99998000000001, 9));
            Assert.AreEqual(new Number(1, 0) - new Number(1, 0), new Number(0, 0));
            Assert.AreEqual(new Number(1, 0) - Number.Null, Number.Null);
            Assert.AreEqual(Number.Null - new Number(10, 0), Number.Null);

            Assert.AreEqual(-new Number(1, 0), new Number(-1, 0));
            Assert.AreEqual(-Number.Null, Number.Null);

            Number number = new Number(7123, 3);
            Assert.AreEqual(--number, new Number(6123, 3));
            number--;
            Assert.AreEqual(number, new Number(5123, 3));
            number = Number.Null;
            Assert.AreEqual(--number, Number.Null);
            Assert.AreEqual(number--, Number.Null);

            Number positive = new Number(501, 1);
            Assert.AreEqual(new Number(-199, 1), positive - new Number(70, 0));
        }

        [TestMethod]
        public void FormatNumbersTest()
        {
            string decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            Assert.AreEqual($"123{decimalSeparator}45", new Number(12345, 2).ToString());
            Assert.AreEqual($"123{decimalSeparator}45", new Number(12345, 2).ToString());
            Assert.AreEqual($"123{decimalSeparator}45", new Number(1234500, 4).ToString());
            Assert.AreEqual($"123{decimalSeparator}45", CreateNumber("0123.4500").ToString());
            Assert.AreEqual($"123{decimalSeparator}45", CreateNumber("00123.45").ToString());
            Assert.AreEqual($"9999999999999999999999999999999999999999{decimalSeparator}9999999999", CreateNumber("9999999999999999999999999999999999999999.9999999999").ToString());
            Assert.AreEqual($"-9999999999999999999999999999999999999999{decimalSeparator}9999999999", CreateNumber("-9999999999999999999999999999999999999999.9999999999").ToString());
            Assert.AreEqual($"0{decimalSeparator}00000001", new Number(1, 8).ToString());
            Assert.AreEqual($"0{decimalSeparator}007", new Number(7, 3).ToString());
            Assert.AreEqual("10101", CreateNumber("010101").ToString());
            Assert.AreEqual($"-10000{decimalSeparator}4", new Number(-100004, 1).ToString());
            Assert.AreEqual("NULL", Number.Null.ToString());
            Assert.AreEqual($"153{decimalSeparator}1", new Number(15310000, 5).ToString());
            Assert.AreEqual($"-153{decimalSeparator}1", CreateNumber("-0153.10000").ToString());
            Assert.AreEqual($"-153{decimalSeparator}10001", CreateNumber("-0153.10001").ToString());

            NumberFormatInfo svSe = NumberFormatInfo.GetInstance(new CultureInfo("sv-SE"));
            Assert.AreEqual("123,45", new Number(12345, 2).ToString(null, svSe));
            Assert.AreEqual("123,45", CreateNumber("00123.45").ToString(null, new CultureInfo("da")));
            Assert.AreEqual("-153,10001", CreateNumber("-0153.10001").ToString(null, new CultureInfo("fr")));
            Assert.AreEqual("-978.453", new Number(-978453, 3).ToString(null, new CultureInfo("en-US")));

            //"C" or "c" - Currency
            Assert.AreEqual("$123.46", new Number(123456, 3).ToString("C", new CultureInfo("en-US")));
            Assert.AreEqual("123,46 €", new Number(123456, 3).ToString("C", new CultureInfo("fr-FR")));
            NumberFormatInfo jaJp = NumberFormatInfo.GetInstance(new CultureInfo("ja-JP"));
            Assert.AreEqual($"{jaJp.CurrencySymbol}123", new Number(123456, 3).ToString("C", new CultureInfo("ja-JP")));
            Assert.AreEqual("(¤123.456)", new Number(-123456, 3).ToString("C3", CultureInfo.InvariantCulture));
            decimal d = -123.456m;
            Assert.AreEqual(d.ToString("C3", new CultureInfo("en-US")), new Number(-123456, 3).ToString("C3", new CultureInfo("en-US")));
            Assert.AreEqual("-123,456 €", new Number(-123456, 3).ToString("C3", new CultureInfo("fr-FR")));
            Assert.AreEqual($"-{jaJp.CurrencySymbol}123.456", new Number(-123456, 3).ToString("C3", new CultureInfo("ja-JP")));
            Assert.AreEqual(ulong.MaxValue.ToString("C2", svSe), new Number(ulong.MaxValue, 0).ToString("C2", svSe));
            Assert.AreEqual(BigInteger.Parse("9999999999999999999999999999999999999999").ToString("C", new CultureInfo("en-US")),
                CreateNumber("9999999999999999999999999999999999999999").ToString("C", new CultureInfo("en-US")));

            //"D" or "d" - Decimal
            Assert.AreEqual("0012.345", new Number(12345, 3).ToString("D7", CultureInfo.InvariantCulture));
            Assert.AreEqual("1234567.89", new Number(123456789, 2).ToString("D0", CultureInfo.InvariantCulture));
            Assert.AreEqual("00123", new Number(123, 0).ToString("D5", CultureInfo.InvariantCulture));
            Assert.AreEqual("0.001", new Number(1, 3).ToString("D4", CultureInfo.InvariantCulture));
            Assert.AreEqual("000.001", new Number(1, 3).ToString("D6", CultureInfo.InvariantCulture));

            //"E" or "e" - Exponential (scientific)
            Assert.AreEqual("1.052033E+003", new Number(10520329112756, 10).ToString("E", new CultureInfo("en-US")));
            Assert.AreEqual("1,052033e+003", new Number(10520329112756, 10).ToString("e", new CultureInfo("fr-FR")));
            Assert.AreEqual("-1.05e+003", new Number(-10520329112756, 10).ToString("e2", new CultureInfo("en-US")));
            Assert.AreEqual("-1,05E+003", new Number(-10520329112756, 10).ToString("E2", new CultureInfo("fr-FR")));
            Assert.AreEqual(0.123456789.ToString("G"), new Number(123456789, 9).ToString("G"));

            //"F" or "f" - Fixed-point
            Assert.AreEqual(1234.567m.ToString("F", new CultureInfo("en-US")), new Number(1234567, 3).ToString("F", new CultureInfo("en-US")));
            Assert.AreEqual(1234.567m.ToString("F", new CultureInfo("de-DE")), new Number(1234567, 3).ToString("F", new CultureInfo("de-DE")));
            Assert.AreEqual("1234.0", new Number(1234, 0).ToString("F1", new CultureInfo("en-US")));
            Assert.AreEqual("1234,0", new Number(1234, 0).ToString("F1", new CultureInfo("de-DE")));
            Assert.AreEqual("-1234.5600", new Number(-123456, 2).ToString("F4", new CultureInfo("en-US")));
            Assert.AreEqual("-1234,5600", new Number(-123456, 2).ToString("F4", new CultureInfo("de-DE")));
            Assert.AreEqual("999999999999999.99999999999999900000", CreateNumber("999999999999999.999999999999999").ToString("F20", new CultureInfo("en-US")));

            //"G" or "g" - General
            Assert.AreEqual("-123.456", new Number(-123456, 3).ToString("G", new CultureInfo("en-US")));
            
            Assert.AreEqual($"{svSe.NegativeSign}123,456", new Number(-123456, 3).ToString("G", svSe));
            Assert.AreEqual("123.5", new Number(1234546, 4).ToString("G4", new CultureInfo("en-US")));
            Assert.AreEqual("123,5", new Number(1234546, 4).ToString("G4", svSe));
            Assert.AreEqual($"{svSe.NegativeSign}0,000000000000000000000000123456789", CreateNumber("-0.000000000000000000000000123456789").ToString("G", svSe));

            //"N" or "n" - Number
            Assert.AreEqual(1234.567m.ToString("N", new CultureInfo("en-US")), new Number(1234567, 3).ToString("N", new CultureInfo("en-US")));
            Assert.AreEqual(1234.567m.ToString("N", svSe), new Number(1234567, 3).ToString("N", svSe));
            Assert.AreEqual("1,234.0", new Number(1234, 0).ToString("N1", new CultureInfo("en-US")));
            Assert.AreEqual("1 234,0", new Number(1234, 0).ToString("N1", svSe));
            Assert.AreEqual("-1,234.560", new Number(-123456, 2).ToString("N3", new CultureInfo("en-US")));
            Assert.AreEqual($"{svSe.NegativeSign}1 234,560", new Number(-123456, 2).ToString("N3", svSe));

            //"P" or "p" - Percent
            Assert.AreEqual(1.ToString("P", new CultureInfo("en-US")), new Number(1, 0).ToString("P", new CultureInfo("en-US")));
            Assert.AreEqual(1.ToString("P", new CultureInfo("fr-FR")), new Number(1, 0).ToString("P", new CultureInfo("fr-FR")));
            Assert.AreEqual("-39.7%", new Number(-39678, 5).ToString("P1", new CultureInfo("en-US")));
            Assert.AreEqual("-39,7 %", new Number(-39678, 5).ToString("P1", new CultureInfo("fr-FR")));
            Assert.AreEqual($"{svSe.NegativeSign}39,68 %", new Number(-39678, 5).ToString("P2", svSe));
            Assert.AreEqual("3.97%", new Number(39678, 6).ToString("P2", new CultureInfo("en-US")));
            Assert.AreEqual("0.40%", new Number(39678, 7).ToString("P2", new CultureInfo("en-US")));
            Assert.AreEqual(10.ToString("p"), new Number(10, 0).ToString("p"));
            Assert.AreEqual(0.02M.ToString("p"), new Number(2, 2).ToString("p"));

            //"0" - Zero placeholder
            Assert.AreEqual("01235", new Number(12345678, 4).ToString("00000"));
            Assert.AreEqual("0.46", new Number(45678, 5).ToString("0.00", new CultureInfo("en-US")));
            Assert.AreEqual("0,46", new Number(45678, 5).ToString("0.00", new CultureInfo("fr-FR")));

            //"#" - Digit placeholder
            Assert.AreEqual("1235", new Number(12345678, 4).ToString("#####"));
            Assert.AreEqual(".46", new Number(45678, 5).ToString("#.##", new CultureInfo("en-US")));
            Assert.AreEqual(",46", new Number(45678, 5).ToString("#.##", new CultureInfo("fr-FR")));

            //"." - Decimal point
            Assert.AreEqual("0.46", new Number(45678, 5).ToString("0.00", new CultureInfo("en-US")));
            Assert.AreEqual("0,46", new Number(45678, 5).ToString("0.00", new CultureInfo("fr-FR")));

            //"," - Group separator and number scaling
            Assert.AreEqual("2,147,483,647", new Number(2147483647, 0).ToString("##,#", new CultureInfo("en-US")));
            Assert.AreEqual("2.147.483.647", new Number(2147483647, 0).ToString("##,#", new CultureInfo("es-ES")));
            Assert.AreEqual("2,147", new Number(2147483647, 0).ToString("#,#,,", new CultureInfo("en-US")));
            Assert.AreEqual("2.147", new Number(2147483647, 0).ToString("#,#,,", new CultureInfo("es-ES")));

            //"%" - Percentage placeholder
            Assert.AreEqual("%36.97", new Number(3697, 4).ToString("%#0.00", new CultureInfo("en-US")));
            Assert.AreEqual("%36,97", new Number(3697, 4).ToString("%#0.00", new CultureInfo("el-GR")));
            Assert.AreEqual("37.0 %", new Number(3697, 4).ToString("##.0 %", new CultureInfo("en-US")));
            Assert.AreEqual("37,0 %", new Number(3697, 4).ToString("##.0 %", new CultureInfo("el-GR")));
            Assert.AreEqual("3,7 %", new Number(3697, 5).ToString("##.0 %", new CultureInfo("el-GR")));

            //"‰" - Per mille placeholder
            Assert.AreEqual("36.97‰", new Number(3697, 5).ToString("#0.00‰", new CultureInfo("en-US")));
            Assert.AreEqual("36,97‰", new Number(3697, 5).ToString("#0.00‰", new CultureInfo("ru-RU")));
            Assert.AreEqual("3,6970‰", new Number(3697, 6).ToString("#0.0000‰", svSe));
            Assert.AreEqual("369,70‰", new Number(3697, 4).ToString("#0.00‰", svSe));

            //Exponential notation
            Assert.AreEqual("98.8e4", new Number(987654, 0).ToString("#0.0e0", new CultureInfo("en-US")));
            Assert.AreEqual("1.504e+03", new Number(150392311, 5).ToString("0.0##e+00", new CultureInfo("en-US")));

            //"\" - Escape character
            Assert.AreEqual("#987654#", new Number(987654, 0).ToString(@"\###00\#"));

            //'string' - Literal string/delimiter
            Assert.AreEqual("68 degrees", new Number(68, 0).ToString("# 'degrees'"));
            Assert.AreEqual("68 degrees", new Number(68, 0).ToString("#' degrees'"));

            //;	Section separator
            Assert.AreEqual("12.35", new Number(12345, 3).ToString("#0.0#;(#0.0#);-\0-", new CultureInfo("en-US")));
            Assert.AreEqual(0.ToString("#0.0#;(#0.0#);-\0-"), new Number(0, 0).ToString("#0.0#;(#0.0#);-\0-"));
            Assert.AreEqual("(12.35)", new Number(-12345, 3).ToString("#0.0#;(#0.0#);-\0-", new CultureInfo("en-US")));
            Assert.AreEqual("12.35", new Number(12345, 3).ToString("#0.0#;(#0.0#)", new CultureInfo("en-US")));
            Assert.AreEqual("0.0", new Number(0, 0).ToString("#0.0#;(#0.0#)", new CultureInfo("en-US")));
            Assert.AreEqual("(12.35)", new Number(-12345, 3).ToString("#0.0#;(#0.0#)", new CultureInfo("en-US")));

            //Other character literals
            Assert.AreEqual("68 °", new Number(68, 0).ToString("# °"));
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void RoundTripFormatting() => new Number(1, 0).ToString("R");

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void HexadecimalFormatting() => new Number(1, 0).ToString("X");

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void FormattingUsingAnUnknownFormatter() => new Number(1, 0).ToString("M");

        private static Number CreateNumber(string s) => Number.Parse(s.GetBytes());
    }
}