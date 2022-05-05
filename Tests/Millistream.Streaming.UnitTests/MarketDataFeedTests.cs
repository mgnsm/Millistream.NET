using Microsoft.VisualStudio.TestTools.UnitTesting;
using Millistream.Streaming.Interop;
using Moq;
using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using MarketDataFeed = Millistream.Streaming.MarketDataFeed<object, object>;

namespace Millistream.Streaming.UnitTests
{
    [TestClass]
    public class MarketDataFeedTests
    {
        private delegate void GetNextMessageCallback(IntPtr handle, ref int mref, ref int mclass, ref ulong insref);
        private delegate void GetNextFieldCallback(IntPtr handle, ref uint tag, ref IntPtr value);
        private delegate void GetInt32PropertyCallback(IntPtr handle, int option, ref int value);
        private delegate void GetUInt64PropertyCallback(IntPtr handle, int option, ref ulong value);
        private delegate void GetInt64PropertyCallback(IntPtr handle, int option, ref long value);
        private delegate void GetIntPtrPropertyCallback(IntPtr handle, int option, ref IntPtr value);

        [AssemblyInitialize]
        public static void DoNotInitializeDefaultMarketDataFeedByDefaultTest(TestContext _1)
        {
            NativeImplementation.Implementation = new Mock<INativeImplementation>().Object;
            Assert.AreEqual(0, NativeImplementation.InstanceCount);
            _ = NativeImplementation.Default;
            Assert.AreEqual(1, NativeImplementation.InstanceCount);
            _ = NativeImplementation.Default;
            Assert.AreEqual(1, NativeImplementation.InstanceCount);
        }

        [TestMethod]
        public void CreateMarketDataFeedTest()
        {
            NativeImplementation.Implementation = new Mock<INativeImplementation>().Object;

            using MarketDataFeed mdf = new();
            using MarketDataFeed mdf2 = new();
            using MarketDataFeed mdf3 = new();
            Assert.AreEqual(1, NativeImplementation.InstanceCount);

            using MarketDataFeed mdf4 = new("lib");
            using MarketDataFeed mdf5 = new("lib");
            using MarketDataFeed mdf6 = new("lib2");
            using MarketDataFeed mdf7 = new();
            Assert.AreEqual(4, NativeImplementation.InstanceCount);
        }

        [TestMethod]
        public void GetFileDescriptorTest()
        {
            const int FileDescriptor = 1000;
            Mock<INativeImplementation> nativeImplementation = new();
            nativeImplementation
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_FD, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, int option_, ref int value) => value = FileDescriptor));
            NativeImplementation.Implementation = nativeImplementation.Object;

            using MarketDataFeed mdf = new();
            Assert.AreEqual(FileDescriptor, mdf.FileDescriptor);
        }

        [TestMethod]
        public void GetErrorCodeTest()
        {
            const Error ErrorCode = Error.MDF_ERR_MSG_TO_LARGE;
            Mock<INativeImplementation> nativeImplementation = new();
            nativeImplementation
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_ERROR, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, int option_, ref int value) => value = (int)ErrorCode));
            NativeImplementation.Implementation = nativeImplementation.Object;

            using MarketDataFeed mdf = new();
            Assert.AreEqual(ErrorCode, mdf.ErrorCode);
        }

        [TestMethod]
        public void GetAndSetReceivedBytesTest() => 
            GetUInt64Property(MDF_OPTION.MDF_OPT_RCV_BYTES, mdf => mdf.ReceivedBytes);

        [TestMethod]
        public void SetReceivedBytesTest()
        {
            const ulong ReceivedBytes = 100;
            Mock<INativeImplementation> nativeImplementation = new();
            Setup(nativeImplementation, MDF_OPTION.MDF_OPT_RCV_BYTES, ReceivedBytes);

            using MarketDataFeed mdf = new()
            {
                ReceivedBytes = ReceivedBytes
            };
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetAndSetSentBytesTest() => GetUInt64Property(MDF_OPTION.MDF_OPT_SENT_BYTES, mdf => mdf.SentBytes);

        [TestMethod]
        public void SetSentBytesTest()
        {
            const ulong SentBytes = 500;
            Mock<INativeImplementation> nativeImplementation = new();
            Setup(nativeImplementation, MDF_OPTION.MDF_OPT_SENT_BYTES, SentBytes);

            using MarketDataFeed mdf = new()
            {
                SentBytes = SentBytes
            };
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetConnectionTimeoutTest() => 
            GetInt32Property(MDF_OPTION.MDF_OPT_CONNECT_TIMEOUT, mdf => mdf.ConnectionTimeout);

        [TestMethod]
        public void SetConnectionTimeoutTest()
        {
            const int ConnectionTimeout = 10;
            Mock<INativeImplementation> nativeImplementation = new();
            Setup(nativeImplementation, MDF_OPTION.MDF_OPT_CONNECT_TIMEOUT, ConnectionTimeout);

            using MarketDataFeed mdf = new()
            {
                ConnectionTimeout = ConnectionTimeout
            };
            nativeImplementation.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateMarketDataFeedWithNoNativeLibraryPathTest() => new MarketDataFeed(default(string));

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateMarketDataFeedWithEmptyNativeLibraryPathTest() => new MarketDataFeed(string.Empty);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallConnectionTimeoutTest()
        {
            NativeImplementation.Implementation = new Mock<INativeImplementation>().Object;
            using MarketDataFeed mdf = new()
            {
                ConnectionTimeout = MarketDataFeed.MinConnectionTimeout - 1
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeConnectionTimeoutTest()
        {
            NativeImplementation.Implementation = new Mock<INativeImplementation>().Object;
            using MarketDataFeed mdf = new()
            {
                ConnectionTimeout = MarketDataFeed.MaxConnectionTimeout + 1
            };
        }

        [TestMethod]
        public void GetHeartbeatIntervalTest() => 
            GetInt32Property(MDF_OPTION.MDF_OPT_HEARTBEAT_INTERVAL, mdf => mdf.HeartbeatInterval);

        [TestMethod]
        public void SetHeartbeatIntervalTest()
        {
            const int HeartbeatInterval = 60;
            Mock<INativeImplementation> nativeImplementation = new();
            Setup(nativeImplementation, MDF_OPTION.MDF_OPT_HEARTBEAT_INTERVAL, HeartbeatInterval);

            using MarketDataFeed mdf = new()
            {
                HeartbeatInterval = HeartbeatInterval
            };
            nativeImplementation.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallHeartbeatIntervalTest()
        {
            NativeImplementation.Implementation = new Mock<INativeImplementation>().Object;
            using MarketDataFeed mdf = new()
            {
                HeartbeatInterval = MarketDataFeed.MinHeartbeatInterval - 1
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeHeartbeatIntervalTest()
        {
            NativeImplementation.Implementation = new Mock<INativeImplementation>().Object;
            using MarketDataFeed mdf = new()
            {
                HeartbeatInterval = MarketDataFeed.MaxHeartbeatInterval + 1
            };
        }

        [TestMethod]
        public void GetMaximumMissedHeartbeatsTest() => 
            GetInt32Property(MDF_OPTION.MDF_OPT_HEARTBEAT_MAX_MISSED, mdf => mdf.MaximumMissedHeartbeats);

        [TestMethod]
        public void SetMaximumMissedHeartbeatsTest()
        {
            const int MaximumMissedHeartbeats = 50;
            Mock<INativeImplementation> nativeImplementation = new();
            Setup(nativeImplementation, MDF_OPTION.MDF_OPT_HEARTBEAT_MAX_MISSED, MaximumMissedHeartbeats);

            using MarketDataFeed mdf = new()
            {
                MaximumMissedHeartbeats = MaximumMissedHeartbeats
            };
            nativeImplementation.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallMaximumMissedHeartbeatsTest()
        {
            NativeImplementation.Implementation = new Mock<INativeImplementation>().Object;
            using MarketDataFeed mdf = new()
            {
                MaximumMissedHeartbeats = MarketDataFeed.MinMissedHeartbeats - 1
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeMaximumMissedHeartbeatsTest()
        {
            NativeImplementation.Implementation = new Mock<INativeImplementation>().Object;
            using MarketDataFeed mdf = new()
            {
                MaximumMissedHeartbeats = MarketDataFeed.MaxMissedHeartbeats + 1
            };
        }

        [TestMethod]
        public void GetAndSetNoDelayTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            int returnValue = 1;
            nativeImplementation
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_TCP_NODELAY, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, int option_, ref int value) => value = returnValue));

            nativeImplementation
                .Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_TCP_NODELAY, It.IsAny<IntPtr>()))
                .Returns(1)
                .Verifiable();

            NativeImplementation.Implementation = nativeImplementation.Object;
            using MarketDataFeed mdf = new();
            //assert that the expected values are returned from the getter
            Assert.IsTrue(mdf.NoDelay);
            returnValue = 0;
            Assert.IsFalse(mdf.NoDelay);
            //set the property
            mdf.NoDelay = true;
            //verify that the setter was invoked
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetAndSetNoEncryptionTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            int returnValue = 1;
            nativeImplementation
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_NO_ENCRYPTION, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, int option_, ref int value) => value = returnValue));

            nativeImplementation
                .Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_NO_ENCRYPTION, It.IsAny<IntPtr>()))
                .Returns(1)
                .Verifiable();

            NativeImplementation.Implementation = nativeImplementation.Object;
            using MarketDataFeed mdf = new();
            //assert that the expected values are returned from the getter
            Assert.IsTrue(mdf.NoEncryption);
            returnValue = 0;
            Assert.IsFalse(mdf.NoEncryption);
            //set the property
            mdf.NoEncryption = true;
            //verify that the setter was invoked
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetTimeDifferenceTest()
        {
            const int Difference = 1;
            Mock<INativeImplementation> nativeImplementation = new();
            nativeImplementation
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_TIME_DIFFERENCE, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, int option, ref int value) => value = Difference));
            NativeImplementation.Implementation = nativeImplementation.Object;
            using MarketDataFeed mdf = new();
            Assert.AreEqual(Difference, mdf.TimeDifference);
        }

        [TestMethod]
        public void GetBindAddressTest() => GetStringProperty(MDF_OPTION.MDF_OPT_BIND_ADDRESS, mdf => mdf.BindAddress);

        [TestMethod]
        public void SetBindAddressTest()
        {
            const string BindingAddress = "123";
            Mock<INativeImplementation> nativeImplementation = new();
            Setup(nativeImplementation, MDF_OPTION.MDF_OPT_BIND_ADDRESS, BindingAddress);

            using MarketDataFeed mdf = new()
            {
                BindAddress = BindingAddress
            };
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetTimeDifferenceNsTest()
        {
            const long Difference = long.MaxValue;
            Mock<INativeImplementation> nativeImplementation = new();
            IntPtr feedHandle = new(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            nativeImplementation
                .Setup(x => x.mdf_get_property(feedHandle, (int)MDF_OPTION.MDF_OPT_TIME_DIFFERENCE_NS, ref It.Ref<long>.IsAny))
                .Returns(1)
                .Callback(new GetInt64PropertyCallback((IntPtr handler, int option, ref long value) => value = Difference));
            NativeImplementation.Implementation = nativeImplementation.Object;

            using MarketDataFeed mdf = new();
            Assert.AreEqual(Difference, mdf.TimeDifferenceNs);
        }

        [TestMethod]
        public void GetMessageDigestsTest() => GetStringProperty(MDF_OPTION.MDF_OPT_CRYPT_DIGESTS, mdf => mdf.MessageDigests);

        [TestMethod]
        public void SetMessageDigestsTest()
        {
            const string MessageDigests = "sha1,md5";
            Mock<INativeImplementation> nativeImplementation = new();
            Setup(nativeImplementation, MDF_OPTION.MDF_OPT_CRYPT_DIGESTS, MessageDigests);

            using MarketDataFeed mdf = new()
            {
                MessageDigests = MessageDigests
            };
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetCiphersTest() => GetStringProperty(MDF_OPTION.MDF_OPT_CRYPT_CIPHERS, mdf => mdf.Ciphers);

        [TestMethod]
        public void SetCiphersTest()
        {
            const string Ciphers = "aes-128-ctr,chacha20";
            Mock<INativeImplementation> nativeImplementation = new();
            Setup(nativeImplementation, MDF_OPTION.MDF_OPT_CRYPT_CIPHERS, Ciphers);

            using MarketDataFeed mdf = new()
            {
                Ciphers = Ciphers
            };
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetMessageDigestTest() => 
            GetStringProperty(MDF_OPTION.MDF_OPT_CRYPT_DIGEST, mdf => mdf.MessageDigest);

        [TestMethod]
        public void GetCipherTest() =>
            GetStringProperty(MDF_OPTION.MDF_OPT_CRYPT_CIPHER, mdf => mdf.Cipher);

        [TestMethod]
        public void GetTimeoutTest() =>
            GetInt32Property(MDF_OPTION.MDF_OPT_TIMEOUT, mdf => mdf.Timeout);

        [TestMethod]
        public void GetAndSetHandleDelayTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            int returnValue = 1;
            nativeImplementation
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_HANDLE_DELAY, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, int option_, ref int value) => value = returnValue));

            nativeImplementation
                .Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_HANDLE_DELAY, It.IsAny<IntPtr>()))
                .Returns(1)
                .Verifiable();

            NativeImplementation.Implementation = nativeImplementation.Object;
            using MarketDataFeed mdf = new();
            //assert that the expected values are returned from the getter
            Assert.IsTrue(mdf.HandleDelay);
            returnValue = 0;
            Assert.IsFalse(mdf.HandleDelay);
            //set the property
            mdf.HandleDelay = true;
            //verify that the setter was invoked
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetDelayTest()
        {
            const byte Delay = 1;
            Mock<INativeImplementation> nativeImplementation = new();
            IntPtr feedHandle = new(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            nativeImplementation
                .Setup(x => x.mdf_get_delay(feedHandle))
                .Returns(Delay);
            NativeImplementation.Implementation = nativeImplementation.Object;

            using MarketDataFeed mdf = new();
            Assert.AreEqual(Delay, mdf.Delay);
        }

        [TestMethod]
        public void GetAndSetDataCallbackTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            nativeImplementation.Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_DATA_CALLBACK_FUNCTION, It.IsAny<IntPtr>())).Returns(1).Verifiable();
            NativeImplementation.Implementation = nativeImplementation.Object;
            using MarketDataFeed mdf = new();
            Assert.IsNull(mdf.DataCallback);
            mdf.DataCallback = new DataCallback<object, object>((userData, mdf) => { });
            Assert.IsNotNull(mdf.DataCallback);
            nativeImplementation.Verify();
            mdf.DataCallback = null;
            Assert.IsNull(mdf.DataCallback);
            nativeImplementation.Verify(x => x.mdf_set_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_DATA_CALLBACK_FUNCTION, IntPtr.Zero), Times.Once);
        }

        [TestMethod]
        public void GetAndSetDataStatusCallbackTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            nativeImplementation.Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_STATUS_CALLBACK_FUNCTION, It.IsAny<IntPtr>())).Returns(1).Verifiable();
            NativeImplementation.Implementation = nativeImplementation.Object;
            using MarketDataFeed mdf = new();
            Assert.IsNull(mdf.StatusCallback);
            mdf.StatusCallback = new StatusCallback<object>((userData, status, host, ip) => { });
            Assert.IsNotNull(mdf.StatusCallback);
            nativeImplementation.Verify();
            mdf.StatusCallback = null;
            Assert.IsNull(mdf.StatusCallback);
            nativeImplementation.Verify(x => x.mdf_set_property(It.IsAny<IntPtr>(), (int)MDF_OPTION.MDF_OPT_STATUS_CALLBACK_FUNCTION, IntPtr.Zero), Times.Once);
        }

        [TestMethod]
        public void ConsumeTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            IntPtr feedHandle = new(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            const int Timeout = 10;
            const int ReturnValue = 1;
            Expression<Func<INativeImplementation, int>> expression = x => x.mdf_consume(feedHandle, Timeout);
            nativeImplementation.Setup(expression).Returns(ReturnValue);
            NativeImplementation.Implementation = nativeImplementation.Object;

            using MarketDataFeed mdf = new();
            Assert.AreEqual(ReturnValue, mdf.Consume(Timeout));
            nativeImplementation.Verify(expression, Times.Once);
        }

        [TestMethod]
        public void GetNextMessageTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            IntPtr feedHandle = new(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            const MessageReference MessageReference = MessageReference.MDF_M_CI;
            const MessageClasses MessageClass = MessageClasses.MDF_MC_ESTIMATES;
            const ulong InstrumentReference = 500;
            nativeImplementation.Setup(x => x.mdf_get_next_message(feedHandle, ref It.Ref<int>.IsAny, ref It.Ref<int>.IsAny, ref It.Ref<ulong>.IsAny))
                .Callback(new GetNextMessageCallback((IntPtr _, ref int mref, ref int mclass, ref ulong insref) =>
                {
                    mref = (int)MessageReference;
                    mclass = (int)MessageClass;
                    insref = InstrumentReference;
                }))
                .Returns(1)
                .Verifiable();
            NativeImplementation.Implementation = nativeImplementation.Object;

            using MarketDataFeed mdf = new();
            Assert.IsTrue(mdf.GetNextMessage(out int returnedMref, out int returnedMclass, out ulong returnedInsref));
            Assert.AreEqual((int)MessageReference, returnedMref);
            Assert.AreEqual((int)MessageClass, returnedMclass);
            Assert.AreEqual(InstrumentReference, returnedInsref);

            Assert.IsTrue(mdf.GetNextMessage(out MessageReference returnedMessageReference, out MessageClasses returnedMessageClasses, out returnedInsref));
            Assert.AreEqual(MessageReference, returnedMessageReference);
            Assert.AreEqual(MessageClass, returnedMessageClasses);
            Assert.AreEqual(InstrumentReference, returnedInsref);

            nativeImplementation.Verify();

            nativeImplementation.Setup(x => x.mdf_get_next_message(feedHandle, ref It.Ref<int>.IsAny, ref It.Ref<int>.IsAny, ref It.Ref<ulong>.IsAny))
                .Callback(new GetNextMessageCallback((IntPtr _, ref int mref, ref int mclass, ref ulong insref) =>
                {
                    mref = (int)MessageReference;
                    mclass = 20971528;
                }))
                .Returns(1);

            Assert.IsTrue(mdf.GetNextMessage(out returnedMessageReference, out returnedMessageClasses, out _));
            Assert.AreEqual(MessageReference, returnedMessageReference);
            Assert.AreEqual(MessageClasses.MDF_MC_ORDER | MessageClasses.MDF_MC_MBO | MessageClasses.MDF_MC_QUOTEBBO, returnedMessageClasses);

            nativeImplementation.Setup(x => x.mdf_get_next_message(feedHandle, ref It.Ref<int>.IsAny, ref It.Ref<int>.IsAny, ref It.Ref<ulong>.IsAny))
                .Callback(new GetNextMessageCallback((IntPtr _, ref int mref, ref int mclass, ref ulong insref) =>
                {
                    mref = (int)MessageReference;
                    mclass = int.MaxValue;
                }))
                .Returns(1);

            Assert.IsTrue(mdf.GetNextMessage(out _, out returnedMessageClasses, out _));
            Assert.AreEqual((MessageClasses)int.MaxValue, returnedMessageClasses);
        }

        [TestMethod]
        public void GetNextFieldTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            IntPtr feedHandle = new(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);

            const Field Tag = Field.MDF_F_ADDRESS;
            Expression<Func<INativeImplementation, int>> expression = x => x.mdf_get_next_field(feedHandle, ref It.Ref<uint>.IsAny, ref It.Ref<IntPtr>.IsAny);
            nativeImplementation.Setup(expression)
                .Callback(new GetNextFieldCallback((IntPtr _, ref uint tag, ref IntPtr value) => tag = (uint)Tag))
                .Returns(1);
            NativeImplementation.Implementation = nativeImplementation.Object;

            using MarketDataFeed mdf = new();
            Assert.IsTrue(mdf.GetNextField(out uint tag, out ReadOnlySpan<byte> _));
            Assert.AreEqual((uint)Tag, tag);
            Assert.IsTrue(mdf.GetNextField(out Field field, out ReadOnlySpan<byte> _));
            Assert.AreEqual(Tag, field);
            nativeImplementation.Verify(expression, Times.Exactly(2));
        }

        [TestMethod]
        public void ConnectTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            IntPtr feedHandle = new(123);
            const string Servers = "host.server.com:9100";
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            Expression<Func<INativeImplementation, int>> expression = x => x.mdf_connect(feedHandle, It.IsAny<IntPtr>());
            nativeImplementation.Setup(expression)
                .Callback((IntPtr handle, IntPtr server) => Compare(Servers, server))
                .Returns(1);
            NativeImplementation.Implementation = nativeImplementation.Object;

            using MarketDataFeed mdf = new();
            Assert.IsTrue(mdf.Connect(Servers));
            nativeImplementation.Verify(expression, Times.Once);
        }

        [TestMethod]
        public void DisconnectTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            IntPtr feedHandle = new(456);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            Expression<Action<INativeImplementation>> expression = x => x.mdf_disconnect(feedHandle);
            nativeImplementation.Setup(expression);
            NativeImplementation.Implementation = nativeImplementation.Object;

            using MarketDataFeed mdf = new();
            mdf.Disconnect();
            mdf.Disconnect();
            mdf.Disconnect();
            nativeImplementation.Verify(expression, Times.Exactly(3));
        }

        [TestMethod]
        public void SendTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            IntPtr feedHandle = new(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            IntPtr messageHandle = new(456);
            nativeImplementation.Setup(x => x.mdf_message_create()).Returns(messageHandle);
            NativeImplementation.Implementation = nativeImplementation.Object;

            using Message message = new();
            Expression<Func<INativeImplementation, int>> expression = x => x.mdf_message_send(feedHandle, messageHandle);
            nativeImplementation.Setup(expression).Returns(1);

            using MarketDataFeed mdf = new();
            Assert.IsTrue(mdf.Send(message));
            nativeImplementation.Verify(expression, Times.Once);

            IMarketDataFeed<object, object> iMdf = mdf;
            Assert.IsTrue(iMdf.Send(message));
            nativeImplementation.Verify(expression, Times.Exactly(2));
            IMessage iMessage = message;
            Assert.IsTrue(iMdf.Send(iMessage));
            nativeImplementation.Verify(expression, Times.Exactly(3));
            Assert.IsFalse(iMdf.Send(new Mock<IMessage>().Object));
        }

        [TestMethod]
        public unsafe void DelayThrowsWhenNativeFunctionIsMissingTest()
        {
            NativeImplementation nativeImplementation = new(default);
            nativeImplementation.mdf_get_delay = default;

            using MarketDataFeed mdf = new(nativeImplementation);
            try
            {
                _ = mdf.Delay;
                Assert.Fail($"No expected {nameof(InvalidOperationException)} was thrown.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual($"The installed version of the native library doesn't include the {nameof(nativeImplementation.mdf_get_delay)} function.", ex.Message);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetFileDescriptorAfterDisposeTest() => _ = GetDisposedMdf().FileDescriptor;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetErrorCodeAfterDisposeTest() => _ = GetDisposedMdf().ErrorCode;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetErrorCodeAfterDisposeTest() => GetDisposedMdf().ErrorCode = Error.MDF_ERR_AUTHFAIL;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetReceivedBytesAfterDisposeTest() => _ = GetDisposedMdf().ReceivedBytes;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetReceivedBytesAfterDisposeTest() => GetDisposedMdf().ReceivedBytes = 100;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetSentBytesAfterDisposeTest() => _ = GetDisposedMdf().SentBytes;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetSentBytesAfterDisposeTest() => GetDisposedMdf().SentBytes = 100;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetConnectionTimeoutAfterDisposeTest() => _ = GetDisposedMdf().ConnectionTimeout;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetConnectionTimeoutAfterDisposeTest() => GetDisposedMdf().ConnectionTimeout = 10;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetHeartbeatIntervalAfterDisposeTest() => _ = GetDisposedMdf().HeartbeatInterval;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetHeartbeatIntervalAfterDisposeTest() => GetDisposedMdf().HeartbeatInterval = 60;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetMaximumMissedHeartbeatsAfterDisposeTest() => _ = GetDisposedMdf().MaximumMissedHeartbeats;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetMaximumMissedHeartbeatsAfterDisposeTest() => GetDisposedMdf().MaximumMissedHeartbeats = 50;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetNoDelayAfterDisposeTest() => _ = GetDisposedMdf().NoDelay;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetNoDelayAfterDisposeTest() => GetDisposedMdf().NoDelay = true;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetNoEncryptionAfterDisposeTest() => _ = GetDisposedMdf().NoEncryption;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetNoEncryptionAfterDisposeTest() => GetDisposedMdf().NoEncryption = true;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetTimeDifferenceAfterDisposeTest() => _ = GetDisposedMdf().TimeDifference;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetBindAddressAfterDisposeTest() => _ = GetDisposedMdf().BindAddress;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetBindAddressAfterDisposeTest() => GetDisposedMdf().BindAddress = "123";

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetTimeDifferenceNsAfterDisposeTest() => _ = GetDisposedMdf().TimeDifferenceNs;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetMessageDigestsAfterDisposeTest() => _ = GetDisposedMdf().MessageDigests;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetMessageDigestsAfterDisposeTest() => GetDisposedMdf().MessageDigests = "sha1";

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetCiphersAfterDisposeTest() => _ = GetDisposedMdf().Ciphers;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetCiphersAfterDisposeTest() => GetDisposedMdf().Ciphers = "chacha20";

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetMessageDigestAfterDisposeTest() => _ = GetDisposedMdf().MessageDigest;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetCipherAfterDisposeTest() => _ = GetDisposedMdf().Cipher;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetTimeoutAfterDisposeTest() => _ = GetDisposedMdf().Timeout;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetHandleDelayAfterDisposeTest() => _ = GetDisposedMdf().HandleDelay;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetHandleDelayAfterDisposeTest() => GetDisposedMdf().HandleDelay = true;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetDelayAfterDisposeTest() => _ = GetDisposedMdf().Delay;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetDataCallbackfterDisposeTest() => _ = GetDisposedMdf().DataCallback;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetDataCallbackAfterDisposeTest() => GetDisposedMdf().DataCallback = new DataCallback<object, object>((userData, mdf) => { });

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetCallbackUserDatafterDisposeTest() => _ = GetDisposedMdf().CallbackUserData;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetCallbackUserDatafterDisposeTest() => GetDisposedMdf().CallbackUserData = default;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetStatusCallbackfterDisposeTest() => _ = GetDisposedMdf().StatusCallback;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetStatusCallbackAfterDisposeTest() => GetDisposedMdf().StatusCallback = new StatusCallback<object>((userData, status, host, ip) => { });

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetStatusCallbackUserDatafterDisposeTest() => _ = GetDisposedMdf().StatusCallbackUserData;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetStatusCallbackUserDatafterDisposeTest() => GetDisposedMdf().StatusCallbackUserData = default;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallConsumeAfterDisposeTest() => GetDisposedMdf().Consume(10);

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallGetNextMessageAfterDisposeTest() => 
            GetDisposedMdf().GetNextMessage(out int _, out int _, out ulong _);

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallGetNextMessageOverloadAfterDisposeTest() =>
            GetDisposedMdf().GetNextMessage(out MessageReference _, out MessageClasses _, out ulong _);

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallGetNextFieldAfterDisposeTest() => GetDisposedMdf().GetNextField(out uint _, out ReadOnlySpan<byte> _);

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FetchInvalidMessageDataTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            IntPtr feedHandle = new(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            nativeImplementation.Setup(x => x.mdf_get_next_message(feedHandle, ref It.Ref<int>.IsAny, ref It.Ref<int>.IsAny, ref It.Ref<ulong>.IsAny))
                .Callback(new GetNextMessageCallback((IntPtr _, ref int mref, ref int mclass, ref ulong insref) => mref = -1))
                .Returns(1);
            NativeImplementation.Implementation = nativeImplementation.Object;

            using MarketDataFeed mdf = new();
            mdf.GetNextMessage(out MessageReference _, out MessageClasses _, out ulong _);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FetchInvalidTagTest()
        {
            Mock<INativeImplementation> nativeImplementation = new();
            IntPtr feedHandle = new(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);

            Expression<Func<INativeImplementation, int>> expression = x => x.mdf_get_next_field(feedHandle, ref It.Ref<uint>.IsAny, ref It.Ref<IntPtr>.IsAny);
            nativeImplementation.Setup(expression)
                .Callback(new GetNextFieldCallback((IntPtr _, ref uint tag, ref IntPtr value) => tag = uint.MaxValue))
                .Returns(1);

            NativeImplementation.Implementation = nativeImplementation.Object;
            using MarketDataFeed mdf = new();
            mdf.GetNextField(out Field _, out ReadOnlySpan<byte> _);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallConnectAfterDisposeTest() => GetDisposedMdf().Connect("server:port");

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallDisconnectAfterDisposeTest() => GetDisposedMdf().Disconnect();

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallSendAfterDisposeTest()
        {
            using Message message = new();
            GetDisposedMdf().Send(message);
        }

        private static void GetInt32Property(MDF_OPTION option, Func<MarketDataFeed, int> getter)
        {
            const int Value = 5;
            Mock<INativeImplementation> nativeImplementationMock = new();
            nativeImplementationMock
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), (int)option, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, int option_, ref int value) => value = Value))
                .Verifiable();
            NativeImplementation.Implementation = nativeImplementationMock.Object;
            using MarketDataFeed mdf = new();
            //assert that the expected value is returned from the getter
            Assert.AreEqual(Value, getter(mdf));
            //assert that the setter was invoked
            nativeImplementationMock.Verify();
        }

        private static void GetUInt64Property(MDF_OPTION option, Func<MarketDataFeed, ulong> getter)
        {
            const ulong Value = 100;
            Mock<INativeImplementation> nativeImplementationMock = new();
            nativeImplementationMock
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), (int)option, ref It.Ref<ulong>.IsAny))
                .Returns(1)
                .Callback(new GetUInt64PropertyCallback((IntPtr handler, int option_, ref ulong value) => value = Value))
                .Verifiable();
            NativeImplementation.Implementation = nativeImplementationMock.Object;
            using MarketDataFeed mdf = new();
            Assert.AreEqual(Value, getter(mdf));
            nativeImplementationMock.Verify();
        }

        private static void GetStringProperty(MDF_OPTION option, Func<MarketDataFeed, string> getter)
        {
            const string Value = "abc";
            byte[] bytes = Encoding.UTF8.GetBytes(Value + char.MinValue);
            Mock<INativeImplementation> nativeImplementation = new();
            IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
            try
            {
                Marshal.Copy(bytes, 0, ptr, bytes.Length);

                nativeImplementation
                    .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), (int)option, ref It.Ref<IntPtr>.IsAny))
                    .Returns(1)
                    .Callback(new GetIntPtrPropertyCallback((IntPtr handler, int option, ref IntPtr value) => value = ptr))
                    .Verifiable();
                NativeImplementation.Implementation = nativeImplementation.Object;

                using MarketDataFeed mdf = new();
                Assert.AreEqual(Value, getter(mdf));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            nativeImplementation.Verify();
        }

        private static void Setup(Mock<INativeImplementation> nativeImplementation, MDF_OPTION option, int value)
        {
            IntPtr feedHandle = new(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            nativeImplementation.Setup(x => x.mdf_set_property(feedHandle, (int)option, It.IsAny<IntPtr>()))
                .Callback((IntPtr handle, int option, IntPtr value_) => Assert.AreEqual(value, Marshal.ReadInt32(value_)))
                .Returns(1)
                .Verifiable();
            NativeImplementation.Implementation = nativeImplementation.Object;

        }

        private static void Setup(Mock<INativeImplementation> nativeImplementation, MDF_OPTION option, ulong value)
        {
            IntPtr feedHandle = new(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            nativeImplementation.Setup(x => x.mdf_set_property(feedHandle, (int)option, It.IsAny<IntPtr>()))
                .Callback((IntPtr handle, int option, IntPtr value_) => Assert.AreEqual(value, (ulong)Marshal.ReadInt64(value_)))
                .Returns(1)
                .Verifiable();
            NativeImplementation.Implementation = nativeImplementation.Object;

        }

        private static void Setup(Mock<INativeImplementation> nativeImplementation, MDF_OPTION option, string value)
        {
            IntPtr feedHandle = new(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            nativeImplementation.Setup(x => x.mdf_set_property(feedHandle, (int)option, It.IsAny<IntPtr>()))
                .Callback((IntPtr handle, int option, IntPtr value_) => Compare(value, value_))
                .Returns(1)
                .Verifiable();
            NativeImplementation.Implementation = nativeImplementation.Object;
        }

        private static MarketDataFeed GetDisposedMdf()
        {
            NativeImplementation.Implementation = new Mock<INativeImplementation>().Object;
            MarketDataFeed mdf = new();
            mdf.Dispose();
            return mdf;
        }

        private static void Compare(string expectedValue, IntPtr actualValue)
        {
            byte[] bytes = new byte[expectedValue.Length];
            Marshal.Copy(actualValue, bytes, 0, expectedValue.Length);
            Assert.AreEqual(expectedValue, Encoding.UTF8.GetString(bytes));
        }
    }
}