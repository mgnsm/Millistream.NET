using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class DateTests
    {
        [TestMethod]
        public void ParseDayTest()
        {
            ParseDateTest(2019, 12, 13);
            ParseDateTest(1, 1, 1);
            ParseDateTest(9999, 12, 31);
            ParseDateTest(2025, 1, 31);
            ParseDateTest(2000, 2, 29);

            Assert.IsFalse(Date.TryParse("2019-02-29", out Date date));
            Assert.AreEqual(default, date);
            Assert.IsFalse(Date.TryParse("2019-2-28", out date));
            Assert.AreEqual(default, date);
            Assert.IsFalse(Date.TryParse("0000-01-01", out _));
            Assert.IsFalse(Date.TryParse("2018-06-31", out _));
            Assert.IsFalse(Date.TryParse("x", out _));
            Assert.IsFalse(Date.TryParse("20190101", out _));
        }

        [TestMethod]
        public void ParseMonthTest()
        {
            ParseDateTest(2019, 12, null);
            ParseDateTest(1, 1, null);
            ParseDateTest(9999, 12, null);
            ParseDateTest(2025, 1, null);
            ParseDateTest(2000, 2, null);

            Assert.IsFalse(Date.TryParse("2019-14", out Date date));
            Assert.AreEqual(default, date);
            Assert.IsTrue(Date.TryParse("2025-12", out date));
            Assert.AreEqual(DateFormat.Month, date.Format);
            Assert.IsFalse(Date.TryParse("2019-1", out _));
        }

        [TestMethod]
        public void ParseYearTest()
        {
            ParseDateTest(2019, null, null);
            ParseDateTest(1, null, null);
            ParseDateTest(9999, null, null);
            ParseDateTest(2025, null, null);
            ParseDateTest(2000, null, null);

            Assert.IsFalse(Date.TryParse("10000", out _));
            Assert.IsFalse(Date.TryParse("0", out _));
            Assert.IsTrue(Date.TryParse("2020", out Date date));
            Assert.AreEqual(DateFormat.Year, date.Format);
            Assert.AreEqual(2020, date.Year);
        }

        [TestMethod]
        public void ParseQuarterTest()
        {
            Assert.IsTrue(Date.TryParse("2019-Q2", out Date date));
            Assert.IsFalse(date.Day.HasValue);
            Assert.IsFalse(date.Month.HasValue);
            Assert.IsTrue(date.Quarter.HasValue);
            Assert.IsFalse(date.SemiAnnual.HasValue);
            Assert.IsFalse(date.Tertiary.HasValue);
            Assert.AreEqual(2019, date.Year);
            Assert.IsFalse(date.Week.HasValue);
            Assert.AreEqual(DateFormat.Quarter, date.Format);
            Assert.AreEqual(2, date.Quarter.Value.Number);
            Assert.AreEqual(2019, date.Quarter.Value.Year);
            Assert.AreEqual("2019-Q2", date.ToString());

            Assert.IsFalse(Date.TryParse("2019-Q5", out _));
            Assert.IsTrue(Date.TryParse("2011-Q1", out _));
            Assert.IsTrue(Date.TryParse("2012-Q3", out _));
            Assert.IsTrue(Date.TryParse("2016-Q4", out _));
            Assert.IsFalse(Date.TryParse("2020-Q0", out _));
        }

        [TestMethod]
        public void ParseTertiaryTest()
        {
            Assert.IsTrue(Date.TryParse("2020-T3", out Date date));
            Assert.IsFalse(date.Day.HasValue);
            Assert.IsFalse(date.Month.HasValue);
            Assert.IsFalse(date.Quarter.HasValue);
            Assert.IsFalse(date.SemiAnnual.HasValue);
            Assert.IsTrue(date.Tertiary.HasValue);
            Assert.AreEqual(2020, date.Year);
            Assert.IsFalse(date.Week.HasValue);
            Assert.AreEqual(DateFormat.Tertiary, date.Format);
            Assert.AreEqual(3, date.Tertiary.Value.Number);
            Assert.AreEqual(2020, date.Tertiary.Value.Year);
            Assert.AreEqual("2020-T3", date.ToString());

            Assert.IsFalse(Date.TryParse("2019-T5", out _));
            Assert.IsTrue(Date.TryParse("2011-T1", out _));
            Assert.IsTrue(Date.TryParse("2012-T3", out _));
            Assert.IsFalse(Date.TryParse("2016-T4", out _));
            Assert.IsFalse(Date.TryParse("2020-T0", out _));
        }

        [TestMethod]
        public void ParseSemiAnnualTest()
        {
            Assert.IsTrue(Date.TryParse("2021-H2", out Date date));
            Assert.IsFalse(date.Day.HasValue);
            Assert.IsFalse(date.Month.HasValue);
            Assert.IsFalse(date.Quarter.HasValue);
            Assert.IsTrue(date.SemiAnnual.HasValue);
            Assert.IsFalse(date.Tertiary.HasValue);
            Assert.AreEqual(2021, date.Year);
            Assert.IsFalse(date.Week.HasValue);
            Assert.AreEqual(DateFormat.SemiAnnual, date.Format);
            Assert.AreEqual(2, date.SemiAnnual.Value.Number);
            Assert.AreEqual(2021, date.SemiAnnual.Value.Year);
            Assert.AreEqual("2021-H2", date.ToString());

            Assert.IsFalse(Date.TryParse("2019-H5", out _));
            Assert.IsTrue(Date.TryParse("2011-H1", out _));
            Assert.IsFalse(Date.TryParse("2012-H3", out _));
            Assert.IsFalse(Date.TryParse("2016-H4", out _));
            Assert.IsFalse(Date.TryParse("2020-H0", out _));
            Assert.IsFalse(Date.TryParse("2020-Hx", out _));
        }

        [TestMethod]
        public void ParseWeekTest()
        {
            Assert.IsTrue(Date.TryParse("2020-W40", out Date date));
            Assert.IsFalse(date.Day.HasValue);
            Assert.IsFalse(date.Month.HasValue);
            Assert.IsFalse(date.Quarter.HasValue);
            Assert.IsFalse(date.SemiAnnual.HasValue);
            Assert.IsFalse(date.Tertiary.HasValue);
            Assert.AreEqual(2020, date.Year);
            Assert.IsTrue(date.Week.HasValue);
            Assert.AreEqual(DateFormat.Week, date.Format);
            Assert.AreEqual(40, date.Week.Value.Number);
            Assert.AreEqual(2020, date.Week.Value.Year);
            Assert.AreEqual("2020-W40", date.ToString());

            Assert.IsTrue(Date.TryParse("2019-W1", out date));
            Assert.AreEqual("2019-W01", date.ToString());
            Assert.AreEqual(2019, date.Year);
            Assert.IsTrue(date.Week.HasValue);
            Assert.AreEqual(1, date.Week.Value.Number);

            Assert.IsTrue(Date.TryParse("2011-W01", out _));
            Assert.IsFalse(Date.TryParse("2012-W54", out _));
            Assert.IsFalse(Date.TryParse("2016-W100", out _));
            Assert.IsFalse(Date.TryParse("2020-Wxx", out _));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseDateWithInvalidCharsTest() => Date.Parse("@x/");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseDateWithInvalidBytesTest() => Date.Parse("10000".GetBytes());

        [TestMethod]
        public void CompareDatesTest()
        {
            Assert.AreEqual(Date.Parse("2019-12-13"), Date.Parse("2019-12-13"));
            Assert.AreEqual(Date.Parse("2019"), new Date(new Year(2019)));
            Assert.IsTrue(Date.Parse("2019") == new Date(new Year(2019)));
            Assert.IsTrue(Date.Parse("2020") != new Date(new Year(2019)));
            Assert.AreEqual(Date.Parse("2019").GetHashCode(), new Date(new Year(2019)).GetHashCode());
            Assert.AreNotEqual(Date.Parse("2019-Q1"), Date.Parse("2019-01"));
            Assert.AreNotEqual(Date.Parse("2019-H1"), Date.Parse("2019-T1"));
            Assert.AreNotEqual(Date.Parse("2019-W01"), Date.Parse("2019-01-01"));
            Assert.AreEqual(Date.Parse("2019-W01"), Date.Parse("2019-W01"));
            Assert.IsFalse(Date.Parse("2019-01-01").Equals(new DateTime(2019,1,1)));
            Assert.IsTrue(Date.Parse("2019-12").Equals(new Date(new Month(new Year(2019), 12))));
        }

        private static void ParseDateTest(int y, int? m, int? d)
        {
            const char PaddingChar = '0';

            StringBuilder sb = new StringBuilder();
            sb.Append($"{y.ToString().PadLeft(4, PaddingChar)}");
            int expectedLength = 4;
            if (m.HasValue)
            {
                sb.Append("-");
                sb.Append(m.ToString().PadLeft(2, PaddingChar));
                expectedLength = 7;
            }
            if (d.HasValue)
            {
                sb.Append("-");
                sb.Append(d.ToString().PadLeft(2, PaddingChar));
                expectedLength = 10;
            }

            string s = sb.ToString();
            Assert.AreEqual(expectedLength, s.Length);

            void Verify(Date date)
            {
                Year year = new Year(y);
                Assert.AreEqual(year, date.Year);
                if (m.HasValue)
                {
                    Month month = new Month(year, m.Value);
                    Assert.AreEqual(month, date.Month);

                    if (d.HasValue)
                    {
                        Assert.AreEqual(new Day(month, d.Value), date.Day);
                        Assert.AreEqual(DateFormat.Day, date.Format);
                    }
                    else
                    {
                        Assert.IsFalse(date.Day.HasValue);
                        Assert.AreEqual(DateFormat.Month, date.Format);
                    }
                }
                else
                {
                    Assert.IsFalse(date.Month.HasValue);
                    Assert.AreEqual(DateFormat.Year, date.Format);
                }

                Assert.IsFalse(date.Quarter.HasValue);
                Assert.IsFalse(date.SemiAnnual.HasValue);
                Assert.IsFalse(date.Tertiary.HasValue);
                Assert.IsFalse(date.Week.HasValue);
                Assert.AreEqual(s, date.ToString());
            }

            Assert.IsTrue(Date.TryParse(s, out Date date));
            Verify(date);
            Assert.IsTrue(Date.TryParse(s.GetBytes(), out date));
            Verify(date);
        }
    }
}