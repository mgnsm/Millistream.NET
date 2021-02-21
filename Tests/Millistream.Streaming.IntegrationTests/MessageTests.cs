using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming.IntegrationTests
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void SetMessageCompressionLevelTest()
        {
            using Message message = new Message();
            foreach (CompressionLevel compressionLevel in Enum.GetValues(typeof(CompressionLevel)))
            {
                message.CompressionLevel = compressionLevel;
                Assert.AreEqual(compressionLevel, message.CompressionLevel);
            }
        }

        [TestMethod]
        public void SetUtf8ValidationTest()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Utf8Validation);
            message.Utf8Validation = false;
            Assert.IsFalse(message.Utf8Validation);
        }

        [TestMethod]
        public void AddNumericTest()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "28"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "283"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "0"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "27"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "0.99"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "255.99"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "10.01"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "103.0001"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "104.857"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "104.8576"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "0.0001"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "10741.76"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "256.01"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "284"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "2000"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "20000000000000000"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "18446744073709551610"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "18446.744073709551615"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "1.0000001"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "0.000000000000001"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "-0.000000000000001"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "-1.0000001"));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, "-18446.744073709551615"));
        }

        [TestMethod]
        public void AddInt64Test()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_QUANTITY, -12345L, 2));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 28L, 0));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 283L, 0));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 0L, 0));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 27L, 0));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 99L, 2));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 25599L, 2));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 1001L, 2));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 1030001L, 4));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 104857L, 3));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 1048576L, 4));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 1L, 4));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 1074176L, 2));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 25601L, 2));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 284L, 0));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 2000L, 0));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 20000000000000000L, 0));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, -10000001L, 7));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 10000001L, 7));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_LANGUAGE, 1L, 15));
        }

        [TestMethod]
        public void AddUInt64Test()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_QUANTITY, 12345UL, 2));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 28UL, 0));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 28UL, 0));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 283UL, 0));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 0UL, 0));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 27UL, 0));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 99UL, 2));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 25599UL, 2));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 1001UL, 2));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 1030001UL, 4));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 104857UL, 3));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 1048576UL, 4));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 1UL, 4));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 1074176UL, 2));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 25601UL, 2));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 284UL, 0));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 2000UL, 0));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 20000000000000000UL, 0));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 18446744073709551610UL, 0));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 18446744073709551615UL, 15));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 10000001UL, 7));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_LANGUAGE, 1UL, 15));
        }

        [TestMethod]
        public void AddDateTest()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, 2020, 12, 30));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, 2006, 9, 19));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, 2008, 10, 0));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, 2008, 0, 0));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, 2008, 0, 1));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, 2008, 0, 2 + 2));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, 2008, 0, 4 + 5));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, 2008, 0, 22 + 9));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, 2008, 13, 53 - 22));
            Assert.IsFalse(message.AddDate(Field.MDF_F_DATE, 2020, 13, 40));
            Assert.IsFalse(message.AddDate(Field.MDF_F_DATE, 2020, 14, 30));

            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-12-30"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-12"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-H1"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-H2"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-T1"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-T2"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-T3"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-Q1"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-Q2"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-Q3"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-Q4"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-W1"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2020-W52"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2006-09-19"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2008-10"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2008"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2008-H1"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2008-T2"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2008-Q4"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2008-W22"));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, "2008-W53"));
            Assert.IsFalse(message.AddDate(Field.MDF_F_DATE, "2020-14-30"));
            Assert.IsFalse(message.AddDate(Field.MDF_F_DATE, "-1"));
            Assert.IsFalse(message.AddDate(Field.MDF_F_DATE, "abc"));
        }

        [TestMethod]
        public void AddTimeTest()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddTime(Field.MDF_F_DATE, "17:03:01"));
            Assert.IsTrue(message.AddTime(Field.MDF_F_DATE, "17:03:01.999"));
            Assert.IsTrue(message.AddTime(Field.MDF_F_DATE, "00:00:00"));
            Assert.IsTrue(message.AddTime(Field.MDF_F_DATE, "23:59:58"));
            Assert.IsTrue(message.AddTime(Field.MDF_F_DATE, "23:59:59.001"));
            Assert.IsTrue(message.AddTime(Field.MDF_F_DATE, "23:59:59.999"));
            Assert.IsTrue(message.AddTime(Field.MDF_F_DATE, "23:59:59.000000001"));
            Assert.IsTrue(message.AddTime(Field.MDF_F_DATE, "23:59:59.999999999"));
            Assert.IsFalse(message.AddTime(Field.MDF_F_DATE, "abc"));

            Assert.IsTrue(message.AddTime2(Field.MDF_F_DATE, 17, 3, 1, 999));
            Assert.IsTrue(message.AddTime2(Field.MDF_F_DATE, 0, 0, 0, 0));
            Assert.IsTrue(message.AddTime2(Field.MDF_F_DATE, 23, 59, 58, 0));
            Assert.IsTrue(message.AddTime2(Field.MDF_F_DATE, 23, 59, 59, 1));
            Assert.IsTrue(message.AddTime2(Field.MDF_F_DATE, 23, 59, 59, 999));
            Assert.IsFalse(message.AddTime2(Field.MDF_F_DATE, 33, 3, 1, 999));

            Assert.IsTrue(message.AddTime3(Field.MDF_F_DATE, 17, 3, 1, 999999999));
            Assert.IsTrue(message.AddTime3(Field.MDF_F_DATE, 17, 3, 1, 999));
            Assert.IsTrue(message.AddTime3(Field.MDF_F_DATE, 0, 0, 0, 0));
            Assert.IsTrue(message.AddTime3(Field.MDF_F_DATE, 23, 59, 58, 0));
            Assert.IsTrue(message.AddTime3(Field.MDF_F_DATE, 0, 0, 0, 1));
            Assert.IsTrue(message.AddTime3(Field.MDF_F_DATE, 23, 59, 59, 1));
            Assert.IsTrue(message.AddTime3(Field.MDF_F_DATE, 23, 59, 59, 999999999));
            Assert.IsFalse(message.AddTime3(Field.MDF_F_DATE, 17, 88, 1, 999));
        }

        [TestMethod]
        public void AddListTest()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_BASICDATA));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "28"));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "28 28"));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "28 28 343"));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "=28"));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "=28 28"));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "=28 28 343"));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "+28"));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "+28 28"));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "+28 28 343"));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "-28"));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "-28 28"));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, "-28 28 343"));
        }

        [TestMethod]
        public void ResetTest()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.AreEqual(1, message.ActiveCount);
            message.Reset();
            Assert.AreEqual(0, message.ActiveCount);
        }

        [TestMethod]
        public void DeleteTest()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_BASICDATA));
            Assert.AreEqual(1, message.ActiveCount);
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.AreEqual(2, message.ActiveCount);
            message.Delete();
            Assert.AreEqual(1, message.ActiveCount);
            message.Delete();
            Assert.AreEqual(0, message.ActiveCount);
        }

        [TestMethod]
        public void AddStringTest()
        {
            string[] strings = new string[]
            {
                "Foo Bar",
                "aaaBBBccc",
                "SE0000108656",
                "åäöÅÄÖæß€µ",
                "12-------P----",
                "46--X-B--P-3--",
            };

            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            foreach (string @string in strings)
            {
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, @string));
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, @string, @string.Length));
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, @string, @string.Length + 5));
                Assert.IsFalse(message.AddString(Field.MDF_F_LANGUAGE, @string, -1));
            }
        }

        [TestMethod]
        public void MoveTest()
        {
            using Message source = new Message();
            const ulong SourceInsRef = 1;
            Assert.IsTrue(source.Add(SourceInsRef, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(source.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(source.Add(SourceInsRef, MessageReference.MDF_M_BASICDATA));
            Assert.IsTrue(source.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(source.Add(SourceInsRef, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(source.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.AreEqual(3, source.ActiveCount);

            using Message destination = new Message();
            Assert.AreEqual(0, destination.ActiveCount);

            Assert.IsTrue(Message.Move(source, destination, 1, 10));
            Assert.AreEqual(0, source.ActiveCount);
            Assert.AreEqual(3, destination.ActiveCount);

            Assert.IsFalse(Message.Move(source, null, 1, 10));
            Assert.IsTrue(Message.Move(destination, null, 10, 11));
            Assert.IsTrue(Message.Move(destination, destination, 11, 12));
        }

        [TestMethod]
        public void SerializeAndDeserializeTest()
        {
            using Message message = new Message();
            Assert.IsFalse(message.Serialize(out IntPtr result));
            Assert.AreEqual(IntPtr.Zero, result);
            //serialize
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_ASKYIELD, "123"));
            Assert.IsTrue(message.Serialize(out result));
            Assert.AreNotEqual(IntPtr.Zero, result);
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.Serialize(out result));
            Assert.AreNotEqual(IntPtr.Zero, result);
            //deserialize
            string s = Marshal.PtrToStringAnsi(result);
            using Message message2 = new Message();
            Assert.IsTrue(message2.Deserialize(s));
            Assert.AreEqual(2, message2.ActiveCount);
            Assert.IsTrue(message.Deserialize(s));
        }
    }
}