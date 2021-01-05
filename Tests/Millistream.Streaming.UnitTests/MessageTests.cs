using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace Millistream.Streaming.UnitTests
{
    [TestClass]
    public sealed class MessageTests
    {
        private delegate void mdf_message_serialize_callback(IntPtr messageHandle, ref IntPtr result);

        [TestMethod]
        public void GetAndSetCompressionLevelTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_message_set_compression_level(It.IsAny<IntPtr>(), It.IsAny<int>())).Returns(1);

            using Message message = new Message(nativeImplementation.Object);
            //assert that the default value is Z_BEST_SPEED
            Assert.AreEqual(CompressionLevel.Z_BEST_SPEED, message.CompressionLevel);

            //assert that the property uses the native implementation to set the compression level
            foreach (CompressionLevel compressionLevel in Enum.GetValues(typeof(CompressionLevel)))
            {
                message.CompressionLevel = compressionLevel;
                nativeImplementation.Verify(x => x.mdf_message_set_compression_level(It.IsAny<IntPtr>(), (int)compressionLevel));
                Assert.AreEqual(compressionLevel, message.CompressionLevel);
            }
        }

        [TestMethod]
        public void GetCountTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            const int Num = 11;
            nativeImplementation.Setup(x => x.mdf_message_get_num(It.IsAny<IntPtr>())).Returns(Num);
            using Message message = new Message(nativeImplementation.Object);
            Assert.AreEqual(Num, message.Count);
        }

        [TestMethod]
        public void GetCountActiveTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            const int NumActive = 5;
            nativeImplementation.Setup(x => x.mdf_message_get_num_active(It.IsAny<IntPtr>())).Returns(NumActive);
            using Message message = new Message(nativeImplementation.Object);
            Assert.AreEqual(NumActive, message.ActiveCount);
        }

        [TestMethod]
        public void AddTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_message_add(It.IsAny<IntPtr>(), It.IsAny<ulong>(), It.IsAny<int>())).Returns(1);

            const ulong instrumentReference = 10;
            MessageReference messageReference = MessageReference.MDF_M_REQUEST;
            using Message message = new Message(nativeImplementation.Object);
            Assert.IsTrue(message.Add(instrumentReference, messageReference));
            Assert.IsTrue(message.Add(instrumentReference, (int)messageReference));
            nativeImplementation.Verify(x => x.mdf_message_add(It.IsAny<IntPtr>(), instrumentReference, (int)messageReference), Times.Exactly(2));
        }

        [TestMethod]
        public void AddNumericTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_message_add_numeric(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>())).Returns(1);

            const Field Field = Field.MDF_F_REQUESTTYPE;
            const string Value = "1.1";
            using Message message = new Message(nativeImplementation.Object);
            Assert.IsTrue(message.AddNumeric(Field, Value));
            Assert.IsTrue(message.AddNumeric((uint)Field, Value));
            nativeImplementation.Verify(x => x.mdf_message_add_numeric(It.IsAny<IntPtr>(), (uint)Field, Value), Times.Exactly(2));
        }

        [TestMethod]
        public void AddInt64AndUInt64Test()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_message_add_int(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<long>(), It.IsAny<int>())).Returns(1);
            nativeImplementation.Setup(x => x.mdf_message_add_uint(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<ulong>(), It.IsAny<int>())).Returns(1);

            const Field Field = Field.MDF_F_LASTPRICE;
            const long SignedValue = 12345;
            const ulong UnsignedValue = 67890;
            int decimals = 2;
            using Message message = new Message(nativeImplementation.Object);
            Assert.IsTrue(message.AddInt64(Field, SignedValue, decimals));
            Assert.IsTrue(message.AddInt64((uint)Field, SignedValue, decimals));
            Assert.IsTrue(message.AddUInt64(Field, UnsignedValue, decimals));
            Assert.IsTrue(message.AddUInt64((uint)Field, UnsignedValue, decimals));
            nativeImplementation.Verify(x => x.mdf_message_add_int(It.IsAny<IntPtr>(), (uint)Field, SignedValue, decimals), Times.Exactly(2));
            nativeImplementation.Verify(x => x.mdf_message_add_uint(It.IsAny<IntPtr>(), (uint)Field, UnsignedValue, decimals), Times.Exactly(2));

            decimals = 0;
            Assert.IsTrue(message.AddInt64(Field, SignedValue, decimals));
            Assert.IsTrue(message.AddInt64((uint)Field, SignedValue, decimals));
            Assert.IsTrue(message.AddUInt64(Field, UnsignedValue, decimals));
            Assert.IsTrue(message.AddUInt64((uint)Field, UnsignedValue, decimals));
            nativeImplementation.Verify(x => x.mdf_message_add_int(It.IsAny<IntPtr>(), (uint)Field, SignedValue, decimals), Times.Exactly(2));
            nativeImplementation.Verify(x => x.mdf_message_add_uint(It.IsAny<IntPtr>(), (uint)Field, UnsignedValue, decimals), Times.Exactly(2));


            decimals = 19;
            Assert.IsTrue(message.AddInt64(Field, SignedValue, decimals));
            Assert.IsTrue(message.AddInt64((uint)Field, SignedValue, decimals));
            Assert.IsTrue(message.AddUInt64(Field, UnsignedValue, decimals));
            Assert.IsTrue(message.AddUInt64((uint)Field, UnsignedValue, decimals));
            nativeImplementation.Verify(x => x.mdf_message_add_int(It.IsAny<IntPtr>(), (uint)Field, SignedValue, decimals), Times.Exactly(2));
            nativeImplementation.Verify(x => x.mdf_message_add_uint(It.IsAny<IntPtr>(), (uint)Field, UnsignedValue, decimals), Times.Exactly(2));

            TestWithAnInvalidNumberOfDecimals(Field, SignedValue, -1, message.AddInt64);
            TestWithAnInvalidNumberOfDecimals((uint)Field, SignedValue, -1, message.AddInt64);

            TestWithAnInvalidNumberOfDecimals(Field, UnsignedValue, -1, message.AddUInt64);
            TestWithAnInvalidNumberOfDecimals((uint)Field, UnsignedValue, -1, message.AddUInt64);

            TestWithAnInvalidNumberOfDecimals(Field, SignedValue, 20, message.AddInt64);
            TestWithAnInvalidNumberOfDecimals((uint)Field, SignedValue, 20, message.AddInt64);

            TestWithAnInvalidNumberOfDecimals(Field, UnsignedValue, 20, message.AddUInt64);
            TestWithAnInvalidNumberOfDecimals((uint)Field, UnsignedValue, 20, message.AddUInt64);

            static void TestWithAnInvalidNumberOfDecimals<TField, TValue>(TField field, TValue value, int decimals, Func<TField, TValue, int, bool> method)
            {
                try
                {
                    method(field, value, decimals);
                    Assert.Fail();
                }
                catch (ArgumentException) { }
            }
        }

        [TestMethod]
        public void AddStringTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_message_add_string(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>())).Returns(1);

            const Field Field = Field.MDF_F_REQUESTID;
            const string Value = "...";
            using Message message = new Message(nativeImplementation.Object);
            Assert.IsTrue(message.AddString(Field, Value));
            Assert.IsTrue(message.AddString((uint)Field, Value));
            nativeImplementation.Verify(x => x.mdf_message_add_string(It.IsAny<IntPtr>(), (uint)Field, Value), Times.Exactly(2));
        }


        [TestMethod]
        public void AddDateTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_message_add_date(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>())).Returns(1);
            nativeImplementation.Setup(x => x.mdf_message_add_date2(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);

            const Field Field = Field.MDF_F_REQUESTID;
            const string Value = "2020-12-06";
            using Message message = new Message(nativeImplementation.Object);
            Assert.IsTrue(message.AddDate(Field, Value));
            Assert.IsTrue(message.AddDate((uint)Field, Value));
            nativeImplementation.Verify(x => x.mdf_message_add_date(It.IsAny<IntPtr>(), (uint)Field, Value), Times.Exactly(2));

            const int Year = 2020;
            const int Month = 12;
            const int Day = 6;
            Assert.IsTrue(message.AddDate(Field, Year, Month, Day));
            Assert.IsTrue(message.AddDate((uint)Field, Year, Month, Day));
            nativeImplementation.Verify(x => x.mdf_message_add_date2(It.IsAny<IntPtr>(), (uint)Field, Year, Month, Day), Times.Exactly(2));
        }

        [TestMethod]
        public void AddTimeTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_message_add_time(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>())).Returns(1);
            nativeImplementation.Setup(x => x.mdf_message_add_time2(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);
            nativeImplementation.Setup(x => x.mdf_message_add_time3(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);

            const Field Field = Field.MDF_F_TIME;
            const string Value = "11:11:11";
            using Message message = new Message(nativeImplementation.Object);
            Assert.IsTrue(message.AddTime(Field, Value));
            Assert.IsTrue(message.AddTime((uint)Field, Value));
            nativeImplementation.Verify(x => x.mdf_message_add_time(It.IsAny<IntPtr>(), (uint)Field, Value), Times.Exactly(2));

            int hour = 12;
            int minute = 13;
            int second = 14;
            const int Millisecond = 999;
            Assert.IsTrue(message.AddTime2(Field, hour, minute, second, Millisecond));
            Assert.IsTrue(message.AddTime2((uint)Field, hour, minute, second, Millisecond));
            nativeImplementation.Verify(x => x.mdf_message_add_time2(It.IsAny<IntPtr>(), (uint)Field, hour, minute, second, Millisecond), Times.Exactly(2));

            hour = 12;
            minute = 13;
            second = 14;
            const int Nanosecond = 999;
            Assert.IsTrue(message.AddTime3(Field, hour, minute, second, Nanosecond));
            Assert.IsTrue(message.AddTime3((uint)Field, hour, minute, second, Nanosecond));
            nativeImplementation.Verify(x => x.mdf_message_add_time3(It.IsAny<IntPtr>(), (uint)Field, hour, minute, second, Nanosecond), Times.Exactly(2));
        }

        [TestMethod]
        public void AddListTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_message_add_list(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>())).Returns(1);
            nativeImplementation.Setup(x => x.mdf_message_add_string(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>())).Returns(1);

            const Field Field = Field.MDF_F_INSREFLIST;
            const string Value = "1 4";
            using Message message = new Message(nativeImplementation.Object);
            Assert.IsTrue(message.AddList(Field, Value));
            Assert.IsTrue(message.AddList((uint)Field, Value));
            nativeImplementation.Verify(x => x.mdf_message_add_list(It.IsAny<IntPtr>(), (uint)Field, Value), Times.Exactly(2));

            ulong[] instrumentReferences = new ulong[4] { 1, 2, 33, 4 };
            Assert.IsTrue(message.AddList(Field, instrumentReferences));
            Assert.IsTrue(message.AddList((uint)Field, instrumentReferences));
            nativeImplementation.Verify(x => x.mdf_message_add_list(It.IsAny<IntPtr>(), (uint)Field, "1 2 33 4"), Times.Exactly(2));

            RequestClass[] requestClasses = new RequestClass[2] { RequestClass.MDF_RC_BASICDATA, RequestClass.MDF_RC_QUOTE };
            Assert.IsTrue(message.AddList(requestClasses));
            nativeImplementation.Verify(x => x.mdf_message_add_list(It.IsAny<IntPtr>(), (uint)Field.MDF_F_REQUESTCLASS, "4 1"));

            requestClasses = new RequestClass[0];
            Assert.IsTrue(message.AddList(requestClasses));
        }

        [TestMethod]
        public void ResetTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            using Message message = new Message(nativeImplementation.Object);
            message.Reset();
            nativeImplementation.Verify(x => x.mdf_message_reset(It.IsAny<IntPtr>()));
        }

        [TestMethod]
        public void DeleteTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_message_del(It.IsAny<IntPtr>())).Returns(1);
            using Message message = new Message(nativeImplementation.Object);
            Assert.IsTrue(message.Delete());
            nativeImplementation.Verify(x => x.mdf_message_del(It.IsAny<IntPtr>()));
        }

        [TestMethod]
        public void SerializeTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            IntPtr messageHandle = new IntPtr(123);
            nativeImplementation.Setup(x => x.mdf_message_create()).Returns(messageHandle);
            IntPtr stringPointer = new IntPtr(456);
            nativeImplementation.Setup(x => x.mdf_message_serialize(messageHandle, ref It.Ref<IntPtr>.IsAny))
                .Returns(1)
                .Callback(new mdf_message_serialize_callback((IntPtr messageHandle, ref IntPtr result) => result = stringPointer))
                .Verifiable();
            using Message message = new Message(nativeImplementation.Object);
            Assert.IsTrue(message.Serialize(out IntPtr result));
            Assert.AreEqual(stringPointer, result);
            nativeImplementation.Verify();

            nativeImplementation.Setup(x => x.mdf_message_serialize(messageHandle, ref It.Ref<IntPtr>.IsAny)).Returns(0).Verifiable();
            Assert.IsFalse(message.Serialize(out result));
            Assert.AreEqual(IntPtr.Zero, result);
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void DeserializeTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            IntPtr messageHandle = new IntPtr(123);
            nativeImplementation.Setup(x => x.mdf_message_create()).Returns(messageHandle);
            const string Data = "ABC";
            nativeImplementation.Setup(x => x.mdf_message_deserialize(messageHandle, Data)).Returns(1).Verifiable();
            using Message message = new Message(nativeImplementation.Object);
            Assert.IsTrue(message.Deserialize(Data));
            nativeImplementation.Verify();

            nativeImplementation.Setup(x => x.mdf_message_deserialize(messageHandle, Data)).Returns(0).Verifiable();
            Assert.IsFalse(message.Deserialize(Data));
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void DisposeTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            using (Message message = new Message(nativeImplementation.Object)) { }
            nativeImplementation.Verify(x => x.mdf_message_destroy(It.IsAny<IntPtr>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullNumericTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddNumeric(Field.MDF_F_LASTPRICE, null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEmptyNumericTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddNumeric(Field.MDF_F_LASTPRICE, string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullStringTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddString(Field.MDF_F_REQUESTID, null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEmptyStringTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddString(Field.MDF_F_REQUESTID, string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullDateTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddDate(Field.MDF_F_DATE, null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEmptyDateTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddDate(Field.MDF_F_DATE, string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullTimeTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddTime(Field.MDF_F_TIME, null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEmptyTimeTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddTime(Field.MDF_F_TIME, string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullListTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddList(Field.MDF_F_INSREFLIST, default(string));

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEmptyListTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddList(Field.MDF_F_INSREFLIST, string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullInstrumentReferenceListTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddList(Field.MDF_F_INSREFLIST, default(IEnumerable<ulong>));

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullReferenceClassesListTest() =>
            new Message(new Mock<INativeImplementation>().Object).AddList(null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeserializeNullReferenceTest() =>
            new Message(new Mock<INativeImplementation>().Object).Deserialize(null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeserializeEmptyStringTest() =>
            new Message(new Mock<INativeImplementation>().Object).Deserialize(string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetCompressionLevelAfterDisposeTest() => _ = GetDisposedMessage().CompressionLevel;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetCompressionLevelAfterDisposeTest() => GetDisposedMessage().CompressionLevel = CompressionLevel.Z_BEST_COMPRESSION;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetCountAfterDisposeTest() => _ = GetDisposedMessage().Count;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetActiveCountAfterDisposeTest() => _ = GetDisposedMessage().ActiveCount;

        [TestMethod]
        public void CannotCallMethodsAfterDisposeTest()
        {
            Message disposedMessage = GetDisposedMessage();

            CatchObjectDisposedException(() => disposedMessage.Add(1, 1));
            CatchObjectDisposedException(() => disposedMessage.Add(1, MessageReference.MDF_M_REQUEST));

            CatchObjectDisposedException(() => disposedMessage.AddNumeric(1, "1"));
            CatchObjectDisposedException(() => disposedMessage.AddNumeric(Field.MDF_F_AVERAGE, "1.1"));

            CatchObjectDisposedException(() => disposedMessage.AddInt64(1, -12345, 2));
            CatchObjectDisposedException(() => disposedMessage.AddInt64(Field.MDF_F_AVERAGE, -12345, 2));

            CatchObjectDisposedException(() => disposedMessage.AddUInt64(1, 12345, 2));
            CatchObjectDisposedException(() => disposedMessage.AddUInt64(Field.MDF_F_AVERAGE, 12345, 2));

            CatchObjectDisposedException(() => disposedMessage.AddString(1, "abc"));
            CatchObjectDisposedException(() => disposedMessage.AddString(Field.MDF_F_REQUESTID, "abc"));

            CatchObjectDisposedException(() => disposedMessage.AddDate(1, "2020-12-30"));
            CatchObjectDisposedException(() => disposedMessage.AddDate(Field.MDF_F_DATE, "2020-12-30"));
            CatchObjectDisposedException(() => disposedMessage.AddDate(1, 2020, 12, 30));
            CatchObjectDisposedException(() => disposedMessage.AddDate(Field.MDF_F_DATE, 2020, 12, 30));

            CatchObjectDisposedException(() => disposedMessage.AddTime(1, "16:30:30"));
            CatchObjectDisposedException(() => disposedMessage.AddTime(Field.MDF_F_TIME, "16:30:30"));

            CatchObjectDisposedException(() => disposedMessage.AddTime2(1, 16, 30, 30, 999));
            CatchObjectDisposedException(() => disposedMessage.AddTime2(Field.MDF_F_TIME, 16, 30, 30, 999));

            CatchObjectDisposedException(() => disposedMessage.AddTime3(1, 16, 30, 30, 999999999));
            CatchObjectDisposedException(() => disposedMessage.AddTime3(Field.MDF_F_TIME, 16, 30, 30, 999999999));

            CatchObjectDisposedException(() => disposedMessage.AddList(1, "1"));
            CatchObjectDisposedException(() => disposedMessage.AddList(Field.MDF_F_INSREFLIST, "1"));
            CatchObjectDisposedException(() => disposedMessage.AddList(1, new ulong[1] { 1 }));
            CatchObjectDisposedException(() => disposedMessage.AddList(Field.MDF_F_INSREFLIST, new ulong[1] { 1 }));
            CatchObjectDisposedException(() => disposedMessage.AddList(new RequestClass[1] { RequestClass.MDF_RC_BASICDATA }));

            CatchObjectDisposedException(() => disposedMessage.Reset());

            CatchObjectDisposedException(() => disposedMessage.Delete());

            CatchObjectDisposedException(() => disposedMessage.Serialize(out IntPtr _));

            CatchObjectDisposedException(() => disposedMessage.Deserialize("ABC"));

        }

        private static Message GetDisposedMessage()
        {
            Message message = new Message(new Mock<INativeImplementation>().Object);
            message.Dispose();
            return message;
        }

        private static void CatchObjectDisposedException(Action action)
        {
            try
            {
                action();
                Assert.Fail();
            }
            catch (ObjectDisposedException) { }
        }
    }
}