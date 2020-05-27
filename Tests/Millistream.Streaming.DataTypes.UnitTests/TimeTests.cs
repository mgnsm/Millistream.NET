using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class TimeTests
    {
        [TestMethod]
        public void TryParseTimeTest()
        {
            TryParseTimeTest(23, 59, 59, null, null);
            TryParseTimeTest(1, 1, 1, 1, null);
            TryParseTimeTest(12, 12, 13, null, 1);
            TryParseTimeTest(9, 9, 13, 1, null);

            string timeValueToParse = "12:12:13.000000001";
            Assert.IsTrue(Time.TryParse(timeValueToParse, out _));
            Assert.IsTrue(Time.TryParse(timeValueToParse.GetBytes(), out Time time));
            Assert.AreEqual(12, time.Hours);
            Assert.AreEqual(12, time.Minutes);
            Assert.AreEqual(13, time.Seconds);
            Assert.AreEqual(0, time.Milliseconds);
            Assert.AreEqual(1, time.Nanoseconds);
            Assert.AreEqual(timeValueToParse, time.ToString());

            timeValueToParse = "12:12:13.123456789";
            Assert.IsTrue(Time.TryParse(timeValueToParse, out _));
            Assert.IsTrue(Time.TryParse(timeValueToParse.GetBytes(), out time));
            Assert.AreEqual(12, time.Hours);
            Assert.AreEqual(12, time.Minutes);
            Assert.AreEqual(13, time.Seconds);
            Assert.AreEqual(123, time.Milliseconds);
            Assert.AreEqual(456789, time.Nanoseconds);
            Assert.AreEqual(timeValueToParse, time.ToString());

            Assert.IsFalse(Time.TryParse("23:59:60", out time));
            Assert.AreEqual(default, time);
            Assert.IsFalse(Time.TryParse("11:11:11.1234", out time));
            Assert.AreEqual(default, time);
            Assert.IsFalse(Time.TryParse("11:11:11.1234567899", out time));
            Assert.AreEqual(default, time);
            Assert.IsFalse(Time.TryParse("abc", out time));
            Assert.AreEqual(default, time);
            Assert.IsFalse(Time.TryParse("123", out time));
            Assert.AreEqual(default, time);
            Assert.IsFalse(Time.TryParse("24:00:00", out time));
            Assert.AreEqual(default, time);
        }

        [TestMethod]
        public void CreateTimeTest()
        {
            CreateTimeTest(12, 12, 12, 0, 0);
            CreateTimeTest(9, 9, 8, 1, 1);
            CreateTimeTest(23, 59, 59, 999, 999_999);
            CreateTimeTest(0, 0, 0, 0, 1);
            CreateTimeTest(0, 13, 0, 1, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooLargeHourTest() => CreateTimeTest(24, 0, 0, 0, 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooSmallHourTest() => CreateTimeTest(-1, 0, 0, 0, 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooLargeMinuteTest() => CreateTimeTest(0, 60, 0, 0, 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooSmallMinuteTest() => CreateTimeTest(0, -1, 0, 0, 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooLargeSecondTest() => CreateTimeTest(0, 0, 60, 0, 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooSmallSecondTest() => CreateTimeTest(0, 0, -1, 0, 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooLargeMillisecondTest() => CreateTimeTest(0, 0, 0, 1_000, 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooSmallMillisecondTest() => CreateTimeTest(0, 0, 0, -1, 0);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooLargeNanosecondTest() => CreateTimeTest(0, 0, 0, 0, 1_000_000);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TooSmallNanosecondTest() => CreateTimeTest(0, 0, 0, 0, -1);

        [TestMethod]
        public void AddTimeTest()
        {
            Assert.AreEqual(new Time(4, 16, 5), Time.Parse("01:30:45").Add(Time.Parse("02:45:20")));
            Assert.AreEqual(new Time(4, 16, 5), Time.Parse("01:30:45") + Time.Parse("02:45:20"));
            Assert.AreEqual(new Time(3, 55, 0), Time.Parse("02:45:00").Add(Time.Parse("01:10:00")));
            Assert.AreEqual(new Time(3, 55, 0), Time.Parse("02:45:00") + Time.Parse("01:10:00"));
            Assert.AreEqual(new Time(4, 5, 46), Time.Parse("02:45:01").Add(Time.Parse("01:20:45")));
            Assert.AreEqual(new Time(4, 5, 46), Time.Parse("02:45:01") + Time.Parse("01:20:45"));
            Assert.AreEqual(new Time(4, 20, 1, 122, 0), Time.Parse("14:00:00.123").Add(Time.Parse("14:20:00.999")));
            Assert.AreEqual(new Time(4, 20, 1, 122, 0), Time.Parse("14:00:00.123") + Time.Parse("14:20:00.999"));
            Assert.AreEqual(new Time(3, 5, 7, 998, 999999), Time.Parse("16:05:07.999999999").Add(Time.Parse("10:59:59.999")));
            Assert.AreEqual(new Time(3, 5, 7, 998, 999999), Time.Parse("16:05:07.999999999") + Time.Parse("10:59:59.999"));
        }

        [TestMethod]
        public void SubtractTimeTest()
        {
            Assert.AreEqual(new Time(3, 5, 25), Time.Parse("04:10:45").Subtract(Time.Parse("01:05:20")));
            Assert.AreEqual(new Time(3, 5, 25), Time.Parse("04:10:45") - Time.Parse("01:05:20"));
            Assert.AreEqual(new Time(2, 35, 0), Time.Parse("04:10:00").Subtract(Time.Parse("01:35:00")));
            Assert.AreEqual(new Time(2, 35, 0), Time.Parse("04:10:00") - Time.Parse("01:35:00"));
            Assert.AreEqual(new Time(23, 15, 00), Time.Parse("15:20:00").Subtract(Time.Parse("16:05:00")));
            Assert.AreEqual(new Time(23, 15, 00), Time.Parse("15:20:00") - Time.Parse("16:05:00"));
            Assert.AreEqual(new Time(21, 54, 54, 900, 0), Time.Parse("04:10:00.100").Subtract(Time.Parse("06:15:05.200")));
            Assert.AreEqual(new Time(21, 54, 54, 900, 0), Time.Parse("04:10:00.100") - Time.Parse("06:15:05.200"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareTimeToObjectOfAnotherTypeTest()
        {
            Time time = new Time(1, 1, 1);
            time.CompareTo(TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public void CompareTimesTest()
        {
            Assert.AreEqual(1, new Time(1, 1, 1).CompareTo(null));
            Assert.AreEqual(-1, new Time(1, 1, 1).CompareTo(new Time(1, 1, 1, 1, 0)));
            Assert.AreEqual(1, new Time(4, 4, 4, 0, 1).CompareTo(new Time(4, 4, 4, 0, 0)));
            Assert.AreEqual(0, new Time(4, 4, 4, 0, 1).CompareTo(new Time(4, 4, 4, 0, 1)));
            Assert.AreEqual(-1, new Time(1, 1, 1).CompareTo(new Time(1, 1, 2)));
            Assert.AreEqual(1, new Time(1, 2, 1).CompareTo(new Time(1, 1, 1)));
            Assert.AreEqual(-1, new Time(0, 1, 1).CompareTo(new Time(1, 1, 1)));
            Assert.AreEqual(1, new Time(2, 1, 1).CompareTo(new Time(1, 1, 1)));

            Assert.IsTrue(new Time(1, 1, 1).Equals(new Time(1, 1, 1)));
            Assert.IsTrue(new Time(1, 1, 1) == new Time(1, 1, 1));
            Assert.IsTrue(new Time(0, 1, 0, 1, 0).Equals(new Time(0, 1, 0, 1, 0)));
            Assert.IsTrue(new Time(0, 1, 0, 1, 0) == new Time(0, 1, 0, 1, 0));
            Assert.IsTrue(new Time(0, 1, 0, 1, 0) != new Time(0, 1, 0, 1, 1));
            Assert.IsTrue(new Time(1, 1, 1) != new Time(1, 1, 0));
            Assert.AreNotEqual(new Time(1, 1, 1).GetHashCode(), new Time(1, 1, 0).GetHashCode());
            Assert.AreEqual(new Time(1, 1, 1).GetHashCode(), new Time(1, 1, 1).GetHashCode());

            Assert.IsTrue(Time.Parse("21:12:12.123") > Time.Parse("21:12:12.122"));
            Assert.IsTrue(Time.Parse("21:12:12.123") >= Time.Parse("21:12:12.122"));
            Assert.IsFalse(Time.Parse("21:12:12.123") < Time.Parse("21:12:12.122"));
            Assert.IsTrue(Time.Parse("00:01:01.001") < Time.Parse("00:01:01.002"));
            Assert.IsTrue(Time.Parse("00:01:01.001") <= Time.Parse("00:01:01.002"));
            Assert.IsFalse(Time.Parse("00:01:01.001") > Time.Parse("00:01:01.002"));
        }

        [TestMethod]
        public void TimeToStringTest()
        {
            Assert.AreEqual("21:54:54.900", new Time(21, 54, 54, 900, 0).ToString());
            Assert.AreEqual("00:00:00.000000001", new Time(0, 0, 0, 0, 1).ToString());
            Assert.AreEqual("00:59:00.001", new Time(0, 59, 0, 1, 0).ToString());
            Assert.AreEqual("00:59:00.001000010", new Time(0, 59, 0, 1, 10).ToString());
            Assert.AreEqual("01:02:04.999888888", new Time(1, 2, 04, 999, 888888).ToString());
            Assert.AreEqual("14:51:02.123", Time.Parse("14:51:02.123").ToString());
            Assert.AreEqual("01:02:03.123456789", Time.Parse("01:02:03.123456789").ToString());
            Assert.AreEqual("23:10:59", Time.Parse("23:10:59").ToString());
        }

        private static void TryParseTimeTest(int hours, int minutes, int seconds, int? milliseconds, int? nanoseconds)
        {
            void AssertPropertyValues(Time time)
            {
                Assert.AreEqual(hours, time.Hours);
                Assert.AreEqual(minutes, time.Minutes);
                Assert.AreEqual(seconds, time.Seconds);
                if (milliseconds.HasValue)
                    Assert.AreEqual(milliseconds.Value, time.Milliseconds);
                else if (nanoseconds.HasValue)
                    Assert.AreEqual(nanoseconds.Value, time.Nanoseconds);
            }

            const char PaddingChar = '0';
            string AddHoursMinutesOrSeconds(int i) => i.ToString().PadLeft(2, PaddingChar);
            string AddMilliseconds(int milliseconds) => $".{milliseconds.ToString().PadLeft(3, PaddingChar)}";
            string AddNanoSeconds(int nanoseconds) => $".{nanoseconds.ToString().PadLeft(9, PaddingChar)}";

            string s = $"{AddHoursMinutesOrSeconds(hours)}:{AddHoursMinutesOrSeconds(minutes)}:{AddHoursMinutesOrSeconds(seconds)}";
            if (milliseconds.HasValue)
                s += AddMilliseconds(milliseconds.Value);
            else if (nanoseconds.HasValue)
                s += AddNanoSeconds(nanoseconds.Value);

            Assert.IsTrue(Time.TryParse(s, out Time time));
            AssertPropertyValues(time);
            Assert.IsTrue(Time.TryParse(s.GetBytes(), out time));
            AssertPropertyValues(time);
        }

        private static void CreateTimeTest(int hours, int minutes, int seconds, int milliseconds, int nanoseconds)
        {
            Time time = new Time(hours, minutes, seconds, milliseconds, nanoseconds);
            Assert.AreEqual(hours, time.Hours);
            Assert.AreEqual(minutes, time.Minutes);
            Assert.AreEqual(seconds, time.Seconds);
            Assert.AreEqual(milliseconds, time.Milliseconds);
            Assert.AreEqual(nanoseconds, time.Nanoseconds);
        }
    }
}