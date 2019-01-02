using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Millistream.Streaming.Fakes;

namespace Millistream.Streaming.UnitTests
{
    [TestClass]
    public class DataFeedTests
    {
        private const string Host = "host";
        private const string Username = "username";
        private const string Password = "password";

        [TestMethod]
        public void ConnectAndAuthenticateTest()
        {
            StubINativeImplementation nativeImplementation = new StubINativeImplementation
            {
                Mdf_connectIntPtrString = (handle, server) => 1,
                Mdf_consumeIntPtrInt32 = (handle, timeout) => 1,
                Mdf_get_next_messageIntPtrInt32RefInt32RefUInt32Ref = (IntPtr handle, ref int message, ref int message_class, ref uint instrument) =>
                {
                    message = (int)MessageReference.MDF_M_LOGONGREETING;
                    return 1;
                }
            };
            nativeImplementation.Mdf_consumeIntPtrInt32 = (handle, timout) => 1;

            using (DataFeed dataFeed = new DataFeed(nativeImplementation))
            {
                //The Connect method should return true whenever mdf_get_next_message returns 1 and sets message to MDF_M_LOGONGREETING.
                Assert.IsTrue(dataFeed.Connect(Host, Username, Password));

                //The Disconnect() method should be called if the Connect method is called again
                bool disconnected = false;
                nativeImplementation.Mdf_disconnectIntPtr = handle => disconnected = true;
                Assert.IsTrue(dataFeed.Connect(Host, Username, Password));
                Assert.IsTrue(disconnected);
                
                //The Connect method should return false when the message is set to MDF_M_LOGOFF
                nativeImplementation.Mdf_get_next_messageIntPtrInt32RefInt32RefUInt32Ref = (IntPtr handle, ref int message, ref int message_class, ref uint instrument) =>
                {
                    message = (int)MessageReference.MDF_M_LOGOFF;
                    return 1;
                };
                Assert.IsFalse(dataFeed.Connect(Host, Username, Password));

                //...and when mdf_connect returns anything else than 1
                nativeImplementation.Mdf_connectIntPtrString = (handle, server) => -1;
                Assert.IsFalse(dataFeed.Connect(Host, Username, Password));
                //...and when mdf_consume returns -1
                nativeImplementation.Mdf_connectIntPtrString = (handle, server) => 1;
                nativeImplementation.Mdf_consumeIntPtrInt32 = (handle, timeout) => -1;
                Assert.IsFalse(dataFeed.Connect(Host, Username, Password));
            }
        }

        [TestMethod]
        public void RequestTest()
        {
            StubINativeImplementation nativeImplementation = new StubINativeImplementation
            {
                Mdf_connectIntPtrString = (handle, server) => 1,
                Mdf_consumeIntPtrInt32 = (handle, timeout) => 1,
                Mdf_get_next_messageIntPtrInt32RefInt32RefUInt32Ref = (IntPtr handle, ref int message, ref int message_class, ref uint instrument) =>
                {
                    message = (int)MessageReference.MDF_M_LOGONGREETING;
                    return 1;
                }
            };
            nativeImplementation.Mdf_consumeIntPtrInt32 = (handle, timout) => 1;

            int methodCallCounter = 0;
            RequestClass[] requestClasses = new RequestClass[1] { RequestClass.MDF_RC_BASICDATA };
            ulong[] instrumentReferences = new ulong[3] { 1L, 2L, 3L };
            nativeImplementation.Mdf_message_add_listIntPtrUInt32String = (message, tag, value) =>
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
                methodCallCounter++;
                return default(int);
            };
            RequestType requestType = RequestType.MDF_RT_FULL;
            nativeImplementation.Mdf_message_add_numericIntPtrUInt32String = (message, tag, value) =>
            {
                Assert.AreEqual((uint)Field.MDF_F_REQUESTTYPE, tag);
                Assert.AreEqual(((int)requestType).ToString(), value);
                methodCallCounter++;
                return default(int);
            };

            using (DataFeed dataFeed = new DataFeed(nativeImplementation))
            {
                if (!dataFeed.Connect(Host, Username, Password))
                    Assert.Inconclusive($"The {nameof(DataFeed.Connect)} method returned false.");

                nativeImplementation.Mdf_message_addIntPtrUInt64Int32 = (message, instrument_reference, message_reference) =>
                {
                    Assert.AreEqual(0UL, instrument_reference);
                    Assert.AreEqual((int)MessageReference.MDF_M_REQUEST, message_reference);
                    methodCallCounter++;
                    return default(int);
                };

                SubscribeMessage subscribeMessage = new SubscribeMessage(requestType, requestClasses);
                dataFeed.Request(subscribeMessage);
                //assert that the expected methods of the INativeImplementation were actually called
                Assert.AreEqual(3, methodCallCounter);
                //subscribe to some instruments
                subscribeMessage.InstrumentReferences = instrumentReferences;
                dataFeed.Request(subscribeMessage);
                Assert.AreEqual(7, methodCallCounter);

                //unsubcribe and assert that the MDF_M_UNSUBSCRIBE message reference was passed to the mdf_message_add method
                nativeImplementation.Mdf_message_addIntPtrUInt64Int32 = (message, instrument_reference, message_reference) =>
                {
                    Assert.AreEqual(0UL, instrument_reference);
                    Assert.AreEqual((int)MessageReference.MDF_M_UNSUBSCRIBE, message_reference);
                    methodCallCounter++;
                    return default(int);
                };
                dataFeed.Request(new UnsubscribeMessage(requestClasses));
                Assert.AreEqual(9, methodCallCounter);
            }
        }

        [TestMethod]
        public void GetAndSetConnectionTimeoutTest() => GetAndSetPropertyTest(MDF_OPTION.MDF_OPT_CONNECT_TIMEOUT, feed => feed.ConnectionTimeout, feed => feed.ConnectionTimeout = 10);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallConnectionTimeoutTest()
        {
            using (DataFeed dataFeed = new DataFeed(new StubINativeImplementation()))
                dataFeed.ConnectionTimeout = DataFeed.MinConnectionTimeout - 1;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeConnectionTimeoutTest()
        {
            using (DataFeed dataFeed = new DataFeed(new StubINativeImplementation()))
                dataFeed.ConnectionTimeout = DataFeed.MaxConnectionTimeout + 1;
        }

        [TestMethod]
        public void GetErrorCodeTest()
        {
            const Error ErrorCode = Error.MDF_ERR_MSG_TO_LARGE;
            StubINativeImplementation nativeImplementation = new StubINativeImplementation
            {
                Mdf_get_propertyIntPtrMDF_OPTIONIntPtrRef = (IntPtr handle, MDF_OPTION option, ref IntPtr value) =>
                {
                    //assert that the correct MDF_OPTION was passed to the mdf_get_property method
                    Assert.AreEqual(MDF_OPTION.MDF_OPT_ERROR, option);
                    value = (IntPtr)ErrorCode;
                    return default(int);
                }
            };

            using (DataFeed dataFeed = new DataFeed(nativeImplementation))
                Assert.AreEqual(ErrorCode, dataFeed.ErrorCode);
        }

        [TestMethod]
        public void GetAndSetHeartbeatIntervalTest() => GetAndSetPropertyTest(MDF_OPTION.MDF_OPT_HEARTBEAT_INTERVAL, feed => feed.HeartbeatInterval, feed => feed.HeartbeatInterval = 60);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallHeartbeatIntervalTest()
        {
            using (DataFeed dataFeed = new DataFeed(new StubINativeImplementation()))
                dataFeed.HeartbeatInterval = DataFeed.MinHeartbeatInterval - 1;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeHeartbeatIntervalTest()
        {
            using (DataFeed dataFeed = new DataFeed(new StubINativeImplementation()))
                dataFeed.HeartbeatInterval = DataFeed.MaxHeartbeatInterval + 1;
        }

        [TestMethod]
        public void GetAndSetMaximumMissedHeartbeats() => GetAndSetPropertyTest(MDF_OPTION.MDF_OPT_HEARTBEAT_MAX_MISSED, feed => feed.MaximumMissedHeartbeats, feed => feed.MaximumMissedHeartbeats = 50);

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToSmallMaximumMissedHeartbeatsTest()
        {
            using (DataFeed dataFeed = new DataFeed(new StubINativeImplementation()))
                dataFeed.MaximumMissedHeartbeats = DataFeed.MinMissedHeartbeats - 1;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ToLargeMaximumMissedHeartbeatsTest()
        {
            using (DataFeed dataFeed = new DataFeed(new StubINativeImplementation()))
                dataFeed.MaximumMissedHeartbeats = DataFeed.MaxMissedHeartbeats + 1;
        }

        [TestMethod]
        public void GetAndSetNoDelayTest()
        {
            bool setterInvoked = false;
            StubINativeImplementation nativeImplementation = new StubINativeImplementation()
            {
                Mdf_get_propertyIntPtrMDF_OPTIONIntPtrRef = (IntPtr handle, MDF_OPTION option, ref IntPtr value) =>
                {
                    Assert.AreEqual(MDF_OPTION.MDF_OPT_TCP_NODELAY, option);
                    value = new IntPtr(1);
                    return default(int);
                },
                Mdf_set_propertyIntPtrMDF_OPTIONIntPtr = (handle, option, value) =>
                {
                    Assert.AreEqual(MDF_OPTION.MDF_OPT_TCP_NODELAY, option);
                    setterInvoked = true;
                    return default(int);
                }
            };

            using (DataFeed dataFeed = new DataFeed(nativeImplementation))
            {
                //assert that the expected values are returned from the getter
                Assert.AreEqual(true, dataFeed.NoDelay);
                nativeImplementation.Mdf_get_propertyIntPtrMDF_OPTIONIntPtrRef = (IntPtr handle, MDF_OPTION option_, ref IntPtr value) =>
                {
                    value = new IntPtr(0);
                    return default(int);
                };
                Assert.AreEqual(false, dataFeed.NoDelay);
                //set the property
                dataFeed.NoDelay = true;
                //assert that the setter was invoked
                Assert.IsTrue(setterInvoked);
            }
        }

        [TestMethod]
        public void GetReceivedBytesTest() => GetUInt64PropertyTest(MDF_OPTION.MDF_OPT_RCV_BYTES, feed => feed.ReceivedBytes);

        [TestMethod]
        public void GetSentBytesTest() => GetUInt64PropertyTest(MDF_OPTION.MDF_OPT_SENT_BYTES, feed => feed.SentBytes);

        [TestMethod]
        public void GetTimeDifferenceTest()
        {
            const int Difference = 1;
            StubINativeImplementation nativeImplementation = new StubINativeImplementation
            {
                Mdf_get_propertyIntPtrMDF_OPTIONIntPtrRef = (IntPtr handle, MDF_OPTION option, ref IntPtr value) =>
                {
                    Assert.AreEqual(MDF_OPTION.MDF_OPT_TIME_DIFFERENCE, option);
                    value = (IntPtr)Difference;
                    return default(int);
                }
            };

            using (DataFeed dataFeed = new DataFeed(nativeImplementation))
                Assert.AreEqual(Difference, dataFeed.TimeDifference);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RequestBeforeConnectTest()
        {
            using (DataFeed dataFeed = new DataFeed(new StubINativeImplementation()))
                dataFeed.Request(new SubscribeMessage(RequestType.MDF_RT_FULL, new RequestClass[1] { RequestClass.MDF_RC_BASICDATA }));
        }

        [TestMethod]
        public void DisposeTest()
        {
            StubINativeImplementation nativeImplementation = new StubINativeImplementation();
            int messageDestroyCounter = 0;
            nativeImplementation.Mdf_message_destroyIntPtr = (message) => messageDestroyCounter++;
            int destroyCounter = 0;
            nativeImplementation.Mdf_destroyIntPtr = (handle) => destroyCounter++;

            DataFeed dataFeed = new DataFeed(nativeImplementation);
            Parallel.Invoke(new Action[5] { dataFeed.Dispose, dataFeed.Dispose, dataFeed.Dispose, dataFeed.Dispose, dataFeed.Dispose });

            //assert that the Dispose() method can be called called more than once, without the handles being destroyed more than once
            Assert.AreEqual(1, messageDestroyCounter);
            Assert.AreEqual(1, destroyCounter);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallConnectAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            dataFeed.Connect(Host, Username, Password);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallDisconnectAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            dataFeed.Disconnect();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallRecycleAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            dataFeed.Recycle(new ResponseMessage());
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotCallRequestAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            dataFeed.Request(new SubscribeMessage(RequestType.MDF_RT_FULL, new RequestClass[1] { RequestClass.MDF_RC_BASICDATA }));
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotHookupConnectionStatusChangedEventHandlerAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            dataFeed.ConnectionStatusChanged += (s, e) => { };
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSubscribeToDataAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            dataFeed.Data.Subscribe(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetConnectionTimeoutAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            int connectionTimeout = dataFeed.ConnectionTimeout;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetConnectionTimeoutAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            dataFeed.ConnectionTimeout = 10;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetErrorCodeAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            Error errorCode = dataFeed.ErrorCode;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetHeartbeatIntervalAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            int heartbeatInterval = dataFeed.HeartbeatInterval;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetHeartbeatIntervalAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            dataFeed.HeartbeatInterval = 60;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetMaximumMissedHeartbeatsAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            int maximumMissedHeartbeats = dataFeed.MaximumMissedHeartbeats;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetMaximumMissedHeartbeatsAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            dataFeed.MaximumMissedHeartbeats = 50;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetNoDelayAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            bool noDelay = dataFeed.NoDelay;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotSetNoDelayAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            dataFeed.NoDelay = true;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetReceivedBytesAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            ulong receivedBytes = dataFeed.ReceivedBytes;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetSentBytesAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            ulong sentBytes = dataFeed.SentBytes;
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CannotGetTimeDifferenceAfterDisposeTest()
        {
            DataFeed dataFeed = new DataFeed(new StubINativeImplementation());
            dataFeed.Dispose();
            int timeDifference = dataFeed.TimeDifference;
        }

        private void GetAndSetPropertyTest(MDF_OPTION option, Func<DataFeed, int> getter, Action<DataFeed> setter)
        {
            const int Value = 5;
            bool setterInvoked = false;
            StubINativeImplementation nativeImplementation = new StubINativeImplementation()
            {
                Mdf_get_propertyIntPtrMDF_OPTIONIntPtrRef = (IntPtr handle, MDF_OPTION option_, ref IntPtr value) =>
                {
                    Assert.AreEqual(option, option_);
                    value = new IntPtr(Value);
                    return default(int);
                },
                Mdf_set_propertyIntPtrMDF_OPTIONIntPtr = (handle, option_, value) =>
                {
                    Assert.AreEqual(option, option_);
                    setterInvoked = true;
                    return default(int);
                }
            };

            using (DataFeed dataFeed = new DataFeed(nativeImplementation))
            {
                //assert that the expected value is returned from the getter
                Assert.AreEqual(Value, getter(dataFeed));
                //set the property
                setter(dataFeed);
                //assert that the setter was invoked
                Assert.IsTrue(setterInvoked);
            }
        }

        private void GetUInt64PropertyTest(MDF_OPTION option, Func<DataFeed, ulong> getter)
        {
            const ulong Bytes = 100;
            StubINativeImplementation nativeImplementation = new StubINativeImplementation
            {
                Mdf_get_propertyIntPtrMDF_OPTIONIntPtrRef = (IntPtr handle, MDF_OPTION option_, ref IntPtr value) =>
                {
                    //assert that the correct MDF_OPTION was passed to the mdf_get_property method
                    Assert.AreEqual(option, option_);
                    value = (IntPtr)Bytes;
                    return default(int);
                }
            };

            //assert that the property returns the correct value
            using (DataFeed dataFeed = new DataFeed(nativeImplementation))
                Assert.AreEqual(Bytes, getter(dataFeed));
        }
    }
}
