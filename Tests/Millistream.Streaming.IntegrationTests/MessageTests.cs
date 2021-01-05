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
        public void AddInt64Test()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddInt64(Field.MDF_F_QUANTITY, -12345, 2));
        }

        [TestMethod]
        public void AddUInt64Test()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddUInt64(Field.MDF_F_QUANTITY, 12345, 2));
        }

        [TestMethod]
        public void AddDateTest()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddDate(Field.MDF_F_DATE, 2020, 12, 30));
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
            Assert.IsFalse(message.AddTime(Field.MDF_F_DATE, "abc"));

            Assert.IsTrue(message.AddTime2(Field.MDF_F_DATE, 17, 3, 1, 999));
            Assert.IsFalse(message.AddTime2(Field.MDF_F_DATE, 33, 3, 1, 999));

            Assert.IsTrue(message.AddTime3(Field.MDF_F_DATE, 17, 3, 1, 999999999));
            Assert.IsTrue(message.AddTime3(Field.MDF_F_DATE, 17, 3, 1, 999));
            Assert.IsFalse(message.AddTime3(Field.MDF_F_DATE, 17, 88, 1, 999));
        }

        [TestMethod]
        public void DeleteTest()
        {
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTCLASS, ((uint)RequestClass.MDF_RC_BASICDATA).ToString()));
            Assert.AreEqual(1, message.ActiveCount);
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_QUOTE));
            Assert.AreEqual(2, message.ActiveCount);
            message.Delete();
            Assert.AreEqual(1, message.ActiveCount);
            message.Delete();
            Assert.AreEqual(0, message.ActiveCount);
        }

        [TestMethod]
        public void MoveTest()
        {
            using Message source = new Message();
            const ulong SourceInsRef = 1;
            Assert.IsTrue(source.Add(SourceInsRef, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(source.AddNumeric(Field.MDF_F_REQUESTTYPE, "1"));
            Assert.IsTrue(source.Add(SourceInsRef, MessageReference.MDF_M_BASICDATA));
            Assert.IsTrue(source.AddNumeric(Field.MDF_F_REQUESTTYPE, "1"));
            Assert.IsTrue(source.Add(SourceInsRef, MessageReference.MDF_M_QUOTE));
            Assert.IsTrue(source.AddNumeric(Field.MDF_F_REQUESTTYPE, "1"));
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
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, "1"));
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