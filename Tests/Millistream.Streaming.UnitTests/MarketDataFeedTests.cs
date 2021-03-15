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
        private delegate void GetInt32PropertyCallback(IntPtr handle, MDF_OPTION option, ref int value);
        private delegate void GetUInt64PropertyCallback(IntPtr handle, MDF_OPTION option, ref ulong value);
        private delegate void GetInt64PropertyCallback(IntPtr handle, MDF_OPTION option, ref long value);

        [TestMethod]
        public void GetFileDescriptorTest()
        {
            const int FileDescriptor = 1000;
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_FD, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, MDF_OPTION option_, ref int value) => value = FileDescriptor));

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            Assert.AreEqual(FileDescriptor, mdf.FileDescriptor);
        }

        [TestMethod]
        public void GetErrorCodeTest()
        {
            const Error ErrorCode = Error.MDF_ERR_MSG_TO_LARGE;
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_ERROR, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, MDF_OPTION option_, ref int value) => value = (int)ErrorCode));

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            Assert.AreEqual(ErrorCode, mdf.ErrorCode);
        }

        [TestMethod]
        public void GetAndSetReceivedBytesTest() => GetAndSetUInt64Property(MDF_OPTION.MDF_OPT_RCV_BYTES, mdf => mdf.ReceivedBytes, mdf => mdf.ReceivedBytes = 100);

        [TestMethod]
        public void GetAndSetSentBytesTest() => GetAndSetUInt64Property(MDF_OPTION.MDF_OPT_SENT_BYTES, mdf => mdf.SentBytes, mdf => mdf.SentBytes = 500);

        [TestMethod]
        public void GetAndSetConnectionTimeoutTest() => 
            GetAndSetProperty(MDF_OPTION.MDF_OPT_CONNECT_TIMEOUT, mdf => mdf.ConnectionTimeout, mdf => mdf.ConnectionTimeout = 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallConnectionTimeoutTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed(new Mock<INativeImplementation>().Object)
            {
                ConnectionTimeout = MarketDataFeed.MinConnectionTimeout - 1
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeConnectionTimeoutTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed(new Mock<INativeImplementation>().Object)
            {
                ConnectionTimeout = MarketDataFeed.MaxConnectionTimeout + 1
            };
        }

        [TestMethod]
        public void GetAndSetHeartbeatIntervalTest() => 
            GetAndSetProperty(MDF_OPTION.MDF_OPT_HEARTBEAT_INTERVAL, mdf => mdf.HeartbeatInterval, mdf => mdf.HeartbeatInterval = 60);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallHeartbeatIntervalTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed(new Mock<INativeImplementation>().Object)
            {
                HeartbeatInterval = MarketDataFeed.MinHeartbeatInterval - 1
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeHeartbeatIntervalTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed(new Mock<INativeImplementation>().Object)
            {
                HeartbeatInterval = MarketDataFeed.MaxHeartbeatInterval + 1
            };
        }

        [TestMethod]
        public void GetAndSetMaximumMissedHeartbeats() => 
            GetAndSetProperty(MDF_OPTION.MDF_OPT_HEARTBEAT_MAX_MISSED, mdf => mdf.MaximumMissedHeartbeats, mdf => mdf.MaximumMissedHeartbeats = 50);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallMaximumMissedHeartbeatsTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed(new Mock<INativeImplementation>().Object)
            {
                MaximumMissedHeartbeats = MarketDataFeed.MinMissedHeartbeats - 1
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeMaximumMissedHeartbeatsTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed(new Mock<INativeImplementation>().Object)
            {
                MaximumMissedHeartbeats = MarketDataFeed.MaxMissedHeartbeats + 1
            };
        }

        [TestMethod]
        public void GetAndSetNoDelayTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            int returnValue = 1;
            nativeImplementation
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_TCP_NODELAY, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, MDF_OPTION option_, ref int value) => value = returnValue));

            nativeImplementation
                .Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_TCP_NODELAY, It.IsAny<IntPtr>()))
                .Returns(1)
                .Verifiable();

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            //assert that the expected values are returned from the getter
            Assert.AreEqual(true, mdf.NoDelay);
            returnValue = 0;
            Assert.AreEqual(false, mdf.NoDelay);
            //set the property
            mdf.NoDelay = true;
            //verify that the setter was invoked
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetAndSetNoEncryptionTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            int returnValue = 1;
            nativeImplementation
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_NO_ENCRYPTION, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, MDF_OPTION option_, ref int value) => value = returnValue));

            nativeImplementation
                .Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_NO_ENCRYPTION, It.IsAny<IntPtr>()))
                .Returns(1)
                .Verifiable();

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            //assert that the expected values are returned from the getter
            Assert.AreEqual(true, mdf.NoEncryption);
            returnValue = 0;
            Assert.AreEqual(false, mdf.NoEncryption);
            //set the property
            mdf.NoEncryption = true;
            //verify that the setter was invoked
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetTimeDifferenceTest()
        {
            const int Difference = 1;
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_TIME_DIFFERENCE, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, MDF_OPTION option, ref int value) => value = Difference));

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            Assert.AreEqual(Difference, mdf.TimeDifference);
        }

        [TestMethod]
        public void GetAndSetBindAddressTest()
        {
            const string BindingAddress = "123";
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_BIND_ADDRESS, It.IsAny<IntPtr>()))
                .Callback((IntPtr handle, MDF_OPTION option, IntPtr value) => Compare(BindingAddress, value))
                .Returns(1)
                .Verifiable();
            nativeImplementation.Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_BIND_ADDRESS, ref It.Ref<IntPtr>.IsAny))
                .Returns(1)
                .Verifiable();

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object)
            {
                BindAddress = BindingAddress
            };
            _ = mdf.BindAddress;
            nativeImplementation.Verify();
        }

        [TestMethod]
        public void GetTimeDifferenceNsTest()
        {
            const long Difference = long.MaxValue;
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            IntPtr feedHandle = new IntPtr(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            nativeImplementation
                .Setup(x => x.mdf_get_property(feedHandle, MDF_OPTION.MDF_OPT_TIME_DIFFERENCE_NS, ref It.Ref<long>.IsAny))
                .Returns(1)
                .Callback(new GetInt64PropertyCallback((IntPtr handler, MDF_OPTION option, ref long value) => value = Difference));

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            Assert.AreEqual(Difference, mdf.TimeDifferenceNs);
        }

        [TestMethod]
        public void GetAndSetDataCallbackTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_DATA_CALLBACK_FUNCTION, It.IsAny<IntPtr>())).Returns(1).Verifiable();
            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            Assert.IsNull(mdf.DataCallback);
            mdf.DataCallback = new Action<object, MarketDataFeed<object, object>>((userData, mdf) => { });
            Assert.IsNotNull(mdf.DataCallback);
            nativeImplementation.Verify();
            mdf.DataCallback = null;
            Assert.IsNull(mdf.DataCallback);
            nativeImplementation.Verify(x => x.mdf_set_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_DATA_CALLBACK_FUNCTION, IntPtr.Zero), Times.Once);
        }

        [TestMethod]
        public void GetAndSetDataStatusCallbackTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            nativeImplementation.Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_STATUS_CALLBACK_FUNCTION, It.IsAny<IntPtr>())).Returns(1).Verifiable();
            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            Assert.IsNull(mdf.StatusCallback);
            mdf.StatusCallback = new Action<object, ConnectionStatus, string, string>((userData, status, host, ip) => { });
            Assert.IsNotNull(mdf.StatusCallback);
            nativeImplementation.Verify();
            mdf.StatusCallback = null;
            Assert.IsNull(mdf.StatusCallback);
            nativeImplementation.Verify(x => x.mdf_set_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_STATUS_CALLBACK_FUNCTION, IntPtr.Zero), Times.Once);
        }

        [TestMethod]
        public void ConsumeTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            IntPtr feedHandle = new IntPtr(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            const int Timeout = 10;
            const int ReturnValue = 1;
            Expression<Func<INativeImplementation, int>> expression = x => x.mdf_consume(feedHandle, Timeout);
            nativeImplementation.Setup(expression).Returns(ReturnValue);

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            Assert.AreEqual(ReturnValue, mdf.Consume(Timeout));
            nativeImplementation.Verify(expression, Times.Once);
        }

        [TestMethod]
        public void GetNextMessageTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            IntPtr feedHandle = new IntPtr(123);
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

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
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
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            IntPtr feedHandle = new IntPtr(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);

            const Field Tag = Field.MDF_F_ADDRESS;
            Expression<Func<INativeImplementation, int>> expression = x => x.mdf_get_next_field(feedHandle, ref It.Ref<uint>.IsAny, ref It.Ref<IntPtr>.IsAny);
            nativeImplementation.Setup(expression)
                .Callback(new GetNextFieldCallback((IntPtr _, ref uint tag, ref IntPtr value) => tag = (uint)Tag))
                .Returns(1);

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            Assert.IsTrue(mdf.GetNextField(out uint tag, out ReadOnlySpan<byte> _));
            Assert.AreEqual((uint)Tag, tag);
            Assert.IsTrue(mdf.GetNextField(out Field field, out ReadOnlySpan<byte> _));
            Assert.AreEqual(Tag, field);
            nativeImplementation.Verify(expression, Times.Exactly(2));
        }

        [TestMethod]
        public void ConnectTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            IntPtr feedHandle = new IntPtr(123);
            const string Servers = "host.server.com:9100";
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            Expression<Func<INativeImplementation, int>> expression = x => x.mdf_connect(feedHandle, It.IsAny<IntPtr>());
            nativeImplementation.Setup(expression)
                .Callback((IntPtr handle, IntPtr server) => Compare(Servers, server))
                .Returns(1);

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            Assert.IsTrue(mdf.Connect(Servers));
            nativeImplementation.Verify(expression, Times.Once);
        }

        [TestMethod]
        public void DisconnectTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            IntPtr feedHandle = new IntPtr(456);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            Expression<Action<INativeImplementation>> expression = x => x.mdf_disconnect(feedHandle);
            nativeImplementation.Setup(expression);

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            mdf.Disconnect();
            mdf.Disconnect();
            mdf.Disconnect();
            nativeImplementation.Verify(expression, Times.Exactly(3));
        }

        [TestMethod]
        public void SendTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            IntPtr feedHandle = new IntPtr(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            IntPtr messageHandle = new IntPtr(456);
            nativeImplementation.Setup(x => x.mdf_message_create()).Returns(messageHandle);

            using Message message = new Message(nativeImplementation.Object);
            Expression<Func<INativeImplementation, int>> expression = x => x.mdf_message_send(feedHandle, messageHandle);
            nativeImplementation.Setup(expression).Returns(1);

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
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
        public void CannotGetDataCallbackfterDisposeTest() => _ = GetDisposedMdf().DataCallback;

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetDataCallbackAfterDisposeTest() => GetDisposedMdf().DataCallback = new Action<object, MarketDataFeed<object, object>>((userData, mdf) => { });

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
        public void CannotSetStatusCallbackAfterDisposeTest() => GetDisposedMdf().StatusCallback = new Action<object, ConnectionStatus, string, string>((userData, status, host, ip) => { });

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
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            IntPtr feedHandle = new IntPtr(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);
            nativeImplementation.Setup(x => x.mdf_get_next_message(feedHandle, ref It.Ref<int>.IsAny, ref It.Ref<int>.IsAny, ref It.Ref<ulong>.IsAny))
                .Callback(new GetNextMessageCallback((IntPtr _, ref int mref, ref int mclass, ref ulong insref) => mref = -1))
                .Returns(1);

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
            mdf.GetNextMessage(out MessageReference _, out MessageClasses _, out ulong _);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FetchInvalidTagTest()
        {
            Mock<INativeImplementation> nativeImplementation = new Mock<INativeImplementation>();
            IntPtr feedHandle = new IntPtr(123);
            nativeImplementation.Setup(x => x.mdf_create()).Returns(feedHandle);

            Expression<Func<INativeImplementation, int>> expression = x => x.mdf_get_next_field(feedHandle, ref It.Ref<uint>.IsAny, ref It.Ref<IntPtr>.IsAny);
            nativeImplementation.Setup(expression)
                .Callback(new GetNextFieldCallback((IntPtr _, ref uint tag, ref IntPtr value) => tag = uint.MaxValue))
                .Returns(1);

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementation.Object);
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
            using Message message = new Message(new Mock<INativeImplementation>().Object);
            GetDisposedMdf().Send(message);
        }

        private static void GetAndSetProperty(MDF_OPTION option, Func<MarketDataFeed, int> getter, Action<MarketDataFeed> setter)
        {
            const int Value = 5;
            Mock<INativeImplementation> nativeImplementationMock = new Mock<INativeImplementation>();
            nativeImplementationMock
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), option, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, MDF_OPTION option_, ref int value) => value = Value));
            SetProperty(nativeImplementationMock, Value, option, getter, setter);
        }

        private static void GetAndSetUInt64Property(MDF_OPTION option, Func<MarketDataFeed, ulong> getter, Action<MarketDataFeed> setter)
        {
            const ulong Bytes = 100;
            Mock<INativeImplementation> nativeImplementationMock = new Mock<INativeImplementation>();
            nativeImplementationMock
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), option, ref It.Ref<ulong>.IsAny))
                .Returns(1)
                .Callback(new GetUInt64PropertyCallback((IntPtr handler, MDF_OPTION option_, ref ulong value) => value = Bytes));
            SetProperty(nativeImplementationMock, Bytes, option, getter, setter);
        }

        private static void SetProperty<T>(Mock<INativeImplementation> nativeImplementationMock, T Value, MDF_OPTION option, Func<MarketDataFeed, T> getter, Action<MarketDataFeed> setter)
        {
            nativeImplementationMock
                .Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), option, It.IsAny<IntPtr>()))
                .Returns(1)
                .Verifiable();

            using MarketDataFeed mdf = new MarketDataFeed(nativeImplementationMock.Object);
            //assert that the expected value is returned from the getter
            Assert.AreEqual(Value, getter(mdf));
            //set the property
            setter(mdf);
            //assert that the setter was invoked
            nativeImplementationMock.Verify();
        }

        private static MarketDataFeed GetDisposedMdf()
        {
            MarketDataFeed mdf = new MarketDataFeed(new Mock<INativeImplementation>().Object);
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