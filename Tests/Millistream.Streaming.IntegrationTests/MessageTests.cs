using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Millistream.Streaming.IntegrationTests
{
    [TestClass]
    public class MessageTests
    {
        private static readonly byte[] s_empty = new byte[1] { (byte)'\0' };

        [TestMethod]
        public void CreateMessageTest()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using Message message = new Message("libmdf.so.0");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using Message message = new Message("libmdf-0.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                using Message message = new Message("libmdf.0.dylib");
            }
            else
                throw new PlatformNotSupportedException();
        }

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
        public void SetDelayTest()
        {
            using Message message = new Message();
            Assert.AreEqual(default, message.Delay);
            const byte Delay = byte.MaxValue;
            message.Delay = Delay;
            Assert.AreEqual(Delay, message.Delay);
        }

        [TestMethod]
        public void AddNumericTest()
        {
            string[] values =
            {
                "28",
                "283",
                "0",
                "27",
                "0.99",
                "255.99",
                "10.01",
                "103.0001",
                "104.857",
                "104.8576",
                "0.0001",
                "10741.76",
                "256.01",
                "284",
                "2000",
                "20000000000000000",
                "18446744073709551610",
                "18446.744073709551615",
                "1.0000001",
                "0.000000000000001",
                "-0.000000000000001",
                "-1.0000001",
                "-18446.744073709551615"
            };

            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            foreach (string value in values)
            {
                Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, value));
                Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, Encoding.UTF8.GetBytes(value + char.MinValue)));
                Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, Encoding.UTF8.GetBytes(value)));
            }
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, default(string)));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, default(ReadOnlySpan<byte>)));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, string.Empty));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_LANGUAGE, s_empty));
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
            string[] validValues = new string[]
            {
                "2020-12-30",
                "2020-12",
                "2020-H1",
                "2020-H2",
                "2020-T1",
                "2020-T2",
                "2020-T3",
                "2020-Q1",
                "2020-Q2",
                "2020-Q3",
                "2020-Q4",
                "2020-W1",
                "2020-W52",
                "2006-09-19",
                "2008-10",
                "2008",
                "2008-H1",
                "2008-T2",
                "2008-Q4",
                "2008-W22",
                "2008-W53"
            };

            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));

            foreach (string value in validValues)
            {
                Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, value));
                Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, Encoding.UTF8.GetBytes(value + char.MinValue)));
                Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, Encoding.UTF8.GetBytes(value)));
            }

            string[] invalidValues = new string[]
            {
                "2020-14-30",
                "-1",
                "abc"
            };

            foreach (string invalidValue in invalidValues)
            {
                Assert.IsFalse(message.AddDate(Field.MDF_F_DATE, invalidValue));
                Assert.IsFalse(message.AddDate(Field.MDF_F_DATE, Encoding.UTF8.GetBytes(invalidValue + char.MinValue)));
                Assert.IsFalse(message.AddDate(Field.MDF_F_DATE, Encoding.UTF8.GetBytes(invalidValue)));
            }

            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, default(string)));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, default(ReadOnlySpan<byte>)));
            Assert.IsFalse(message.AddDate(Field.MDF_F_DATE, string.Empty));
            Assert.IsFalse(message.AddDate(Field.MDF_F_DATE, s_empty));

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
        }

        [TestMethod]
        public void AddTimeTest()
        {
            string[] values = new string[]
            {
                "17:03:01",
                "17:03:01.999",
                "00:00:00",
                "23:59:58",
                "23:59:59.001",
                "23:59:59.999",
                "23:59:59.000000001",
                "23:59:59.999999999"
            };

            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));

            foreach (string value in values)
            {
                Assert.IsTrue(message.AddTime(Field.MDF_F_DATE, value));
                Assert.IsTrue(message.AddTime(Field.MDF_F_DATE, Encoding.UTF8.GetBytes(value + char.MinValue)));
                Assert.IsTrue(message.AddTime(Field.MDF_F_DATE, Encoding.UTF8.GetBytes(value)));
            }
            Assert.IsFalse(message.AddTime(Field.MDF_F_DATE, "abc"));
            Assert.IsFalse(message.AddTime(Field.MDF_F_DATE, Encoding.UTF8.GetBytes("abc")));
            Assert.IsFalse(message.AddTime(Field.MDF_F_DATE, default(string))); 
            Assert.IsFalse(message.AddTime(Field.MDF_F_DATE, default(ReadOnlySpan<byte>)));
            Assert.IsFalse(message.AddTime(Field.MDF_F_DATE, string.Empty));
            Assert.IsFalse(message.AddTime(Field.MDF_F_DATE, s_empty));

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
            string[] values = new string[]
            {
                "28",
                "28 28",
                "28 28 343",
                "=28",
                "=28 28",
                "=28 28 343",
                "+28",
                "+28 28",
                "+28 28 343",
                "-28",
                "-28 28",
                "-28 28 343"
            };

            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_BASICDATA));
            foreach (string value in values)
            {
                Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, value));
                Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, Encoding.UTF8.GetBytes(value + char.MinValue)));
                Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, Encoding.UTF8.GetBytes(value)));
            }
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, default(string)));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, default(ReadOnlySpan<byte>)));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, string.Empty));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, s_empty));
        }

        [TestMethod]
        public void ResetTest()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.AreEqual(1, message.ActiveCount);
            Assert.AreEqual(0, message.FieldCount);
            message.Reset();
            Assert.AreEqual(0, message.ActiveCount);
            Assert.AreEqual(0, message.FieldCount);
        }

        [TestMethod]
        public void DeleteTest()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_BASICDATA));
            Assert.AreEqual(1, message.ActiveCount);
            Assert.AreEqual(1, message.FieldCount);
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.AreEqual(2, message.ActiveCount);
            Assert.AreEqual(0, message.FieldCount);
            message.Delete();
            Assert.AreEqual(1, message.ActiveCount);
            Assert.AreEqual(1, message.FieldCount);
            message.Delete();
            Assert.AreEqual(0, message.ActiveCount);
            Assert.AreEqual(0, message.FieldCount);
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
                "46--X-B--P-3--"
            };

            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            foreach (string @string in strings)
            {
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, @string));
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, @string, @string.Length));
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, @string, @string.Length + 5));
                Assert.IsFalse(message.AddString(Field.MDF_F_LANGUAGE, @string, -1));

                byte[] bytes = Encoding.UTF8.GetBytes(@string + char.MinValue);
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, bytes));
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, bytes, @string.Length));
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, bytes, @string.Length + 5));
                Assert.IsFalse(message.AddString(Field.MDF_F_LANGUAGE, bytes, -1));
                
                bytes = Encoding.UTF8.GetBytes(@string);
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, bytes));
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, bytes, @string.Length));
                Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, bytes, @string.Length + 5));
                Assert.IsFalse(message.AddString(Field.MDF_F_LANGUAGE, bytes, -1));
            }

            Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, default(string)));
            Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, string.Empty));
            Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, string.Empty, 0));
            Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, string.Empty, 5));
            Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, default(string), -1));
            Assert.IsFalse(message.AddString(Field.MDF_F_LANGUAGE, string.Empty, -1));

            Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, default(ReadOnlySpan<byte>)));
            Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, default(ReadOnlySpan<byte>), -1));
            Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, s_empty));
            Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, s_empty, 0));
            Assert.IsTrue(message.AddString(Field.MDF_F_LANGUAGE, s_empty, 5));
            Assert.IsFalse(message.AddString(Field.MDF_F_LANGUAGE, s_empty, -1));
        }

        [TestMethod]
        public void MoveTest()
        {
            using Message source = new Message();
            const ulong SourceInsRef = 1;
            Assert.IsTrue(source.Add(SourceInsRef, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(source.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.AreEqual(1, source.ActiveCount);
            Assert.AreEqual(1, source.FieldCount);
            Assert.IsTrue(source.Add(SourceInsRef, MessageReference.MDF_M_BASICDATA));
            Assert.IsTrue(source.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.AreEqual(2, source.ActiveCount);
            Assert.AreEqual(1, source.FieldCount);
            Assert.IsTrue(source.Add(SourceInsRef, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(source.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.AreEqual(3, source.ActiveCount);
            Assert.AreEqual(1, source.FieldCount);

            using Message destination = new Message();
            Assert.AreEqual(0, destination.ActiveCount);
            Assert.AreEqual(0, destination.FieldCount);

            Assert.IsTrue(Message.Move(source, destination, 1, 10));
            Assert.AreEqual(0, source.ActiveCount);
            Assert.AreEqual(0, source.FieldCount);
            Assert.AreEqual(3, destination.ActiveCount);
            Assert.AreEqual(1, destination.FieldCount);

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
            Assert.AreEqual(1, message2.FieldCount);
            Assert.IsTrue(message.Deserialize(s));
            message2.Reset();
            Assert.AreEqual(0, message2.ActiveCount);
            Assert.AreEqual(0, message2.FieldCount);
            Assert.IsTrue(message2.Deserialize(result));
            Assert.AreEqual(2, message2.ActiveCount);
            Assert.AreEqual(1, message2.FieldCount);
            message2.Reset();
            Assert.AreEqual(0, message2.ActiveCount);
            Assert.AreEqual(0, message2.FieldCount);
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            Assert.IsTrue(message2.Deserialize(bytes));
            Assert.AreEqual(2, message2.ActiveCount);
            Assert.AreEqual(1, message2.FieldCount);
        }
    }
}