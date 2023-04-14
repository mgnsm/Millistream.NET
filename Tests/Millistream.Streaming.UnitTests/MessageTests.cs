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
            _nativeImplementation.Setup(x => x.mdf_message_set_property(It.IsAny<IntPtr>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);

            NativeImplementation nativeImplementation = new(default);
            using Message message = new(nativeImplementation);
            //assert that the default value is Z_BEST_SPEED
            Assert.AreEqual(CompressionLevel.Z_BEST_SPEED, message.CompressionLevel);

            //assert that the property uses the native implementation to set the compression level
            foreach (CompressionLevel compressionLevel in Enum.GetValues(typeof(CompressionLevel)))
            {
                message.CompressionLevel = compressionLevel;
                _nativeImplementation.Verify(x => x.mdf_message_set_property(It.IsAny<IntPtr>(), (int)MDF_MSG_OPTION.MDF_MSG_OPT_COMPRESSION, (int)compressionLevel));
                Assert.AreEqual(compressionLevel, message.CompressionLevel);
            }

            //assert that the libmdf-1.0.25 fallback is used as expected
            _nativeImplementation.Reset();
            _nativeImplementation.Setup(x => x.mdf_message_set_compression_level(It.IsAny<IntPtr>(), It.IsAny<int>())).Returns(1);
            unsafe
            {
                nativeImplementation.mdf_message_set_property = default;
            }

            foreach (CompressionLevel compressionLevel in Enum.GetValues(typeof(CompressionLevel)))
            {
                message.CompressionLevel = compressionLevel;
                _nativeImplementation.Verify(x => x.mdf_message_set_property(It.IsAny<IntPtr>(), (int)MDF_MSG_OPTION.MDF_MSG_OPT_COMPRESSION, (int)compressionLevel), Times.Never);
                _nativeImplementation.Verify(x => x.mdf_message_set_compression_level(It.IsAny<IntPtr>(), (int)compressionLevel), Times.Once);
                Assert.AreEqual(compressionLevel, message.CompressionLevel);
            }
        }

        [TestMethod]
        public void GetCountTest()
        {
            const int Count = 11;
            _nativeImplementation.Setup(x => x.mdf_message_get_num(It.IsAny<IntPtr>())).Returns(Count);
            using Message message = new();
            Assert.AreEqual(Count, message.Count);
        }

        [TestMethod]
        public void GetActiveCountTest()
        {
            const int Count = 5;
            _nativeImplementation.Setup(x => x.mdf_message_get_num_active(It.IsAny<IntPtr>())).Returns(Count);
            using Message message = new();
            Assert.AreEqual(Count, message.ActiveCount);
        }

        [TestMethod]
        public void GetFieldCountTest()
        {
            const int Count = 6;
            _nativeImplementation.Setup(x => x.mdf_message_get_num_fields(It.IsAny<IntPtr>())).Returns(Count);
            using Message message = new();
            Assert.AreEqual(Count, message.FieldCount);
        }

        [TestMethod]
        public void GetAndSetUtf8ValidationTest()
        {
            _nativeImplementation.Setup(x => x.mdf_message_set_property(It.IsAny<IntPtr>(), (int)MDF_MSG_OPTION.MDF_MSG_OPT_UTF8, 0)).Returns(1).Verifiable();
            NativeImplementation nativeImplementation = new(default);
            using Message message = new(nativeImplementation);
            Assert.AreEqual(true, message.Utf8Validation);
            message.Utf8Validation = false;
            Assert.IsFalse(message.Utf8Validation);
            _nativeImplementation.Verify();
            _nativeImplementation.Setup(x => x.mdf_message_set_property(It.IsAny<IntPtr>(), (int)MDF_MSG_OPTION.MDF_MSG_OPT_UTF8, 1)).Returns(1).Verifiable();
            message.Utf8Validation = true;
            Assert.IsTrue(message.Utf8Validation);
            _nativeImplementation.Verify();
            _nativeImplementation.Reset();

            unsafe
            {
                nativeImplementation.mdf_message_set_property = default;
            }
            _nativeImplementation.Setup(x => x.mdf_message_set_utf8_validation(It.IsAny<IntPtr>(), 1)).Returns(1).Verifiable();
            _nativeImplementation.Setup(x => x.mdf_message_set_utf8_validation(It.IsAny<IntPtr>(), 0)).Returns(1).Verifiable();

            message.Utf8Validation = false;
            Assert.IsFalse(message.Utf8Validation);
            message.Utf8Validation = true;
            Assert.IsTrue(message.Utf8Validation);
            _nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetAndSetDelayTest()
        {
            _nativeImplementation.Setup(x => x.mdf_message_set_property(It.IsAny<IntPtr>(), (int)MDF_MSG_OPTION.MDF_MSG_OPT_DELAY, It.IsAny<int>())).Returns(1)
                .Verifiable();
            using Message message = new();
            Assert.AreEqual(default, message.Delay);
            const byte Delay = byte.MaxValue - 10;
            message.Delay = Delay;
            Assert.AreEqual(Delay, message.Delay);
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

            _nativeImplementation.Setup(x => x.mdf_message_add_numeric_str(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>()))
                .Callback((IntPtr message, uint tag, string value) => Assert.AreEqual(Value, value))
                .Returns(1);

            using Message message = new();
            Assert.IsTrue(message.AddNumeric(Field, Value));
            Assert.IsTrue(message.AddNumeric((uint)Field, Value));
            _nativeImplementation.Verify(x => x.mdf_message_add_numeric_str(message.Handle, (uint)Field, It.IsAny<string>()), Times.Exactly(2));

            _nativeImplementation.Setup(x => x.mdf_message_add_numeric(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                .Callback((IntPtr message, uint tag, IntPtr value) => Compare(Value, value))
                .Returns(1);
            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            Assert.IsTrue(message.AddNumeric(Field, bytes));
            Assert.IsTrue(message.AddNumeric((uint)Field, bytes));
            _nativeImplementation.Verify(x => x.mdf_message_add_numeric(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(2));
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

            Expression<Func<int, bool>> match = i => i < 0 || i > 19;
            _nativeImplementation.Setup(x => x.mdf_message_add_int(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<long>(), It.Is<int>(match))).Returns(0);
            _nativeImplementation.Setup(x => x.mdf_message_add_uint(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<ulong>(), It.Is<int>(match))).Returns(0);

            Assert.IsFalse(message.AddInt64(Field, SignedValue, -1));
            Assert.IsFalse(message.AddInt64((uint)Field, SignedValue, -1));

            Assert.IsFalse(message.AddUInt64(Field, UnsignedValue, -1));
            Assert.IsFalse(message.AddUInt64((uint)Field, UnsignedValue, -1));

            Assert.IsFalse(message.AddInt64(Field, SignedValue, 20));
            Assert.IsFalse(message.AddInt64((uint)Field, SignedValue, 20));

            Assert.IsFalse(message.AddUInt64(Field, UnsignedValue, 20));
            Assert.IsFalse(message.AddUInt64((uint)Field, UnsignedValue, 20));
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
            Assert.IsTrue(message.AddString(Field, bytes));
            Assert.IsTrue(message.AddString((uint)Field, bytes));
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

            _nativeImplementation.Setup(x => x.mdf_message_add_date_str(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>()))
                .Callback((IntPtr message, uint tag, string value) => Assert.AreEqual(Value, value))
                .Returns(1);

            using Message message = new();
            Assert.IsTrue(message.AddDate(Field, Value));
            Assert.IsTrue(message.AddDate((uint)Field, Value));
            _nativeImplementation.Verify(x => x.mdf_message_add_date_str(message.Handle, (uint)Field, It.IsAny<string>()), Times.Exactly(2));

            _nativeImplementation.Setup(x => x.mdf_message_add_date(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                .Callback((IntPtr message, uint tag, IntPtr value) => Compare(Value, value))
                .Returns(1);

            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            Assert.IsTrue(message.AddDate(Field, bytes));
            Assert.IsTrue(message.AddDate((uint)Field, bytes));
            _nativeImplementation.Verify(x => x.mdf_message_add_date(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(2));

            const int Year = 2020;
            const int Month = 12;
            const int Day = 6;
            _nativeImplementation.Setup(x => x.mdf_message_add_date2(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);
            Assert.IsTrue(message.AddDate(Field, Year, Month, Day));
            Assert.IsTrue(message.AddDate((uint)Field, Year, Month, Day));
            _nativeImplementation.Verify(x => x.mdf_message_add_date2(message.Handle, (uint)Field, Year, Month, Day), Times.Exactly(2));
        }

        [TestMethod]
        public void AddTimeTest()
        {
            const Field Field = Field.MDF_F_TIME;
            const string Value = "11:11:11";

            _nativeImplementation.Setup(x => x.mdf_message_add_time_str(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>()))
                .Callback((IntPtr message, uint tag, string value) => Assert.AreEqual(Value, value))
                .Returns(1);

            using Message message = new();
            Assert.IsTrue(message.AddTime(Field, Value));
            Assert.IsTrue(message.AddTime((uint)Field, Value));
            _nativeImplementation.Verify(x => x.mdf_message_add_time_str(message.Handle, (uint)Field, It.IsAny<string>()), Times.Exactly(2));

            _nativeImplementation.Setup(x => x.mdf_message_add_time(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                .Callback((IntPtr message, uint tag, IntPtr value) => Compare(Value, value))
                .Returns(1);
            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            Assert.IsTrue(message.AddTime(Field, bytes));
            Assert.IsTrue(message.AddTime((uint)Field, bytes));
            _nativeImplementation.Verify(x => x.mdf_message_add_time(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(2));

            _nativeImplementation.Setup(x => x.mdf_message_add_time2(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);
            int hour = 12;
            int minute = 13;
            int second = 14;
            const int Millisecond = 999;
            Assert.IsTrue(message.AddTime2(Field, hour, minute, second, Millisecond));
            Assert.IsTrue(message.AddTime2((uint)Field, hour, minute, second, Millisecond));
            _nativeImplementation.Verify(x => x.mdf_message_add_time2(message.Handle, (uint)Field, hour, minute, second, Millisecond), Times.Exactly(2));

            _nativeImplementation.Setup(x => x.mdf_message_add_time3(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);
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

            _nativeImplementation.Setup(x => x.mdf_message_add_list_str(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>()))
                .Callback((IntPtr message, uint tag, string value) => Assert.AreEqual(Value, value))
                .Returns(1);

            using Message message = new();
            Assert.IsTrue(message.AddList(Field, Value));
            Assert.IsTrue(message.AddList((uint)Field, Value));
            _nativeImplementation.Verify(x => x.mdf_message_add_list_str(message.Handle, (uint)Field, It.IsAny<string>()), Times.Exactly(2));

            _nativeImplementation.Setup(x => x.mdf_message_add_list(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<IntPtr>()))
                .Callback((IntPtr message, uint tag, IntPtr value) => Compare(Value, value))
                .Returns(1);

            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            Assert.IsTrue(message.AddList(Field, bytes));
            Assert.IsTrue(message.AddList((uint)Field, bytes));
            _nativeImplementation.Verify(x => x.mdf_message_add_list(message.Handle, (uint)Field, It.IsAny<IntPtr>()), Times.Exactly(2));
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
            _nativeImplementation.Setup(x => x.mdf_message_deserialize_str(messageHandle, It.IsAny<string>()))
                .Returns(1)
                .Callback((IntPtr message, string data) => Assert.AreEqual(Data, data));
            using Message message = new();
            Assert.IsTrue(message.Deserialize(Data));
            _nativeImplementation.Verify(x => x.mdf_message_deserialize_str(messageHandle, It.IsAny<string>()), Times.Once);

            _nativeImplementation.Setup(x => x.mdf_message_deserialize(messageHandle, It.IsAny<IntPtr>()))
                .Returns(1)
                .Callback((IntPtr message, IntPtr data) => Compare(Data, data));
            IntPtr p = Marshal.StringToHGlobalAnsi(Data);
            Assert.IsTrue(message.Deserialize(p));
            Assert.IsTrue(message.Deserialize(Encoding.ASCII.GetBytes(Data)));
            _nativeImplementation.Verify(x => x.mdf_message_deserialize(messageHandle, It.IsAny<IntPtr>()), Times.Exactly(2));

            _nativeImplementation.Setup(x => x.mdf_message_deserialize_str(messageHandle, It.IsAny<string>())).Returns(0);
            Assert.IsFalse(message.Deserialize(Data));
            _nativeImplementation.Verify(x => x.mdf_message_deserialize_str(messageHandle, It.IsAny<string>()), Times.Exactly(2));
            _nativeImplementation.Setup(x => x.mdf_message_deserialize(messageHandle, It.IsAny<IntPtr>())).Returns(0);
            Assert.IsFalse(message.Deserialize(p));
            _nativeImplementation.Verify(x => x.mdf_message_deserialize(messageHandle, It.IsAny<IntPtr>()), Times.Exactly(3));
        }

        [TestMethod]
        public void DisposeTest()
        {
            using (Message message = new()) { }
            _nativeImplementation.Verify(x => x.mdf_message_destroy(It.IsAny<IntPtr>()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateMessageWithNoNativeLibraryPathTest () => new Message(default(string));

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
            new Message().Deserialize(default(string));

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeserializeEmptyStringTest() =>
            new Message().Deserialize(string.Empty);

        [TestMethod]
        public void MethodsReturnFalseAfterDisposeTest()
        {
            Message disposedMessage = new();
            disposedMessage.Dispose();

            Assert.IsFalse(disposedMessage.Add(1, 1));
            Assert.IsFalse(disposedMessage.Add(1, MessageReference.MDF_M_REQUEST));

            Assert.IsFalse(disposedMessage.AddNumeric(1, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsFalse(disposedMessage.AddNumeric(Field.MDF_F_AVERAGE, "1.1"));
            byte[] bytes = Encoding.UTF8.GetBytes("1.1");
            Assert.IsFalse(disposedMessage.AddNumeric(1, bytes));
            Assert.IsFalse(disposedMessage.AddNumeric(Field.MDF_F_AVERAGE, bytes));

            Assert.IsFalse(disposedMessage.AddInt64(1, -12345, 2));
            Assert.IsFalse(disposedMessage.AddInt64(Field.MDF_F_AVERAGE, -12345, 2));

            Assert.IsFalse(disposedMessage.AddUInt64(1, 12345, 2));
            Assert.IsFalse(disposedMessage.AddUInt64(Field.MDF_F_AVERAGE, 12345, 2));

            Assert.IsFalse(disposedMessage.AddString(1, "abc"));
            Assert.IsFalse(disposedMessage.AddString(1, "abc", 1));
            Assert.IsFalse(disposedMessage.AddString(Field.MDF_F_REQUESTID, "abc"));
            Assert.IsFalse(disposedMessage.AddString(Field.MDF_F_REQUESTID, "abc", 1));
            bytes = Encoding.UTF8.GetBytes("abc");
            Assert.IsFalse(disposedMessage.AddString(1, bytes));
            Assert.IsFalse(disposedMessage.AddString(1, bytes, 1));
            Assert.IsFalse(disposedMessage.AddString(Field.MDF_F_REQUESTID, bytes));
            Assert.IsFalse(disposedMessage.AddString(Field.MDF_F_REQUESTID, bytes, 1));


            Assert.IsFalse(disposedMessage.AddDate(1, "2020-12-30"));
            Assert.IsFalse(disposedMessage.AddDate(Field.MDF_F_DATE, "2020-12-30"));
            Assert.IsFalse(disposedMessage.AddDate(1, 2020, 12, 30));
            Assert.IsFalse(disposedMessage.AddDate(Field.MDF_F_DATE, 2020, 12, 30));
            bytes = Encoding.UTF8.GetBytes("2020-12-30");
            Assert.IsFalse(disposedMessage.AddDate(1, bytes));
            Assert.IsFalse(disposedMessage.AddDate(Field.MDF_F_DATE, bytes));

            Assert.IsFalse(disposedMessage.AddTime(1, "16:30:30"));
            Assert.IsFalse(disposedMessage.AddTime(Field.MDF_F_TIME, "16:30:30"));
            bytes = Encoding.UTF8.GetBytes("16:30:30");
            Assert.IsFalse(disposedMessage.AddTime(1, bytes));
            Assert.IsFalse(disposedMessage.AddTime(Field.MDF_F_TIME, bytes));

            Assert.IsFalse(disposedMessage.AddTime2(1, 16, 30, 30, 999));
            Assert.IsFalse(disposedMessage.AddTime2(Field.MDF_F_TIME, 16, 30, 30, 999));

            Assert.IsFalse(disposedMessage.AddTime3(1, 16, 30, 30, 999999999));
            Assert.IsFalse(disposedMessage.AddTime3(Field.MDF_F_TIME, 16, 30, 30, 999999999));

            Assert.IsFalse(disposedMessage.AddList(1, "1"));
            Assert.IsFalse(disposedMessage.AddList(Field.MDF_F_INSREFLIST, "1"));
            bytes = Encoding.UTF8.GetBytes("1");
            Assert.IsFalse(disposedMessage.AddList(1, bytes));
            Assert.IsFalse(disposedMessage.AddList(Field.MDF_F_INSREFLIST, bytes));

            Assert.IsFalse(disposedMessage.Delete());

            Assert.IsFalse(disposedMessage.Serialize(out IntPtr _));

            Assert.IsFalse(disposedMessage.Deserialize("ABC"));
            Assert.IsFalse(disposedMessage.Deserialize(new IntPtr(123)));
            Assert.IsFalse(disposedMessage.Deserialize(Encoding.ASCII.GetBytes("ABC")));
        }

        [TestMethod]
        public unsafe void MemberThrowsWhenNativeFunctionIsMissingTest()
        {
            NativeImplementation nativeImplementation = new(default);
            nativeImplementation.mdf_message_set_property = default;
            nativeImplementation.mdf_message_add_int = default;
            nativeImplementation.mdf_message_add_uint = default;
            nativeImplementation.mdf_message_add_string2 = default;
            nativeImplementation.mdf_message_add_date2 = default;
            nativeImplementation.mdf_message_add_time2 = default;
            nativeImplementation.mdf_message_add_time3 = default;
            nativeImplementation.mdf_message_move = default;
            nativeImplementation.mdf_message_serialize = default;
            nativeImplementation.mdf_message_deserialize = default;
            nativeImplementation.mdf_message_get_num_fields = default;

            using Message message = new(nativeImplementation);
            CatchInvalidOperationException(() => message.Delay = 1, nameof(nativeImplementation.mdf_message_set_property));
            CatchInvalidOperationException(() => message.AddInt64(default(uint), default, default), nameof(nativeImplementation.mdf_message_add_int));
            CatchInvalidOperationException(() => message.AddInt64(default(Field), default, default), nameof(nativeImplementation.mdf_message_add_int));
            CatchInvalidOperationException(() => message.AddUInt64(default(uint), default, default), nameof(nativeImplementation.mdf_message_add_uint));
            CatchInvalidOperationException(() => message.AddUInt64(default(Field), default, default), nameof(nativeImplementation.mdf_message_add_uint));
            CatchInvalidOperationException(() => message.AddString(default(uint), default(string), default), nameof(nativeImplementation.mdf_message_add_string2));
            CatchInvalidOperationException(() => message.AddString(default(Field), default(string), default), nameof(nativeImplementation.mdf_message_add_string2));
            CatchInvalidOperationException(() => message.AddString(default(uint), default(ReadOnlySpan<byte>), default), nameof(nativeImplementation.mdf_message_add_string2));
            CatchInvalidOperationException(() => message.AddString(default(Field), default(ReadOnlySpan<byte>), default), nameof(nativeImplementation.mdf_message_add_string2));
            CatchInvalidOperationException(() => message.AddDate(default(uint), default, default, default), nameof(nativeImplementation.mdf_message_add_date2));
            CatchInvalidOperationException(() => message.AddDate(default(Field), default, default, default), nameof(nativeImplementation.mdf_message_add_date2));
            CatchInvalidOperationException(() => message.AddTime2(default(uint), default, default, default, default), nameof(nativeImplementation.mdf_message_add_time2));
            CatchInvalidOperationException(() => message.AddTime2(default(Field), default, default, default, default), nameof(nativeImplementation.mdf_message_add_time2));
            CatchInvalidOperationException(() => message.AddTime3(default(uint), default, default, default, default), nameof(nativeImplementation.mdf_message_add_time3));
            CatchInvalidOperationException(() => message.AddTime3(default(Field), default, default, default, default), nameof(nativeImplementation.mdf_message_add_time3));
            CatchInvalidOperationException(() => Message.Move(message, default, default, default), nameof(nativeImplementation.mdf_message_move));
            CatchInvalidOperationException(() => message.Serialize(out _), nameof(nativeImplementation.mdf_message_serialize));
            CatchInvalidOperationException(() => message.Deserialize("..."), nameof(nativeImplementation.mdf_message_deserialize));
            CatchInvalidOperationException(() => message.Deserialize(default(ReadOnlySpan<byte>)), nameof(nativeImplementation.mdf_message_deserialize));
            CatchInvalidOperationException(() => _ = message.FieldCount, nameof(nativeImplementation.mdf_message_get_num_fields));

            //The mdf_message_set_compression_level and mdf_message_set_utf8_validation in libmdf-1.0.25 should be used
            message.CompressionLevel = CompressionLevel.Z_BEST_COMPRESSION;
            message.Utf8Validation = false;
            //...assuming they exist in the installed native library
            nativeImplementation.mdf_message_set_compression_level = default;
            nativeImplementation.mdf_message_set_utf8_validation = default;
            CatchInvalidOperationException(() => message.CompressionLevel = CompressionLevel.Z_NO_COMPRESSION, 
                $"{nameof(nativeImplementation.mdf_message_set_property)} or {nameof(nativeImplementation.mdf_message_set_compression_level)}");
            CatchInvalidOperationException(() => message.Utf8Validation = true,
                $"{nameof(nativeImplementation.mdf_message_set_property)} or {nameof(nativeImplementation.mdf_message_set_utf8_validation)}");

            static void CatchInvalidOperationException(Action action, string missingFunctionName)
            {
                try
                {
                    action();
                    Assert.Fail($"No expected {nameof(InvalidOperationException)} was thrown.");
                }
                catch (InvalidOperationException ex)
                {
                    Assert.AreEqual($"The installed version of the native library doesn't include the {missingFunctionName} function.", ex.Message);
                }
            }
        }

        private static void Compare(string expectedValue, IntPtr actualValue)
        {
            byte[] bytes = new byte[expectedValue.Length];
            Marshal.Copy(actualValue, bytes, 0, expectedValue.Length);
            Assert.AreEqual(expectedValue, Encoding.ASCII.GetString(bytes));
        }
    }
}