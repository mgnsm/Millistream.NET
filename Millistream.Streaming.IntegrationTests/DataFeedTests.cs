using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Millistream.Streaming.IntegrationTests
{
    [TestClass]
    public class DataFeedTests
    {
        private static readonly TimeSpan _waitTimeout = TimeSpan.FromSeconds(10);

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ConnectAndDisconnectIntegrationTest()
        {
            string host = GetTestRunParameter("host");
            string username = GetTestRunParameter("username");
            string password = GetTestRunParameter("password");
            using (DataFeed dataFeed = new DataFeed())
            using (ManualResetEvent manualResetEvent = new ManualResetEvent(false))
            {
                dataFeed.ConsumeTimeout = 1;
   
                bool isConnected = false;
                void connectedHandler(object s, ConnectionStatusChangedEventArgs e)
                {
                    if (e.ConnectionStatus == ConnectionStatus.MDF_STATUS_CONNECTED)
                    {
                        isConnected = true;
                        manualResetEvent.Set();
                    }
                }
                dataFeed.ConnectionStatusChanged += connectedHandler;
                //assert that the feed can be connected to
                Assert.IsTrue(dataFeed.Connect(host, username, password));
                //assert that a ConnectionStatusChanged is raised when the feed is connected 
                manualResetEvent.WaitOne(_waitTimeout);
                Assert.IsTrue(isConnected);
                
                //connect again
                dataFeed.ConnectionStatusChanged -= connectedHandler;
                Assert.IsTrue(dataFeed.Connect(host, username, password));

                //disconnnect
                void disconnectedHandler(object s, ConnectionStatusChangedEventArgs e)
                {
                    if (e.ConnectionStatus == ConnectionStatus.MDF_STATUS_DISCONNECTED)
                    {
                        isConnected = false;
                        manualResetEvent.Set();
                    }
                }
                dataFeed.ConnectionStatusChanged += disconnectedHandler;
                dataFeed.Disconnect();
                //assert that a ConnectionStatusChanged is raised when the feed is disconnected
                manualResetEvent.WaitOne(_waitTimeout);
                Assert.IsFalse(isConnected);

                //connect again
                dataFeed.ConnectionStatusChanged -= disconnectedHandler;
                dataFeed.ConnectionStatusChanged += connectedHandler;
                Assert.IsTrue(dataFeed.Connect(host, username, password));
                manualResetEvent.WaitOne(_waitTimeout);
                Assert.IsTrue(isConnected);
                //disconnect again
                dataFeed.ConnectionStatusChanged -= connectedHandler;
                dataFeed.ConnectionStatusChanged += disconnectedHandler;
                dataFeed.Disconnect();
                manualResetEvent.WaitOne(_waitTimeout);
                Assert.IsFalse(isConnected);
            }
        }

        [TestMethod]
        public void SubscribeAndUnsubscribeIntegrationTest()
        {
            const ulong InstrumentReference = 772;
            const string RequestId = "rid";
            RequestClass[] requestClasses = new RequestClass[2] { RequestClass.MDF_RC_BASICDATA, RequestClass.MDF_RC_QUOTE };
            ulong[] instrumentReferences = new ulong[1] { InstrumentReference };
            List<ResponseMessage> receivedResponseMessages = new List<ResponseMessage>();
            using (DataFeed dataFeed = new DataFeed())
            using (ManualResetEvent manualResetEvent = new ManualResetEvent(false))
            {
                dataFeed.ConsumeTimeout = 1;

                dataFeed.DataReceived += (s, e) =>
                {
                    if (e.Message.MessageReference == MessageReference.MDF_M_REQUESTFINISHED
                     && e.Message.Fields.TryGetValue(Field.MDF_F_REQUESTID, out string requestId)
                     && requestId == RequestId)
                        manualResetEvent.Set(); //signal when the request has been completed in full
                    else
                        receivedResponseMessages.Add(e.Message);
                };

                Assert.IsTrue(dataFeed.Connect(GetTestRunParameter("host"), GetTestRunParameter("username"), GetTestRunParameter("password")),
                    "Connect failed.");

                //subscribe to basic data and quotes for instrument 772 (ERIC B)
                dataFeed.Request(new SubscribeMessage(RequestType.MDF_RT_FULL, requestClasses)
                {
                    InstrumentReferences = instrumentReferences,
                    RequestId = RequestId
                });

                manualResetEvent.WaitOne(_waitTimeout);
                //assert that some responses were received
                Assert.IsTrue(receivedResponseMessages.Count > 0);
                foreach (ResponseMessage receivedResponseMessage in receivedResponseMessages)
                    Assert.AreEqual(InstrumentReference, receivedResponseMessage.InstrumentReference);

                //unsubscribe
                dataFeed.Request(new UnsubscribeMessage(requestClasses) { RequestId = RequestId });
                Assert.IsTrue(manualResetEvent.WaitOne(_waitTimeout));
            }
        }

        private string GetTestRunParameter(string parameterName)
        {
            string parameterValue = TestContext.Properties.TryGetValue(parameterName, out object value) ? value as string : default(string);
            if (string.IsNullOrEmpty(parameterValue))
                Assert.Fail($"No {parameterName} was specified in the .runsettings file.");
            return parameterValue;
        }
    }
}