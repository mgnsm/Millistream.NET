using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Millistream.Streaming.IntegrationTests
{
    [TestClass]
    public class DataFeedTests
    {
        private static readonly TimeSpan s_waitTimeout = TimeSpan.FromSeconds(10);

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ConnectAndDisconnectIntegrationTest()
        {
            string host = GetTestRunParameter("host");
            string username = GetTestRunParameter("username");
            string password = GetTestRunParameter("password");
            using DataFeed dataFeed = new DataFeed();
            using ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            dataFeed.ConsumeTimeout = 1;

            bool isConnected = false;
            void ConnectedHandler(object s, ConnectionStatusChangedEventArgs e)
            {
                if (e.ConnectionStatus == ConnectionStatus.MDF_STATUS_CONNECTED)
                {
                    isConnected = true;
                    manualResetEvent.Set();
                }
            }
            dataFeed.ConnectionStatusChanged += ConnectedHandler;
            //assert that the feed can be connected to
            Assert.IsTrue(dataFeed.Connect(host, username, password));
            //assert that a ConnectionStatusChanged is raised when the feed is connected 
            manualResetEvent.WaitOne(s_waitTimeout);
            Assert.IsTrue(isConnected);

            //connect again
            dataFeed.ConnectionStatusChanged -= ConnectedHandler;
            Assert.IsTrue(dataFeed.Connect(host, username, password));

            //disconnnect
            void DisconnectedHandler(object s, ConnectionStatusChangedEventArgs e)
            {
                if (e.ConnectionStatus == ConnectionStatus.MDF_STATUS_DISCONNECTED)
                {
                    isConnected = false;
                    manualResetEvent.Set();
                }
            }
            dataFeed.ConnectionStatusChanged += DisconnectedHandler;
            dataFeed.Disconnect();
            //assert that a ConnectionStatusChanged is raised when the feed is disconnected
            manualResetEvent.WaitOne(s_waitTimeout);
            Assert.IsFalse(isConnected);

            //connect again
            dataFeed.ConnectionStatusChanged -= DisconnectedHandler;
            dataFeed.ConnectionStatusChanged += ConnectedHandler;
            Assert.IsTrue(dataFeed.Connect(host, username, password));
            manualResetEvent.WaitOne(s_waitTimeout);
            Assert.IsTrue(isConnected);
            //disconnect again
            dataFeed.ConnectionStatusChanged -= ConnectedHandler;
            dataFeed.ConnectionStatusChanged += DisconnectedHandler;
            dataFeed.Disconnect();
            manualResetEvent.WaitOne(s_waitTimeout);
            Assert.IsFalse(isConnected);
        }

        [TestMethod]
        public void SubscribeAndUnsubscribeIntegrationTest()
        {
            const ulong InstrumentReference = 772;
            const string RequestId = "rid";
            RequestClass[] requestClasses = new RequestClass[2] { RequestClass.MDF_RC_BASICDATA, RequestClass.MDF_RC_QUOTE };
            ulong[] instrumentReferences = new ulong[1] { InstrumentReference };
            List<ResponseMessage> receivedResponseMessages = new List<ResponseMessage>();
            using DataFeed dataFeed = new DataFeed();
            using ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            dataFeed.ConsumeTimeout = 1;

            void OnNext(ResponseMessage message)
            {
                if (message.MessageReference == MessageReference.MDF_M_REQUESTFINISHED
                    && message.Fields.TryGetValue(Field.MDF_F_REQUESTID, out ReadOnlyMemory<byte> requestId)
                    && Encoding.UTF8.GetString(requestId.Span) == RequestId)
                    manualResetEvent.Set(); //signal when the request has been completed in full
                else
                    receivedResponseMessages.Add(message);
            }

            dataFeed.Data.Subscribe(new ResponseMessageObserver(OnNext, null, null));

            Connect(dataFeed);

            //subscribe to basic data and quotes for instrument 772 (ERIC B)
            dataFeed.Request(new SubscribeMessage(RequestType.MDF_RT_FULL, requestClasses)
            {
                InstrumentReferences = instrumentReferences,
                RequestId = RequestId
            });

            manualResetEvent.WaitOne(s_waitTimeout);
            //assert that some responses were received
            Assert.IsTrue(receivedResponseMessages.Count > 0);
            foreach (ResponseMessage receivedResponseMessage in receivedResponseMessages)
                Assert.AreEqual(InstrumentReference, receivedResponseMessage.InstrumentReference);

            //unsubscribe
            dataFeed.Request(new UnsubscribeMessage(requestClasses) { RequestId = RequestId });
            Assert.IsTrue(manualResetEvent.WaitOne(s_waitTimeout));
        }

        [TestMethod]
        public void WildcardSubscriptionsTest()
        {
            const string RequestId = "rid2";
            using DataFeed dataFeed = new DataFeed();
            using AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            dataFeed.ConsumeTimeout = 120;

            Dictionary<MessageReference, int> receivedMessageTypes = new Dictionary<MessageReference, int>();
            Dictionary<ulong, int> receivedInstrumentReferences = new Dictionary<ulong, int>();
            void OnNext(ResponseMessage message)
            {
                Assert.IsNotNull(message);
                switch (message.MessageReference)
                {
                    case MessageReference.MDF_M_REQUESTFINISHED:
                        if (message.Fields.TryGetValue(Field.MDF_F_REQUESTID, out ReadOnlyMemory<byte> requestId)
                            && Encoding.UTF8.GetString(requestId.Span) == RequestId)
                            autoResetEvent.Set(); //signal when the request has been completed in full
                        break;
                    default:
                        static void IncreaseCount<T>(Dictionary<T, int> dictionary, T key)
                        {
                            dictionary.TryGetValue(key, out int currentCount);
                            dictionary[key] = currentCount++;
                        }
                        IncreaseCount(receivedMessageTypes, message.MessageReference);
                        IncreaseCount(receivedInstrumentReferences, message.InstrumentReference);
                        break;
                }
                dataFeed.Recycle(message);
            }

            void UnsubscribeAndClear(RequestClass[] requestClasses)
            {
                dataFeed.Request(new UnsubscribeMessage(requestClasses) { RequestId = RequestId });
                Assert.IsTrue(autoResetEvent.WaitOne(s_waitTimeout));
                receivedMessageTypes.Clear();
                receivedInstrumentReferences.Clear();
                Assert.AreEqual(0, receivedMessageTypes.Count);
                Assert.AreEqual(0, receivedInstrumentReferences.Count);
            }

            dataFeed.Data.Subscribe(new ResponseMessageObserver(OnNext, null, null));

            Connect(dataFeed);

            //subscribe to a specific request class (MDF_RC_BASICDATA) for all instruments
            RequestClass[] requestClasses = new RequestClass[1] { RequestClass.MDF_RC_BASICDATA };
            dataFeed.Request(new SubscribeMessage(RequestType.MDF_RT_IMAGE, requestClasses) { RequestId = RequestId });
            autoResetEvent.WaitOne(s_waitTimeout);
            Assert.IsTrue(receivedMessageTypes.Count == 1);
            Assert.IsTrue(receivedInstrumentReferences.Count > 1);
            UnsubscribeAndClear(requestClasses);

            //subscribe to all messages for a particular instrument
            dataFeed.Request(new SubscribeMessage(RequestType.MDF_RT_IMAGE)
            { 
                RequestId = RequestId,
                InstrumentReferences = new ulong[1] { 772 }
            });
            autoResetEvent.WaitOne(s_waitTimeout);
            Assert.IsTrue(receivedMessageTypes.Count > 1);
            Assert.IsTrue(receivedInstrumentReferences.Count == 1);
            UnsubscribeAndClear(null);

            //subscribe to all request classes and all instruments that the account is entitled for
            dataFeed.Request(new SubscribeMessage(RequestType.MDF_RT_IMAGE) { RequestId = RequestId });
            autoResetEvent.WaitOne(s_waitTimeout);
            Assert.IsTrue(receivedMessageTypes.Count > 1);
            Assert.IsTrue(receivedInstrumentReferences.Count > 1);
            UnsubscribeAndClear(null);
        }

        private void Connect(IDataFeed dataFeed) =>
            Assert.IsTrue(dataFeed.Connect(GetTestRunParameter("host"), GetTestRunParameter("username"), GetTestRunParameter("password")),
                "Connect failed.");

        private string GetTestRunParameter(string parameterName)
        {
            string parameterValue = TestContext.Properties[parameterName] as string;
            if (string.IsNullOrEmpty(parameterValue))
                Assert.Fail($"No {parameterName} was specified in the .runsettings file.");
            return parameterValue;
        }
    }
}