using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MarketDataFeed = Millistream.Streaming.MarketDataFeed<object, object>;

namespace Millistream.Streaming.IntegrationTests
{
    [TestClass]
    public class MarketDataFeedTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void GetAndSetPropertiesTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed();

            Assert.AreEqual(0UL, mdf.ReceivedBytes);
            Assert.AreEqual(0UL, mdf.SentBytes);

            //FileDescriptor
            Assert.AreEqual(-1, mdf.FileDescriptor);
            Assert.IsTrue(mdf.Connect(TestContext.GetTestRunParameter("host")));
            Assert.AreNotEqual(-1, mdf.FileDescriptor);
            mdf.Disconnect();

            //ErrorCode
            Assert.AreEqual(Error.MDF_ERR_NO_ERROR, mdf.ErrorCode);
            const Error ErrorCode = Error.MDF_ERR_MSG_TO_LARGE;
            mdf.ErrorCode = ErrorCode;
            Assert.AreEqual(ErrorCode, mdf.ErrorCode);

            //ReceivedBytes
            Assert.IsTrue(mdf.ReceivedBytes > 0);
            const ulong ReceivedBytes = ulong.MaxValue;
            mdf.ReceivedBytes = ReceivedBytes;
            Assert.AreEqual(ReceivedBytes, mdf.ReceivedBytes);

            //SentBytes
            Assert.IsTrue(mdf.SentBytes > 0);
            const ulong SentBytes = ulong.MinValue;
            mdf.SentBytes = SentBytes;
            Assert.AreEqual(SentBytes, mdf.SentBytes);

            //ConnectionTimeout
            Assert.AreEqual(5, mdf.ConnectionTimeout);
            const int ConnectionTimeout = 10;
            mdf.ConnectionTimeout = ConnectionTimeout;
            Assert.AreEqual(ConnectionTimeout, mdf.ConnectionTimeout);

            //HeartbeatInterval
            Assert.AreEqual(30, mdf.HeartbeatInterval);
            const int HeartbeatInterval = 85000;
            mdf.HeartbeatInterval = HeartbeatInterval;
            Assert.AreEqual(HeartbeatInterval, mdf.HeartbeatInterval);

            //MaximumMissedHeartbeats
            Assert.AreEqual(2, mdf.MaximumMissedHeartbeats);
            const int MaximumMissedHeartbeats = 85;
            mdf.MaximumMissedHeartbeats = MaximumMissedHeartbeats;
            Assert.AreEqual(MaximumMissedHeartbeats, mdf.MaximumMissedHeartbeats);

            //NoDelay
            Assert.IsFalse(mdf.NoDelay);
            mdf.NoDelay = true;
            Assert.IsTrue(mdf.NoDelay);

            //NoEncryption
            Assert.IsFalse(mdf.NoEncryption);
            mdf.NoEncryption = true;
            Assert.IsTrue(mdf.NoEncryption);

            //TimeDifference
            _ = mdf.TimeDifference;

            //BindAddress
            Assert.IsNull(mdf.BindAddress);
            const string BindAddress = "123";
            mdf.BindAddress = BindAddress;
            Assert.AreEqual(BindAddress, mdf.BindAddress);

            //TimeDifferenceNs (requires version 1.0.24 of the native library which currently only comes as a pre-built binary on Linux)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                _ = mdf.TimeDifferenceNs;
        }

        [TestMethod]
        public void ConnectAndLogOnTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed();
            using Message message = new Message();
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_BASICDATA));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Field.MDF_F_INSREFLIST, "772"));
            Assert.IsFalse(mdf.Send(message));
            //connect
            Assert.IsTrue(mdf.Connect(TestContext.GetTestRunParameter("host")));
            //log on
            Assert.IsTrue(LogOn(mdf, message, TestContext));
            //log off
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_LOGOFF));
            Assert.IsTrue(mdf.Send(message));
            Assert.IsTrue(Consume(mdf, MessageReference.MDF_M_LOGOFF));
            mdf.Disconnect();
            mdf.Disconnect();
            mdf.Disconnect();
        }

        [TestMethod]
        public void SendMultipleMessagesTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed();
            //connect
            Assert.IsTrue(mdf.Connect(TestContext.GetTestRunParameter("host")));

            using Message message = new Message();
            //log on
            Assert.IsTrue(LogOn(mdf, message, TestContext));

            List<string> requestIds = new List<string>(3)
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_BASICDATA));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Field.MDF_F_INSREFLIST, "772"));
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTID, requestIds[0]));

            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_QUOTE));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Field.MDF_F_INSREFLIST, "1146"));
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTID, requestIds[1]));

            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_TRADE));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Field.MDF_F_INSREFLIST, "105"));
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTID, requestIds[2]));

            Assert.AreEqual(3, message.ActiveCount);

            //send a request with 3 messages
            Assert.IsTrue(mdf.Send(message));
            //consume the response messages
            Assert.IsTrue(Consume(mdf, requestIds));

            message.Reset();
            Assert.AreEqual(0, message.ActiveCount);

            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_ORDER));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Field.MDF_F_INSREFLIST, "354"));
            string requestId = Guid.NewGuid().ToString();
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTID, requestId));
            Assert.AreEqual(1, message.ActiveCount);

            //send another request with a single message
            Assert.IsTrue(mdf.Send(message));
            Assert.IsTrue(Consume(mdf, new string[1] { requestId }));

            mdf.Disconnect();
        }

        [TestMethod]
        public void DataCallbackTest()
        {
            const string UserData = "sample data...";
            const string RequestId = "rid";

            using MarketDataFeed mdf = new MarketDataFeed();
            //connect
            Assert.IsTrue(mdf.Connect(TestContext.GetTestRunParameter("host")));
            //log on
            using Message message = new Message();
            Assert.IsTrue(LogOn(mdf, message, TestContext));
            //set some custom user callback data
            mdf.CallbackUserData = UserData;
            Assert.AreEqual(UserData, mdf.CallbackUserData);
            //register a callback
            bool requestFinished = false;
            void OnDataReceived(object userData, MarketDataFeed<object, object> mdf)
            {
                Assert.AreEqual(UserData, userData);
                while (mdf.GetNextMessage(out int _, out int _, out ulong _))
                    while (mdf.GetNextField(out Field field, out ReadOnlySpan<byte> value))
                        if (field == Field.MDF_F_REQUESTID && Encoding.UTF8.GetString(value.ToArray()) == RequestId)
                            requestFinished = true;
            }
            mdf.DataCallback = OnDataReceived;
            //request some data
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_BASICDATA));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Field.MDF_F_INSREFLIST, "772"));
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTID, RequestId));
            Assert.IsTrue(mdf.Send(message));

            //invoke the callback by consuming
            do
            {
                if (mdf.Consume(10) == -1)
                    break;
            } while (!requestFinished);

            Assert.IsTrue(requestFinished);
        }

        [TestMethod]
        public void StatusCallbackTest()
        {
            HashSet<ConnectionStatus> receivedStatuses = new HashSet<ConnectionStatus>();
            using MarketDataFeed<object, HashSet<ConnectionStatus>> mdf = new MarketDataFeed<object, HashSet<ConnectionStatus>>()
            {
                StatusCallbackUserData = receivedStatuses
            };

            static void OnStatusChanged(HashSet<ConnectionStatus> statuses, ConnectionStatus status, string host, string ip)
            {
                if (!statuses.Contains(status))
                    statuses.Add(status);
            }
            mdf.StatusCallback = OnStatusChanged;

            //connect
            Assert.IsTrue(mdf.Connect(TestContext.GetTestRunParameter("host")));
            Assert.AreEqual(4, receivedStatuses.Count);
            Assert.IsTrue(receivedStatuses.Contains(ConnectionStatus.MDF_STATUS_LOOKUP));
            Assert.IsTrue(receivedStatuses.Contains(ConnectionStatus.MDF_STATUS_CONNECTING));
            Assert.IsTrue(receivedStatuses.Contains(ConnectionStatus.MDF_STATUS_CONNECTED));
            Assert.IsTrue(receivedStatuses.Contains(ConnectionStatus.MDF_STATUS_READYTOLOGON));

            //disconnect
            mdf.Disconnect();
            Assert.AreEqual(5, receivedStatuses.Count);
            Assert.IsTrue(receivedStatuses.Contains(ConnectionStatus.MDF_STATUS_DISCONNECTED));
        }

        [TestMethod]
        public void SubscribeAndUnsubscribeTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed();
            using Message message = new Message();
            //connect
            Assert.IsTrue(mdf.Connect(TestContext.GetTestRunParameter("host")));
            //log on
            Assert.IsTrue(LogOn(mdf, message, TestContext));
            //subscribe to basic data and quotes for instrument 772 (ERIC B)
            string requestId = "rid";
            RequestClass[] requestClasses = new RequestClass[2] { RequestClass.MDF_RC_BASICDATA, RequestClass.MDF_RC_QUOTE };
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(message.AddList(requestClasses));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_FULL));
            const string InsRef = "772";
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, InsRef));
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTID, requestId));
            Assert.IsTrue(mdf.Send(message));
            message.Reset();
            //consume the request
            Assert.IsTrue(Consume(mdf, new string[1] { requestId }));
            //unsubscribe
            requestId = "rid2";
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_UNSUBSCRIBE));
            Assert.IsTrue(message.AddList(requestClasses));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, InsRef));
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTID, requestId));
            Assert.IsTrue(mdf.Send(message));
            message.Reset();
            //consume
            Assert.IsTrue(Consume(mdf, new string[1] { requestId }));
        }

        [TestMethod]
        public void WildcardSubscriptionsTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed();
            using Message message = new Message();
            //connect
            Assert.IsTrue(mdf.Connect(TestContext.GetTestRunParameter("host")));
            //log on
            Assert.IsTrue(LogOn(mdf, message, TestContext));
            
            //subscribe to a specific request class (MDF_RC_BASICDATA) for a couple of instruments
            string requestId = "rid";
            RequestClass[] requestClasses = new RequestClass[1] { RequestClass.MDF_RC_BASICDATA };
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(message.AddList(requestClasses));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTID, requestId));
            ulong[] instrumentReferences = new ulong[4] { 354, 772, 928, 1168 };
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, instrumentReferences));
            Assert.IsTrue(mdf.Send(message));
            message.Reset();
            //consume
            Dictionary<int, int> receivedMessageTypes = new Dictionary<int, int>();
            Dictionary<ulong, int> receivedInstrumentReferences = new Dictionary<ulong, int>();

            bool ConsumeAndCount(string requestId)
            {
                const int TimeoutInSeconds = 120;
                bool requestFinished = false;
                DateTime time = DateTime.UtcNow;
                do
                {
                    int ret = mdf.Consume(10);
                    if (ret == -1)
                        break;

                    if (ret == 1)
                    {
                        while (mdf.GetNextMessage(out int mref, out int _, out ulong instrumentReference))
                        {
                            while (mdf.GetNextField(out uint field, out ReadOnlySpan<byte> value))
                            {
                                if (field == (uint)Field.MDF_F_REQUESTID && Encoding.UTF8.GetString(value.ToArray()) == requestId)
                                    requestFinished = true;
                            }

                            if (mref != (int)MessageReference.MDF_M_REQUESTFINISHED)
                            {
                                static void IncreaseCount<T>(Dictionary<T, int> dictionary, T key)
                                {
                                    dictionary.TryGetValue(key, out int currentCount);
                                    dictionary[key] = ++currentCount;
                                }
                                IncreaseCount(receivedMessageTypes, mref);
                                IncreaseCount(receivedInstrumentReferences, instrumentReference);
                            }
                        }
                    }
                } while (!requestFinished && DateTime.UtcNow.Subtract(time).TotalSeconds < TimeoutInSeconds);

                return requestFinished;
            }

            Assert.IsTrue(ConsumeAndCount(requestId));
            Assert.IsTrue(receivedMessageTypes.Count == 1);
            Assert.IsTrue(receivedInstrumentReferences.Count > 1);

            void UnsubscribeAndClear(RequestClass[] requestClasses, ulong[] instrumentReferences)
            {
                string requestId = Guid.NewGuid().ToString();
                Assert.IsTrue(message.Add(0, MessageReference.MDF_M_UNSUBSCRIBE));
                if (requestClasses != null)
                    Assert.IsTrue(message.AddList(requestClasses));
                if (instrumentReferences != null)
                    Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, instrumentReferences));
                Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTID, requestId));
                Assert.IsTrue(mdf.Send(message));
                message.Reset();
                Assert.IsTrue(Consume(mdf, new string[1] { requestId }));
                receivedMessageTypes.Clear();
                receivedInstrumentReferences.Clear();
                Assert.AreEqual(0, receivedMessageTypes.Count);
                Assert.AreEqual(0, receivedInstrumentReferences.Count);
            }
            UnsubscribeAndClear(requestClasses, instrumentReferences);

            //subscribe to all messages for a particular instrument
            requestId = "rid2";
            instrumentReferences = new ulong[1] { 772 };
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.All));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddList(Field.MDF_F_INSREFLIST, new ulong[1] { 772 }));
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTID, requestId));
            Assert.IsTrue(mdf.Send(message));
            message.Reset();
            Assert.IsTrue(ConsumeAndCount(requestId));
            Assert.IsTrue(receivedMessageTypes.Count > 1);
            Assert.IsTrue(receivedInstrumentReferences.Count == 1);
            UnsubscribeAndClear(null, instrumentReferences);
        }

        [TestMethod]
        public void CreateInstrumentsTest()
        {
            using MarketDataFeed mdf = new MarketDataFeed();
            using Message message = new Message();
            //connect
            Assert.IsTrue(mdf.Connect(TestContext.GetTestRunParameter("host")));
            //log on
            Assert.IsTrue(LogOn(mdf, message, TestContext));
            //send request
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_REQUEST));
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_INSREF));
            Assert.IsTrue(message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_STREAM));
            Assert.IsTrue(message.AddString(Field.MDF_F_REQUESTID, "rid"));
            Assert.IsTrue(mdf.Send(message));
            message.Reset();
            //consume
            const int TimeoutInSeconds = 30;
            bool succeeded = false;
            DateTime time = DateTime.UtcNow;
            do
            {
                int ret = mdf.Consume(10);
                if (ret == -1)
                    break;

                if (ret == 1)
                {
                    while (mdf.GetNextMessage(out MessageReference messageReference, out MessageClasses _, out ulong instrumentReference))
                    {
                        if (messageReference == MessageReference.MDF_M_INSREF)
                            succeeded = true;

                        while (mdf.GetNextField(out uint field, out ReadOnlySpan<byte> value))
                        {
                            //no permissions to create instruments
                            if (field == (uint)Field.MDF_F_REQUESTSTATUS
                                && Utf8Parser.TryParse(value, out uint @uint, out int _)
                                && @uint == 101)
                                succeeded = true;
                        }
                    }
                }
            } while (!succeeded && DateTime.UtcNow.Subtract(time).TotalSeconds < TimeoutInSeconds);

            Assert.IsTrue(succeeded);
        }

        private static bool LogOn(MarketDataFeed mdf, Message message, TestContext testContext)
        {
            Assert.IsTrue(message.Add(0, MessageReference.MDF_M_LOGON));
            Assert.IsTrue(message.AddString(Field.MDF_F_USERNAME, testContext.GetTestRunParameter("username")));
            Assert.IsTrue(message.AddString(Field.MDF_F_PASSWORD, testContext.GetTestRunParameter("password")));
            Assert.IsTrue(mdf.Send(message));
            message.Reset();
            return Consume(mdf, MessageReference.MDF_M_LOGONGREETING);
        }

        private static bool Consume(MarketDataFeed mdf, MessageReference messageReference)
        {
            if (mdf == null)
                throw new ArgumentNullException(nameof(mdf));

            const int TimeoutInSeconds = 10;
            DateTime time = DateTime.UtcNow;
            do
            {
                int ret = mdf.Consume(1);
                switch (ret)
                {
                    case 1:
                        while (mdf.GetNextMessage(out int mref, out int _, out ulong _))
                            if (mref == (int)messageReference)
                                return true;
                        break;
                    case -1:
                        return false;
                }

            } while (DateTime.UtcNow.Subtract(time).TotalSeconds < TimeoutInSeconds);

            return false;
        }

        private static bool Consume(MarketDataFeed mdf, IReadOnlyCollection<string> requestIds)
        {
            if (mdf == null)
                throw new ArgumentNullException(nameof(mdf));

            if (requestIds == null)
                throw new ArgumentNullException(nameof(requestIds));

            const int TimeoutInSeconds = 90;
            int numberOfFinishedRequests = 0;
            DateTime time = DateTime.UtcNow;
            do
            {
                int ret = mdf.Consume(10);
                if (ret == -1)
                    break;

                if (ret == 1)
                    while (mdf.GetNextMessage(out int _, out int _, out ulong _))
                        while (mdf.GetNextField(out Field field, out ReadOnlySpan<byte> value))
                            if (field == Field.MDF_F_REQUESTID && requestIds.Contains(Encoding.UTF8.GetString(value.ToArray())))
                                numberOfFinishedRequests++;
            } while (numberOfFinishedRequests < requestIds.Count && DateTime.UtcNow.Subtract(time).TotalSeconds < TimeoutInSeconds);

            return numberOfFinishedRequests == requestIds.Count;
        }
    }
}