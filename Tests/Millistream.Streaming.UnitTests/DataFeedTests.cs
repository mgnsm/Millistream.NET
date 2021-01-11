﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MarketDataFeed = Millistream.Streaming.MarketDataFeed<object, object>;

namespace Millistream.Streaming.UnitTests
{
    [TestClass]
    public class DataFeedTests
    {
        private const string Host = "host";
        private const string Username = "username";
        private const string Password = "password";

        private delegate void mdf_get_next_message_callback(IntPtr handle, ref int message, ref int message_class, ref ulong instrument);

        [TestMethod]
        public void ConnectAndAuthenticateTest()
        {
            Mock<INativeImplementation> nativeImplementationMock = new Mock<INativeImplementation>();
            nativeImplementationMock.Setup(x => x.mdf_connect(It.IsAny<IntPtr>(), It.IsAny<string>())).Returns(1);
            nativeImplementationMock.Setup(x => x.mdf_disconnect(It.IsAny<IntPtr>()));
            nativeImplementationMock.Setup(x => x.mdf_consume(It.IsAny<IntPtr>(), It.IsAny<int>())).Returns(1);
            nativeImplementationMock.Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), It.IsAny<MDF_OPTION>(), ref It.Ref<int>.IsAny)).Returns(1);
            nativeImplementationMock.Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), It.IsAny<MDF_OPTION>(), It.IsAny<IntPtr>())).Returns(1);
            int returnedMessage = (int)MessageReference.MDF_M_LOGONGREETING;
            nativeImplementationMock
                .Setup(x => x.mdf_get_next_message(It.IsAny<IntPtr>(), ref It.Ref<int>.IsAny, ref It.Ref<int>.IsAny, ref It.Ref<ulong>.IsAny))
                .Callback(new mdf_get_next_message_callback((IntPtr handle, ref int message, ref int message_class, ref ulong instrument)
                    => message = returnedMessage))
                .Returns(1);

            using DataFeed dataFeed = new DataFeed(nativeImplementationMock.Object);
            //The Connect method should return true whenever mdf_get_next_message returns 1 and sets message to MDF_M_LOGONGREETING.
            Assert.IsTrue(dataFeed.Connect(Host, Username, Password));

            const string ExtraCredential = "extra_credential";
            nativeImplementationMock.Verify(x => x.mdf_message_add(It.IsAny<IntPtr>(), 0, (int)MessageReference.MDF_M_LOGON));
            nativeImplementationMock.Verify(x => x.mdf_message_add_string(It.IsAny<IntPtr>(), (uint)Field.MDF_F_USERNAME, Username));
            nativeImplementationMock.Verify(x => x.mdf_message_add_string(It.IsAny<IntPtr>(), (uint)Field.MDF_F_PASSWORD, Password));
            nativeImplementationMock.Verify(x => x.mdf_message_add_string(It.IsAny<IntPtr>(), (uint)Field.MDF_F_EXTRACREDENTIAL, ExtraCredential), Times.Never);

            Assert.IsTrue(dataFeed.Connect(Host, Username, Password, ExtraCredential));
            nativeImplementationMock.Verify(x => x.mdf_message_add_string(It.IsAny<IntPtr>(), (uint)Field.MDF_F_EXTRACREDENTIAL, ExtraCredential));

            //The Disconnect() method should be called if the Connect method is called again
            Assert.IsTrue(dataFeed.Connect(Host, Username, Password));
            nativeImplementationMock.Verify(x => x.mdf_disconnect(It.IsAny<IntPtr>()));

            returnedMessage = (int)MessageReference.MDF_M_LOGOFF;
            //The Connect method should return false when the message is set to MDF_M_LOGOFF
            Assert.IsFalse(dataFeed.Connect(Host, Username, Password));

            //...and when mdf_connect returns anything else than 1
            nativeImplementationMock.Setup(x => x.mdf_connect(It.IsAny<IntPtr>(), It.IsAny<string>())).Returns(-1);
            Assert.IsFalse(dataFeed.Connect(Host, Username, Password));
            //...and when mdf_consume returns -1
            nativeImplementationMock.Setup(x => x.mdf_connect(It.IsAny<IntPtr>(), It.IsAny<string>())).Returns(1);
            nativeImplementationMock.Setup(x => x.mdf_consume(It.IsAny<IntPtr>(), It.IsAny<int>())).Returns(-1);
            Assert.IsFalse(dataFeed.Connect(Host, Username, Password));
        }

        [TestMethod]
        public void RequestTest()
        {
            Mock<INativeImplementation> nativeImplementationMock = new Mock<INativeImplementation>();
            nativeImplementationMock.Setup(x => x.mdf_connect(It.IsAny<IntPtr>(), It.IsAny<string>())).Returns(1);
            nativeImplementationMock.Setup(x => x.mdf_consume(It.IsAny<IntPtr>(), It.IsAny<int>())).Returns(1);
            nativeImplementationMock.Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), It.IsAny<MDF_OPTION>(), ref It.Ref<int>.IsAny)).Returns(1);
            nativeImplementationMock.Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), It.IsAny<MDF_OPTION>(), It.IsAny<IntPtr>())).Returns(1);
            int returnedMessage = (int)MessageReference.MDF_M_LOGONGREETING;
            nativeImplementationMock.Setup(x => x.mdf_get_next_message(It.IsAny<IntPtr>(),
                ref It.Ref<int>.IsAny, ref It.Ref<int>.IsAny, ref It.Ref<ulong>.IsAny))
                .Callback(new mdf_get_next_message_callback((IntPtr handle, ref int message, ref int message_class, ref ulong instrument)
                    => message = returnedMessage))
                .Returns(1);

            RequestClass[] requestClasses = new RequestClass[1] { RequestClass.MDF_RC_BASICDATA };
            ulong[] instrumentReferences = new ulong[3] { 1L, 2L, 3L };
            int mdf_message_add_list(IntPtr message, uint tag, string value)
            {
                const string Separator = " ";
                switch (tag)
                {
                    case (uint)Field.MDF_F_REQUESTCLASS:
                        Assert.AreEqual(string.Join(Separator, requestClasses.Select(x => ((int)x).ToString())), value);
                        break;
                    case (uint)Field.MDF_F_INSREFLIST:
                        Assert.AreEqual(string.Join(Separator, instrumentReferences), value);
                        break;
                }
                return default;
            }
            nativeImplementationMock
                .Setup(x => x.mdf_message_add_list(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>()))
                .Returns<IntPtr, uint, string>(mdf_message_add_list);

            RequestType requestType = RequestType.MDF_RT_FULL;
            int mdf_message_add_numeric(IntPtr message, uint tag, string value)
            {
                Assert.AreEqual((uint)Field.MDF_F_REQUESTTYPE, tag);
                Assert.AreEqual(((int)requestType).ToString(), value);
                return default;
            }
            nativeImplementationMock
                .Setup(x => x.mdf_message_add_numeric(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>()))
                .Returns<IntPtr, uint, string>(mdf_message_add_numeric);

            using DataFeed dataFeed = new DataFeed(nativeImplementationMock.Object);
            if (!dataFeed.Connect(Host, Username, Password))
                Assert.Inconclusive($"The {nameof(DataFeed.Connect)} method returned false.");

            static int mdf_message_add(IntPtr message, ulong instrument_reference, int message_reference)
            {
                Assert.AreEqual(0UL, instrument_reference);
                Assert.AreEqual((int)MessageReference.MDF_M_REQUEST, message_reference);
                return default;
            }
            nativeImplementationMock
                .Setup(x => x.mdf_message_add(It.IsAny<IntPtr>(), It.IsAny<ulong>(), It.IsAny<int>()))
                .Returns<IntPtr, ulong, int>(mdf_message_add);

            DateTime utcStartTime = new DateTime(2020, 12, 21, 22, 16, 35, 999).AddTicks(2);
            SubscribeMessage subscribeMessage = new SubscribeMessage(requestType, requestClasses)
            {
                UtcStartTime = utcStartTime
            };
            dataFeed.Request(subscribeMessage);
            //assert that the expected methods of the INativeImplementation were actually called
            nativeImplementationMock.Verify(x => x.mdf_message_add_list(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>()));
            nativeImplementationMock.Verify(x => x.mdf_message_add_numeric(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>()));
            nativeImplementationMock.Verify(x => x.mdf_message_add(It.IsAny<IntPtr>(), It.IsAny<ulong>(), It.IsAny<int>()));
            nativeImplementationMock.Verify(x => x.mdf_message_add_date2(It.IsAny<IntPtr>(), It.IsAny<uint>(), utcStartTime.Year, utcStartTime.Month, utcStartTime.Day));
            nativeImplementationMock.Verify(x => x.mdf_message_add_time3(It.IsAny<IntPtr>(), It.IsAny<uint>(), utcStartTime.Hour, utcStartTime.Minute, utcStartTime.Second, utcStartTime.Millisecond * 1_000_000 + 200));
            nativeImplementationMock.Invocations.Clear();
            //subscribe to some instruments
            subscribeMessage.InstrumentReferences = instrumentReferences;
            dataFeed.Request(subscribeMessage);
            nativeImplementationMock.Verify(x => x.mdf_message_add_list(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>()), Times.Exactly(2));
            nativeImplementationMock.Verify(x => x.mdf_message_add_numeric(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>()));
            nativeImplementationMock.Verify(x => x.mdf_message_add(It.IsAny<IntPtr>(), It.IsAny<ulong>(), It.IsAny<int>()));
            nativeImplementationMock.Invocations.Clear();

            const int MaxInstrumentReferencesCount = 1_000_000;
            subscribeMessage.InstrumentReferences = Enumerable.Range(1, MaxInstrumentReferencesCount + 1).Select(x => (ulong)x);
            try
            {
                dataFeed.Request(subscribeMessage);
                Assert.Fail($"More than {MaxInstrumentReferencesCount} instrument references unexpectedly accepted.");
            }
            catch (ArgumentException) { }

            //unsubcribe and assert that the MDF_M_UNSUBSCRIBE message reference was passed to the mdf_message_add method
            static int mdf_message_add2(IntPtr message, ulong instrument_reference, int message_reference)
            {
                Assert.AreEqual(0UL, instrument_reference);
                Assert.AreEqual((int)MessageReference.MDF_M_UNSUBSCRIBE, message_reference);
                return default;
            }
            nativeImplementationMock
                .Setup(x => x.mdf_message_add(It.IsAny<IntPtr>(), It.IsAny<ulong>(), It.IsAny<int>()))
                .Returns<IntPtr, ulong, int>(mdf_message_add2);

            dataFeed.Request(new UnsubscribeMessage(requestClasses));
            nativeImplementationMock.Verify(x => x.mdf_message_add_list(It.IsAny<IntPtr>(), It.IsAny<uint>(), It.IsAny<string>()));
            nativeImplementationMock.Verify(x => x.mdf_message_add(It.IsAny<IntPtr>(), It.IsAny<ulong>(), It.IsAny<int>()));
        }

        [TestMethod]
        public void GetAndSetConnectionTimeoutTest() => GetAndSetPropertyTest(MDF_OPTION.MDF_OPT_CONNECT_TIMEOUT, feed => feed.ConnectionTimeout, feed => feed.ConnectionTimeout = 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallConnectionTimeoutTest()
        {
            using DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object)
            {
                ConnectionTimeout = MarketDataFeed.MinConnectionTimeout - 1
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeConnectionTimeoutTest()
        {
            using DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object)
            {
                ConnectionTimeout = MarketDataFeed.MaxConnectionTimeout + 1
            };
        }

        [TestMethod]
        public void GetErrorCodeTest()
        {
            const Error ErrorCode = Error.MDF_ERR_MSG_TO_LARGE;
            Mock<INativeImplementation> nativeImplementationMock = new Mock<INativeImplementation>();
            nativeImplementationMock
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_ERROR, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, MDF_OPTION option_, ref int value) => value = (int)ErrorCode));

            using DataFeed dataFeed = new DataFeed(nativeImplementationMock.Object);
            Assert.AreEqual(ErrorCode, dataFeed.ErrorCode);
        }

        [TestMethod]
        public void GetAndSetHeartbeatIntervalTest() => GetAndSetPropertyTest(MDF_OPTION.MDF_OPT_HEARTBEAT_INTERVAL, feed => feed.HeartbeatInterval, feed => feed.HeartbeatInterval = 60);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallHeartbeatIntervalTest()
        {
            using DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object)
            {
                HeartbeatInterval = MarketDataFeed.MinHeartbeatInterval - 1
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeHeartbeatIntervalTest()
        {
            using DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object)
            {
                HeartbeatInterval = MarketDataFeed.MaxHeartbeatInterval + 1
            };
        }

        [TestMethod]
        public void GetAndSetMaximumMissedHeartbeats() => GetAndSetPropertyTest(MDF_OPTION.MDF_OPT_HEARTBEAT_MAX_MISSED, feed => feed.MaximumMissedHeartbeats, feed => feed.MaximumMissedHeartbeats = 50);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallMaximumMissedHeartbeatsTest()
        {
            using DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object)
            {
                MaximumMissedHeartbeats = MarketDataFeed.MinMissedHeartbeats - 1
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeMaximumMissedHeartbeatsTest()
        {
            using DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object)
            {
                MaximumMissedHeartbeats = MarketDataFeed.MaxMissedHeartbeats + 1
            };
        }

        [TestMethod]
        public void GetAndSetNoDelayTest()
        {
            Mock<INativeImplementation> nativeImplementationMock = new Mock<INativeImplementation>();
            int returnValue = 1;
            nativeImplementationMock
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_TCP_NODELAY, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, MDF_OPTION option_, ref int value) => value = returnValue));

            nativeImplementationMock
                .Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_TCP_NODELAY, It.IsAny<IntPtr>()))
                .Returns(1)
                .Verifiable();

            using DataFeed dataFeed = new DataFeed(nativeImplementationMock.Object);
            //assert that the expected values are returned from the getter
            Assert.AreEqual(true, dataFeed.NoDelay);
            returnValue = 0;
            Assert.AreEqual(false, dataFeed.NoDelay);
            //set the property
            dataFeed.NoDelay = true;
            //verify that the setter was invoked
            nativeImplementationMock.Verify();
        }

        [TestMethod]
        public void GetReceivedBytesTest() => GetUInt64PropertyTest(MDF_OPTION.MDF_OPT_RCV_BYTES, feed => feed.ReceivedBytes);

        [TestMethod]
        public void GetSentBytesTest() => GetUInt64PropertyTest(MDF_OPTION.MDF_OPT_SENT_BYTES, feed => feed.SentBytes);

        [TestMethod]
        public void GetTimeDifferenceTest()
        {
            const int Difference = 1;
            Mock<INativeImplementation> nativeImplementationMock = new Mock<INativeImplementation>();
            nativeImplementationMock
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), MDF_OPTION.MDF_OPT_TIME_DIFFERENCE, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, MDF_OPTION option, ref int value) => value = Difference));

            using DataFeed dataFeed = new DataFeed(nativeImplementationMock.Object);
            Assert.AreEqual(Difference, dataFeed.TimeDifference);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RequestBeforeConnectTest()
        {
            using DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Request(new SubscribeMessage(RequestType.MDF_RT_FULL, new RequestClass[1] { RequestClass.MDF_RC_BASICDATA }));
        }

        [TestMethod]
        public void DisposeTest()
        {
            Mock<INativeImplementation> nativeImplementationMock = new Mock<INativeImplementation>();
            nativeImplementationMock.Setup(x => x.mdf_message_destroy(It.IsAny<IntPtr>()));
            nativeImplementationMock.Setup(x => x.mdf_destroy(It.IsAny<IntPtr>()));

            DataFeed dataFeed = new DataFeed(nativeImplementationMock.Object);
            Parallel.Invoke(new Action[5] { dataFeed.Dispose, dataFeed.Dispose, dataFeed.Dispose, dataFeed.Dispose, dataFeed.Dispose });

            //verify that the Dispose() method can be called called more than once, without the handles being destroyed more than once
            nativeImplementationMock.Verify(x => x.mdf_message_destroy(It.IsAny<IntPtr>()), Times.Once);
            nativeImplementationMock.Verify(x => x.mdf_destroy(It.IsAny<IntPtr>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallConnectAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            dataFeed.Connect(Host, Username, Password);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallDisconnectAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            dataFeed.Disconnect();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallRecycleAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            dataFeed.Recycle(new ResponseMessage());
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallRequestAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            dataFeed.Request(new SubscribeMessage(RequestType.MDF_RT_FULL, new RequestClass[1] { RequestClass.MDF_RC_BASICDATA }));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotHookupConnectionStatusChangedEventHandlerAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            dataFeed.ConnectionStatusChanged += (s, e) => { };
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSubscribeToDataAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            dataFeed.Data.Subscribe(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetConnectionTimeoutAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            _ = dataFeed.ConnectionTimeout;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetConnectionTimeoutAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            dataFeed.ConnectionTimeout = 10;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetErrorCodeAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            _ = dataFeed.ErrorCode;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetHeartbeatIntervalAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            _ = dataFeed.HeartbeatInterval;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetHeartbeatIntervalAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            dataFeed.HeartbeatInterval = 60;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetMaximumMissedHeartbeatsAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            _ = dataFeed.MaximumMissedHeartbeats;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetMaximumMissedHeartbeatsAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            dataFeed.MaximumMissedHeartbeats = 50;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetNoDelayAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            _ = dataFeed.NoDelay;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetNoDelayAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            dataFeed.NoDelay = true;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetReceivedBytesAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            _ = dataFeed.ReceivedBytes;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetSentBytesAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            _ = dataFeed.SentBytes;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetTimeDifferenceAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Dispose();
            _ = dataFeed.TimeDifference;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotSubscribeWithInsRefRequestClass()
        {
            using DataFeed dataFeed = new DataFeed(new Mock<INativeImplementation>().Object);
            dataFeed.Request(new SubscribeMessage(RequestType.MDF_RT_FULL, new RequestClass[1] { RequestClass.MDF_RC_INSREF }));
        }

        private void GetAndSetPropertyTest(MDF_OPTION option, Func<DataFeed, int> getter, Action<DataFeed> setter)
        {
            const int Value = 5;
            Mock<INativeImplementation> nativeImplementationMock = new Mock<INativeImplementation>();
            nativeImplementationMock
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), option, ref It.Ref<int>.IsAny))
                .Returns(1)
                .Callback(new GetInt32PropertyCallback((IntPtr handler, MDF_OPTION option_, ref int value) => value = Value));
            nativeImplementationMock
                .Setup(x => x.mdf_set_property(It.IsAny<IntPtr>(), option, It.IsAny<IntPtr>()))
                .Returns(1)
                .Verifiable();

            using DataFeed dataFeed = new DataFeed(nativeImplementationMock.Object);
            //assert that the expected value is returned from the getter
            Assert.AreEqual(Value, getter(dataFeed));
            //set the property
            setter(dataFeed);
            //assert that the setter was invoked
            nativeImplementationMock.Verify();
        }

        private void GetUInt64PropertyTest(MDF_OPTION option, Func<DataFeed, ulong> getter)
        {
            const ulong Bytes = 100;
            Mock<INativeImplementation> nativeImplementationMock = new Mock<INativeImplementation>();
            nativeImplementationMock
                .Setup(x => x.mdf_get_property(It.IsAny<IntPtr>(), option, ref It.Ref<ulong>.IsAny))
                .Returns(1)
                .Callback(new GetUInt64PropertyCallback((IntPtr handler, MDF_OPTION option_, ref ulong value) => value = Bytes));

            //assert that the property returns the correct value
            using DataFeed dataFeed = new DataFeed(nativeImplementationMock.Object);
            Assert.AreEqual(Bytes, getter(dataFeed));
        }
    }
}