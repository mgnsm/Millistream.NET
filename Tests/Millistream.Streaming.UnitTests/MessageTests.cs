using Microsoft.VisualStudio.TestTools.UnitTesting;
using Millistream.Streaming.Interop;
using Moq;
using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;

namespace Millistream.Streaming.UnitTests
{
    [TestClass]
    public sealed class MessageTests
    {
        private readonly Mock<INativeImplementation> _nativeImplementation = new();
        private delegate void mdf_message_serialize_callback(IntPtr messageHandle, ref IntPtr result);

        public MessageTests() => NativeImplementation.Implementation = _nativeImplementation.Object;

        [TestMethod]
        public void CreateMessageTest()
        {
            int expectedInstanceCount = NativeImplementation.InstanceCount == 0 ? 1 : NativeImplementation.InstanceCount;
            using Message message = new();
            using Message message2 = new();
            using Message message3 = new();
            using Message message4 = new();
            using Message message5 = new();
            Assert.AreEqual(expectedInstanceCount, NativeImplementation.InstanceCount);

            using Message message6 = new("lib");
            using Message message7 = new("lib");
            using Message message8 = new("lib2");
            using Message message9 = new();
            Assert.AreEqual(expectedInstanceCount + 3, NativeImplementation.InstanceCount);
        }

        [TestMethod]
        public void GetAndSetCompressionLevelTest()
        {
            _nativeImplementation.Setup(x => x.mdf_message_set_compression_level(It.IsAny<IntPtr>(), It.IsAny<int>())).Returns(1);

            using Message message = new();
            //assert that the default value is Z_BEST_SPEED
            Assert.AreEqual(CompressionLevel.Z_BEST_SPEED, message.CompressionLevel);

            //assert that the property uses the native implementation to set the compression level
            foreach (CompressionLevel compressionLevel in Enum.GetValues(typeof(CompressionLevel)))
            {
                message.CompressionLevel = compressionLevel;
                _nativeImplementation.Verify(x => x.mdf_message_set_compression_level(It.IsAny<IntPtr>(), (int)compressionLevel));
                Assert.AreEqual(compressionLevel, message.CompressionLevel);
            }
        }

        [TestMethod]
        public void GetCountTest()
        {
            const int Num = 11;
            _nativeImplementation.Setup(x => x.mdf_message_get_num(It.IsAny<IntPtr>())).Returns(Num);
            using Message message = new();
            Assert.AreEqual(Num, message.Count);
        }

        [TestMethod]
        public void GetCountActiveTest()
        {
            const int NumActive = 5;
            _nativeImplementation.Setup(x => x.mdf_message_get_num_active(It.IsAny<IntPtr>())).Returns(NumActive);
            using Message message = new();
            Assert.AreEqual(NumActive, message.ActiveCount);
        }

        [TestMethod]
        public void GetAndSetUtf8ValidationTest()
        {
            _nativeImplementation.Setup(x => x.mdf_message_set_utf8_validation(It.IsAny<IntPtr>(), 0)).Returns(1).Verifiable();
            using Message message = new();
            Assert.AreEqual(true, message.Utf8Validation);
            message.Utf8Validation = false;
            Assert.AreEqual(false, message.Utf8Validation);
            _nativeImplementation.Verify();
            _nativeImplementation.Setup(x => x.mdf_message_set_utf8_validation(It.IsAny<IntPtr>(), 1)).Returns(1).Verifiable();
            message.Utf8Validation = true;
            Assert.AreEqual(true, message.Utf8Validation);
            _nativeImplementation.Verify();
        }

        [TestMethod]
        public void AddTest()
        {
            _nativeImplementation.Setup(x => x.mdf_message_add(It.IsAny<IntPtr>(), It.IsAny<ulong>(), It.IsAny<int>())).Returns(1);

            const ulong instrumentReference = 10;
            MessageReference messageReference = MessageReference.MDF_M_REQUEST;
            using Message message = new();
            Assert.IsTrue(message.Add(instrumentReference, messageReference));
            Assert.IsTrue(message.Add(instrumentReference, (int)messageReference));
            _nativeImplementation.Verify(x => x.mdf_message_add(It.IsAny<IntPtr>(), instrumentReference, (int)messageReference), Times.Exactly(2));
        }

        [TestMethod]
        public void AddNumericTest()
        {
            const Field Field = Field.MDF_F_REQUESTTYPE;
            const string Value = "1.1";

            _nativeImplementation.Setup(x => x.mdf_message_add_numeric(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                .Callback((IntPtr message, uint tag, IntPtr value) => Compare(Value, value))
                .Returns(1);

            using Message message = new();
            Assert.IsTrue(message.AddNumeric(Field, Value));
            Assert.IsTrue(message.AddNumeric((uint)Field, Value));
            _nativeImplementation.Verify(x => x.mdf_message_add_numeric(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(2));
            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            Assert.IsTrue(message.AddNumeric(Field, bytes));
            Assert.IsTrue(message.AddNumeric((uint)Field, bytes));
            _nativeImplementation.Verify(x => x.mdf_message_add_numeric(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(4));
        }

        [TestMethod]
        public void AddInt64AndUInt64Test()
        {
            _nativeImplementation.Setup(x => x.mdf_message_add_int(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<long>(), It.IsAny<int>())).Returns(1);
            _nativeImplementation.Setup(x => x.mdf_message_add_uint(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<ulong>(), It.IsAny<int>())).Returns(1);

            const Field Field = Field.MDF_F_LASTPRICE;
            const long SignedValue = 12345;
            const ulong UnsignedValue = 67890;
            int decimals = 2;
            using Message message = new();
            Assert.IsTrue(message.AddInt64(Field, SignedValue, decimals));
            Assert.IsTrue(message.AddInt64((uint)Field, SignedValue, decimals));
            Assert.IsTrue(message.AddUInt64(Field, UnsignedValue, decimals));
            Assert.IsTrue(message.AddUInt64((uint)Field, UnsignedValue, decimals));
            _nativeImplementation.Verify(x => x.mdf_message_add_int(It.IsAny<IntPtr>(), (uint)Field, SignedValue, decimals), Times.Exactly(2));
            _nativeImplementation.Verify(x => x.mdf_message_add_uint(It.IsAny<IntPtr>(), (uint)Field, UnsignedValue, decimals), Times.Exactly(2));

            decimals = 0;
            Assert.IsTrue(message.AddInt64(Field, SignedValue, decimals));
            Assert.IsTrue(message.AddInt64((uint)Field, SignedValue, decimals));
            Assert.IsTrue(message.AddUInt64(Field, UnsignedValue, decimals));
            Assert.IsTrue(message.AddUInt64((uint)Field, UnsignedValue, decimals));
            _nativeImplementation.Verify(x => x.mdf_message_add_int(It.IsAny<IntPtr>(), (uint)Field, SignedValue, decimals), Times.Exactly(2));
            _nativeImplementation.Verify(x => x.mdf_message_add_uint(It.IsAny<IntPtr>(), (uint)Field, UnsignedValue, decimals), Times.Exactly(2));


            decimals = 19;
            Assert.IsTrue(message.AddInt64(Field, SignedValue, decimals));
            Assert.IsTrue(message.AddInt64((uint)Field, SignedValue, decimals));
            Assert.IsTrue(message.AddUInt64(Field, UnsignedValue, decimals));
            Assert.IsTrue(message.AddUInt64((uint)Field, UnsignedValue, decimals));
            _nativeImplementation.Verify(x => x.mdf_message_add_int(It.IsAny<IntPtr>(), (uint)Field, SignedValue, decimals), Times.Exactly(2));
            _nativeImplementation.Verify(x => x.mdf_message_add_uint(It.IsAny<IntPtr>(), (uint)Field, UnsignedValue, decimals), Times.Exactly(2));

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
            _nativeImplementation.Setup(x => x.mdf_message_add_string(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>())).Returns(1);
            _nativeImplementation.Setup(x => x.mdf_message_add_string2(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>(), It.IsAny<int>())).Returns(1);

            const Field Field = Field.MDF_F_REQUESTID;
            const string Value = "...";
            using Message message = new();
            Assert.IsTrue(message.AddString(Field, Value));
            Assert.IsTrue(message.AddString((uint)Field, Value));

            Assert.IsTrue(message.AddString(Field, Value, Value.Length));
            Assert.IsTrue(message.AddString((uint)Field, Value, Value.Length));

            _nativeImplementation.Verify(x => x.mdf_message_add_string(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(2));
            _nativeImplementation.Verify(x => x.mdf_message_add_string2(message.Handle, (uint)Field, It.IsAny<IntPtr>(), Value.Length), Times.Exactly(2));

            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            Assert.IsTrue(message.AddString(Field, Value));
            Assert.IsTrue(message.AddString((uint)Field, Value));
            _nativeImplementation.Verify(x => x.mdf_message_add_string(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(4));

            Assert.IsTrue(message.AddString(Field, bytes, bytes.Length));
            Assert.IsTrue(message.AddString((uint)Field, bytes, bytes.Length));
            _nativeImplementation.Verify(x => x.mdf_message_add_string2(message.Handle, (uint)Field, It.IsAny<IntPtr>(), Value.Length), Times.Exactly(4));
        }


        [TestMethod]
        public void AddDateTest()
        {
            const Field Field = Field.MDF_F_REQUESTID;
            const string Value = "2020-12-06";

            _nativeImplementation.Setup(x => x.mdf_message_add_date(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                .Callback((IntPtr message, uint tag, IntPtr value) => Compare(Value, value))
                .Returns(1);
            _nativeImplementation.Setup(x => x.mdf_message_add_date2(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);

            using Message message = new();
            Assert.IsTrue(message.AddDate(Field, Value));
            Assert.IsTrue(message.AddDate((uint)Field, Value));
            _nativeImplementation.Verify(x => x.mdf_message_add_date(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(2));
            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            Assert.IsTrue(message.AddDate(Field, bytes));
            Assert.IsTrue(message.AddDate((uint)Field, bytes));
            _nativeImplementation.Verify(x => x.mdf_message_add_date(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(4));

            const int Year = 2020;
            const int Month = 12;
            const int Day = 6;
            Assert.IsTrue(message.AddDate(Field, Year, Month, Day));
            Assert.IsTrue(message.AddDate((uint)Field, Year, Month, Day));
            _nativeImplementation.Verify(x => x.mdf_message_add_date2(message.Handle, (uint)Field, Year, Month, Day), Times.Exactly(2));
        }

        [TestMethod]
        public void AddTimeTest()
        {
            const Field Field = Field.MDF_F_TIME;
            const string Value = "11:11:11";

            _nativeImplementation.Setup(x => x.mdf_message_add_time(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                .Callback((IntPtr message, uint tag, IntPtr value) => Compare(Value, value))
                .Returns(1);
            _nativeImplementation.Setup(x => x.mdf_message_add_time2(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);
            _nativeImplementation.Setup(x => x.mdf_message_add_time3(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);

            using Message message = new();
            Assert.IsTrue(message.AddTime(Field, Value));
            Assert.IsTrue(message.AddTime((uint)Field, Value));
            _nativeImplementation.Verify(x => x.mdf_message_add_time(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(2));

            int hour = 12;
            int minute = 13;
            int second = 14;
            const int Millisecond = 999;
            Assert.IsTrue(message.AddTime2(Field, hour, minute, second, Millisecond));
            Assert.IsTrue(message.AddTime2((uint)Field, hour, minute, second, Millisecond));
            _nativeImplementation.Verify(x => x.mdf_message_add_time2(message.Handle, (uint)Field, hour, minute, second, Millisecond), Times.Exactly(2));

            hour = 12;
            minute = 13;
            second = 14;
            const int Nanosecond = 999;
            Assert.IsTrue(message.AddTime3(Field, hour, minute, second, Nanosecond));
            Assert.IsTrue(message.AddTime3((uint)Field, hour, minute, second, Nanosecond));
            _nativeImplementation.Verify(x => x.mdf_message_add_time3(message.Handle, (uint)Field, hour, minute, second, Nanosecond), Times.Exactly(2));
        }

        [TestMethod]
        public void AddListTest()
        {
            const Field Field = Field.MDF_F_INSREFLIST;
            const string Value = "1 4";

            _nativeImplementation.Setup(x => x.mdf_message_add_list(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                .Callback((IntPtr message, uint tag, IntPtr value) => Compare(Value, value))
                .Returns(1);

            using Message message = new();
            Assert.IsTrue(message.AddList(Field, Value));
            Assert.IsTrue(message.AddList((uint)Field, Value));
            _nativeImplementation.Verify(x => x.mdf_message_add_list(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(2));
            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            Assert.IsTrue(message.AddList(Field, bytes));
            Assert.IsTrue(message.AddList((uint)Field, bytes));
            _nativeImplementation.Verify(x => x.mdf_message_add_list(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(4));
        }

        [TestMethod]
        public void ResetTest()
        {
            using Message message = new();
            message.Reset();
            _nativeImplementation.Verify(x => x.mdf_message_reset(It.IsAny<IntPtr>()));
        }

        [TestMethod]
        public void DeleteTest()
        {
            _nativeImplementation.Setup(x => x.mdf_message_del(It.IsAny<IntPtr>())).Returns(1);
            using Message message = new();
            Assert.IsTrue(message.Delete());
            _nativeImplementation.Verify(x => x.mdf_message_del(It.IsAny<IntPtr>()));
        }

        [TestMethod]
        public void MoveTest()
        {
            const ulong SourceInsref = 1;
            const ulong DestinationInsref = 2;

            _nativeImplementation.Setup(x => x.mdf_message_create()).Returns(new IntPtr(123));
            using Message source = new();

            _nativeImplementation.Setup(x => x.mdf_message_create()).Returns(new IntPtr(456));
            using Message destination = new();

            _nativeImplementation.Setup(x => x.mdf_message_move(source.Handle, destination.Handle, SourceInsref, DestinationInsref)).Returns(1).Verifiable();

            Assert.IsTrue(Message.Move(source, destination, SourceInsref, DestinationInsref));
            _nativeImplementation.Verify();

            _nativeImplementation.Setup(x => x.mdf_message_move(source.Handle, destination.Handle, SourceInsref, DestinationInsref)).Returns(0).Verifiable();
            Assert.IsFalse(Message.Move(source, destination, SourceInsref, DestinationInsref));
            _nativeImplementation.Verify();
        }

        [TestMethod]
        public void SerializeTest()
        {
            IntPtr messageHandle = new(123);
            _nativeImplementation.Setup(x => x.mdf_message_create()).Returns(messageHandle);
            IntPtr stringPointer = new(456);
            _nativeImplementation.Setup(x => x.mdf_message_serialize(messageHandle, ref It.Ref<IntPtr>.IsAny))
                .Returns(1)
                .Callback(new mdf_message_serialize_callback((IntPtr messageHandle, ref IntPtr result) => result = stringPointer))
                .Verifiable();
            using Message message = new();
            Assert.IsTrue(message.Serialize(out IntPtr result));
            Assert.AreEqual(stringPointer, result);
            _nativeImplementation.Verify();

            _nativeImplementation.Setup(x => x.mdf_message_serialize(messageHandle, ref It.Ref<IntPtr>.IsAny)).Returns(0).Verifiable();
            Assert.IsFalse(message.Serialize(out result));
            Assert.AreEqual(IntPtr.Zero, result);
            _nativeImplementation.Verify();
        }

        [TestMethod]
        public void DeserializeTest()
        {
            IntPtr messageHandle = new(123);
            _nativeImplementation.Setup(x => x.mdf_message_create()).Returns(messageHandle);
            const string Data = "ABC";
            Expression<Func<INativeImplementation, int>> expression = x => x.mdf_message_deserialize(messageHandle, It.IsAny<IntPtr>());
            _nativeImplementation.Setup(expression)
                .Returns(1)
                .Callback((IntPtr message, IntPtr data) => Compare(Data, data))
                .Verifiable();
            using Message message = new();
            Assert.IsTrue(message.Deserialize(Data));
            IntPtr p = Marshal.StringToHGlobalAnsi(Data);
            Assert.IsTrue(message.Deserialize(p));
            _nativeImplementation.Verify(expression, Times.Exactly(2));

            _nativeImplementation.Setup(expression).Returns(0);
            Assert.IsFalse(message.Deserialize(Data));
            Assert.IsFalse(message.Deserialize(p));
            _nativeImplementation.Verify(expression, Times.Exactly(4));
        }

        [TestMethod]
        public void DisposeTest()
        {
            using (Message message = new()) { }
            _nativeImplementation.Verify(x => x.mdf_message_destroy(It.IsAny<IntPtr>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateMessageWithNoNativeLibraryPathTest () => new Message(default);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateMessageWithEmptyNativeLibraryPathTest() => new Message(string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MoveNullReferenceTest() =>
            Message.Move(null, new(), 1, 2);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeserializeNullReferenceTest() =>
            new Message().Deserialize(null);

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeserializeEmptyStringTest() =>
            new Message().Deserialize(string.Empty);

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
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetUtf8ValidationAfterDisposeTest() => _ = GetDisposedMessage().Utf8Validation;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetUtf8ValidationAfterDisposeTest() => GetDisposedMessage().Utf8Validation = false;

        [TestMethod]
        public void CannotCallMethodsAfterDisposeTest()
        {
            Message disposedMessage = GetDisposedMessage();

            CatchObjectDisposedException(() => disposedMessage.Add(1, 1));
            CatchObjectDisposedException(() => disposedMessage.Add(1, MessageReference.MDF_M_REQUEST));

            CatchObjectDisposedException(() => disposedMessage.AddNumeric(1, StringConstants.RequestTypes.MDF_RT_IMAGE));
            CatchObjectDisposedException(() => disposedMessage.AddNumeric(Field.MDF_F_AVERAGE, "1.1"));
            byte[] bytes = Encoding.UTF8.GetBytes("1.1");
            CatchObjectDisposedException(() => disposedMessage.AddNumeric(1, bytes));
            CatchObjectDisposedException(() => disposedMessage.AddNumeric(Field.MDF_F_AVERAGE, bytes));

            CatchObjectDisposedException(() => disposedMessage.AddInt64(1, -12345, 2));
            CatchObjectDisposedException(() => disposedMessage.AddInt64(Field.MDF_F_AVERAGE, -12345, 2));

            CatchObjectDisposedException(() => disposedMessage.AddUInt64(1, 12345, 2));
            CatchObjectDisposedException(() => disposedMessage.AddUInt64(Field.MDF_F_AVERAGE, 12345, 2));

            CatchObjectDisposedException(() => disposedMessage.AddString(1, "abc"));
            CatchObjectDisposedException(() => disposedMessage.AddString(1, "abc", 1));
            CatchObjectDisposedException(() => disposedMessage.AddString(Field.MDF_F_REQUESTID, "abc"));
            CatchObjectDisposedException(() => disposedMessage.AddString(Field.MDF_F_REQUESTID, "abc", 1));
            bytes = Encoding.UTF8.GetBytes("abc");
            CatchObjectDisposedException(() => disposedMessage.AddString(1, bytes));
            CatchObjectDisposedException(() => disposedMessage.AddString(1, bytes, 1));
            CatchObjectDisposedException(() => disposedMessage.AddString(Field.MDF_F_REQUESTID, bytes));
            CatchObjectDisposedException(() => disposedMessage.AddString(Field.MDF_F_REQUESTID, bytes, 1));


            CatchObjectDisposedException(() => disposedMessage.AddDate(1, "2020-12-30"));
            CatchObjectDisposedException(() => disposedMessage.AddDate(Field.MDF_F_DATE, "2020-12-30"));
            CatchObjectDisposedException(() => disposedMessage.AddDate(1, 2020, 12, 30));
            CatchObjectDisposedException(() => disposedMessage.AddDate(Field.MDF_F_DATE, 2020, 12, 30));
            bytes = Encoding.UTF8.GetBytes("2020-12-30");
            CatchObjectDisposedException(() => disposedMessage.AddDate(1, bytes));
            CatchObjectDisposedException(() => disposedMessage.AddDate(Field.MDF_F_DATE, bytes));

            CatchObjectDisposedException(() => disposedMessage.AddTime(1, "16:30:30"));
            CatchObjectDisposedException(() => disposedMessage.AddTime(Field.MDF_F_TIME, "16:30:30"));
            bytes = Encoding.UTF8.GetBytes("16:30:30");
            CatchObjectDisposedException(() => disposedMessage.AddTime(1, bytes));
            CatchObjectDisposedException(() => disposedMessage.AddTime(Field.MDF_F_TIME, bytes));

            CatchObjectDisposedException(() => disposedMessage.AddTime2(1, 16, 30, 30, 999));
            CatchObjectDisposedException(() => disposedMessage.AddTime2(Field.MDF_F_TIME, 16, 30, 30, 999));

            CatchObjectDisposedException(() => disposedMessage.AddTime3(1, 16, 30, 30, 999999999));
            CatchObjectDisposedException(() => disposedMessage.AddTime3(Field.MDF_F_TIME, 16, 30, 30, 999999999));

            CatchObjectDisposedException(() => disposedMessage.AddList(1, "1"));
            CatchObjectDisposedException(() => disposedMessage.AddList(Field.MDF_F_INSREFLIST, "1"));
            bytes = Encoding.UTF8.GetBytes("1");
            CatchObjectDisposedException(() => disposedMessage.AddList(1, bytes));
            CatchObjectDisposedException(() => disposedMessage.AddList(Field.MDF_F_INSREFLIST, bytes));

            CatchObjectDisposedException(() => disposedMessage.Reset());

            CatchObjectDisposedException(() => disposedMessage.Delete());

            CatchObjectDisposedException(() => disposedMessage.Serialize(out IntPtr _));

            CatchObjectDisposedException(() => disposedMessage.Deserialize("ABC"));
            CatchObjectDisposedException(() => disposedMessage.Deserialize(new IntPtr(123)));
        }

        private static void Compare(string expectedValue, IntPtr actualValue)
        {
            byte[] bytes = new byte[expectedValue.Length];
            Marshal.Copy(actualValue, bytes, 0, expectedValue.Length);
            Assert.AreEqual(expectedValue, Encoding.ASCII.GetString(bytes));
        }

        private static Message GetDisposedMessage()
        {
            Message message = new();
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