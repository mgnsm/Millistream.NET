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
            Assert.AreEqual("40.05", number4.ToString(CultureInfo.InvariantCulture));

            Number number5 = new Number(new BigInteger(1010000007), 7);
            Assert.IsFalse(number5.IsNegative);
            Assert.AreEqual(10, number5.GetPrecision());
            Assert.AreEqual(7, number5.Scale);
            Assert.AreEqual("101.0000007", number5.ToString(CultureInfo.InvariantCulture));

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

        [TestMethod]
        public void FormatNumbersTest()
        {
            string decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            Assert.AreEqual($"123{decimalSeparator}45", new Number(12345, 2).ToString());
            Assert.AreEqual($"123{decimalSeparator}45", new Number(12345, 2).ToString());
            Assert.AreEqual($"123{decimalSeparator}4500", new Number(1234500, 4).ToString());
            Assert.AreEqual($"0123{decimalSeparator}4500", CreateNumber("0123.4500").ToString());
            Assert.AreEqual($"00123{decimalSeparator}45", CreateNumber("00123.45").ToString());
            Assert.AreEqual($"9999999999999999999999999999999999999999{decimalSeparator}9999999999", CreateNumber("9999999999999999999999999999999999999999.9999999999").ToString());
            Assert.AreEqual($"-9999999999999999999999999999999999999999{decimalSeparator}9999999999", CreateNumber("-9999999999999999999999999999999999999999.9999999999").ToString());
            Assert.AreEqual("010101", CreateNumber("010101").ToString());
            Assert.AreEqual($"-10000{decimalSeparator}4", new Number(-100004, 1).ToString());
            Assert.AreEqual("NULL", Number.Null.ToString());
            Assert.AreEqual($"153{decimalSeparator}10000", new Number(15310000, 5).ToString());
            Assert.AreEqual($"-0153{decimalSeparator}10000", CreateNumber("-0153.10000").ToString());
            Assert.AreEqual($"-0153{decimalSeparator}10001", CreateNumber("-0153.10001").ToString());

            Assert.AreEqual("123,45", new Number(12345, 2).ToString(null, new CultureInfo("sv")));
            Assert.AreEqual("00123,45", CreateNumber("00123.45").ToString(null, new CultureInfo("da")));
            Assert.AreEqual("-0153,10001", CreateNumber("-0153.10001").ToString(null, new CultureInfo("fr")));
            Assert.AreEqual("-978.453", new Number(-978453, 3).ToString(null, new CultureInfo("en-US")));

            //"C" or "c" - Currency
            Assert.AreEqual("$123.46", new Number(123456, 3).ToString("C", new CultureInfo("en-US")));
            Assert.AreEqual("123,46 €", new Number(123456, 3).ToString("C", new CultureInfo("fr-FR")));
            Assert.AreEqual("¥123", new Number(123456, 3).ToString("C", new CultureInfo("ja-JP")));
            Assert.AreEqual("($123.456)", new Number(-123456, 3).ToString("C3", new CultureInfo("en-US")));
            Assert.AreEqual("-123,456 €", new Number(-123456, 3).ToString("C3", new CultureInfo("fr-FR")));
            Assert.AreEqual("-¥123.456", new Number(-123456, 3).ToString("C3", new CultureInfo("ja-JP")));
            Assert.AreEqual(ulong.MaxValue.ToString("C2", new CultureInfo("sv-SE")), new Number(ulong.MaxValue, 0).ToString("C2", new CultureInfo("sv-SE")));
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
            Assert.AreEqual("1234.57", new Number(1234567, 3).ToString("F", new CultureInfo("en-US")));
            Assert.AreEqual("1234,57", new Number(1234567, 3).ToString("F", new CultureInfo("de-DE")));
            Assert.AreEqual("1234.0", new Number(1234, 0).ToString("F1", new CultureInfo("en-US")));
            Assert.AreEqual("1234,0", new Number(1234, 0).ToString("F1", new CultureInfo("de-DE")));
            Assert.AreEqual("-1234.5600", new Number(-123456, 2).ToString("F4", new CultureInfo("en-US")));
            Assert.AreEqual("-1234,5600", new Number(-123456, 2).ToString("F4", new CultureInfo("de-DE")));
            Assert.AreEqual("999999999999999.99999999999999900000", CreateNumber("999999999999999.999999999999999").ToString("F20", new CultureInfo("en-US")));

            //"G" or "g" - General
            Assert.AreEqual("-123.456", new Number(-123456, 3).ToString("G", new CultureInfo("en-US")));
            Assert.AreEqual("-123,456", new Number(-123456, 3).ToString("G", new CultureInfo("sv-SE")));
            Assert.AreEqual("123.5", new Number(1234546, 4).ToString("G4", new CultureInfo("en-US")));
            Assert.AreEqual("123,5", new Number(1234546, 4).ToString("G4", new CultureInfo("sv-SE")));
            Assert.AreEqual("-0,000000000000000000000000123456789", CreateNumber("-0.000000000000000000000000123456789").ToString("G", new CultureInfo("sv-SE")));

            //"N" or "n" - Number
            Assert.AreEqual("1,234.57", new Number(1234567, 3).ToString("N", new CultureInfo("en-US")));
            Assert.AreEqual("1 234,57", new Number(1234567, 3).ToString("N", new CultureInfo("sv-SE")));
            Assert.AreEqual("1,234.0", new Number(1234, 0).ToString("N1", new CultureInfo("en-US")));
            Assert.AreEqual("1 234,0", new Number(1234, 0).ToString("N1", new CultureInfo("sv-SE")));
            Assert.AreEqual("-1,234.560", new Number(-123456, 2).ToString("N3", new CultureInfo("en-US")));
            Assert.AreEqual("-1 234,560", new Number(-123456, 2).ToString("N3", new CultureInfo("sv-SE")));

            //"P" or "p" - Percent
            Assert.AreEqual("100.00%", new Number(1, 0).ToString("P", new CultureInfo("en-US")));
            Assert.AreEqual("100,00 %", new Number(1, 0).ToString("P", new CultureInfo("fr-FR")));
            Assert.AreEqual("-39.7%", new Number(-39678, 5).ToString("P1", new CultureInfo("en-US")));
            Assert.AreEqual("-39,7 %", new Number(-39678, 5).ToString("P1", new CultureInfo("fr-FR")));
            Assert.AreEqual("-39,68 %", new Number(-39678, 5).ToString("P2", new CultureInfo("sv-SE")));
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
            Assert.AreEqual("3,6970‰", new Number(3697, 6).ToString("#0.0000‰", new CultureInfo("sv-SE")));
            Assert.AreEqual("369,70‰", new Number(3697, 4).ToString("#0.00‰", new CultureInfo("sv-SE")));

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

        private static void ParseNumberTest(string s)
        {
            Assert.IsTrue(Number.TryParse(s, out Number number));
            Assert.AreEqual(s, number.ToString(CultureInfo.InvariantCulture));
            Assert.IsTrue(Number.TryParse(s.GetBytes(), out number));
            Assert.AreEqual(s, number.ToString(CultureInfo.InvariantCulture));
        }

        private static Number CreateNumber(string s) => Number.Parse(s.GetBytes());
    }
}