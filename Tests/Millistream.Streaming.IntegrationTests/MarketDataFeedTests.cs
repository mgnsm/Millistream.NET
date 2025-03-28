﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using MarketDataFeed = Millistream.Streaming.MarketDataFeed<object, object>;

namespace Millistream.Streaming.IntegrationTests
{
    [TestClass]
    public class MarketDataFeedTests
    {
        public TestContext TestContext { get; set; }

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _) => AppDomain.MonitoringIsEnabled = true;

        [TestMethod]
        public void CreateMarketDataFeed()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using MarketDataFeed mdf = new("libmdf.so.0");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using MarketDataFeed mdf = new("libmdf-0.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                using MarketDataFeed mdf = new("libmdf.0.dylib");
            }
            else
                throw new PlatformNotSupportedException();
        }

        [TestMethod]
        public void GetAndSetPropertiesTest()
        {
            using MarketDataFeed mdf = new();

            Assert.AreEqual(0UL, mdf.ReceivedBytes);
            Assert.AreEqual(0UL, mdf.SentBytes);
            Assert.AreEqual(0U, mdf.ReadBufferSize);
            Assert.AreEqual(2048U, mdf.ReadBufferMaxSize);
            Assert.AreEqual(default, mdf.BindAddress);
            Assert.AreEqual(default, mdf.Cipher);
            Assert.AreEqual(default, mdf.ConnectedHost);
            Assert.AreEqual(default, mdf.ConnectedIPAddress);
            Assert.AreEqual(default, mdf.MessageDigest);

            //HandleDelay
            Assert.IsFalse(mdf.HandleDelay);
            mdf.HandleDelay = true;

            //Delay
            _ = mdf.Delay;

            //FileDescriptor
            Assert.AreEqual(-1, mdf.FileDescriptor);
            string host = GetTestRunParameter("host");
            Assert.IsTrue(mdf.Connect(host));
            Assert.AreNotEqual(-1, mdf.FileDescriptor);

            using Message message = new();
            LogOn(mdf, message);
            Assert.IsTrue(mdf.HandleDelay);
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
            string bindAddress = "123";
            mdf.BindAddress = bindAddress;
            Assert.AreEqual(bindAddress, mdf.BindAddress);
            mdf.BindAddress = null;
            Assert.IsNull(mdf.BindAddress);
            bindAddress = "åäö.12";
            mdf.BindAddress = bindAddress;
            Assert.AreEqual(bindAddress, mdf.BindAddress);

            _ = mdf.TimeDifferenceNs;

            //ReadBufferMaxSize
            mdf.ReadBufferMaxSize = uint.MaxValue;
            Assert.AreEqual(uint.MaxValue, mdf.ReadBufferMaxSize);

            //Allocations
            long allocatedBytes = GetTotalAllocatedBytes();
            _ = mdf.FileDescriptor;

            mdf.ErrorCode = Error.MDF_ERR_MSG_OOB;
            _ = mdf.ErrorCode;

            mdf.ReceivedBytes += 100;
            _ = mdf.ReceivedBytes;

            mdf.SentBytes -= 100;
            _ = mdf.SentBytes;

            mdf.ConnectionTimeout = 35;
            _ = mdf.ConnectionTimeout;

            mdf.HeartbeatInterval = 1000;
            _ = mdf.HeartbeatInterval;

            mdf.MaximumMissedHeartbeats = 50;
            _ = mdf.MaximumMissedHeartbeats;

            mdf.NoDelay = false;
            _ = mdf.NoDelay;

            mdf.NoEncryption = true;
            _ = mdf.NoEncryption;

            _ = mdf.TimeDifference;

            mdf.BindAddress = "abc";

            _ = mdf.Timeout;

            mdf.HandleDelay = false;
            _ = mdf.HandleDelay;

            _ = mdf.Delay;

            _ = mdf.MessageClass;

            Assert.AreEqual(allocatedBytes, GetTotalAllocatedBytes());

            Assert.IsTrue(host.AsSpan().Slice(0, host.LastIndexOf(':')).SequenceEqual(mdf.ConnectedHost.AsSpan()));
            Assert.IsNotNull(mdf.ConnectedIPAddress);
        }

        [TestMethod]
        public void GetAndSetMessageDigestsAndCiphersTest()
        {
            using MarketDataFeed mdf = new();
            Assert.IsFalse(string.IsNullOrEmpty(mdf.MessageDigests));
            Assert.IsFalse(string.IsNullOrEmpty(mdf.Ciphers));

            char[] separator = new char[1] { ',' };
            string[] digests = mdf.MessageDigests.Split(separator);
            Assert.IsTrue(digests != null && digests.Length > 0);
            string prefferedDigest = digests[0];
            Assert.IsFalse(string.IsNullOrEmpty(prefferedDigest));

            string[] ciphers = mdf.Ciphers.Split(separator);
            Assert.IsTrue(ciphers != null && ciphers.Length > 0);
            string preferredChipher = ciphers[0];
            Assert.IsFalse(string.IsNullOrEmpty(preferredChipher));

            mdf.MessageDigests = prefferedDigest;
            mdf.Ciphers = preferredChipher;
            Assert.IsTrue(mdf.Connect(GetTestRunParameter("host")));
            Assert.AreEqual(prefferedDigest, mdf.MessageDigest);
            Assert.AreEqual(preferredChipher, mdf.Cipher);

            mdf.Disconnect();
            mdf.MessageDigests = null;
            Assert.IsFalse(string.IsNullOrEmpty(mdf.MessageDigests));
            mdf.Ciphers = null;
            Assert.IsFalse(string.IsNullOrEmpty(mdf.Ciphers));
            Assert.IsTrue(mdf.Connect(GetTestRunParameter("host")));
            Assert.IsFalse(string.IsNullOrEmpty(mdf.MessageDigest));
            Assert.IsFalse(string.IsNullOrEmpty(mdf.Cipher));
        }

        [TestMethod]
        public void ConnectAndLogOnTest()
        {
            using MarketDataFeed mdf = new();
            using Message message = new();
            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTCLASS, RequestClasses.MDF_RC_BASICDATA));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Fields.MDF_F_INSREFLIST, "772"));
            Assert.IsFalse(mdf.Send(message));
            //connect
            Assert.IsTrue(mdf.Connect(GetTestRunParameter("host")));
            //log on
            Assert.IsTrue(LogOn(mdf, message));
            //log off
            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_LOGOFF));
            Assert.IsTrue(mdf.Send(message));
            Assert.IsTrue(Consume(mdf, MessageReferences.MDF_M_LOGOFF));
            mdf.Disconnect();
            mdf.Disconnect();
            mdf.Disconnect();
        }

        [TestMethod]
        public void SendMultipleMessagesTest()
        {
            using MarketDataFeed mdf = new();
            //connect
            Assert.IsTrue(mdf.Connect(GetTestRunParameter("host")));

            using Message message = new();
            //log on
            Assert.IsTrue(LogOn(mdf, message));

            List<string> requestIds = new(3)
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };
            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTCLASS, RequestClasses.MDF_RC_BASICDATA));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Fields.MDF_F_INSREFLIST, "772"));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTID, requestIds[0]));
            Assert.AreEqual(1, message.ActiveCount);
            Assert.AreEqual(4, message.FieldCount);

            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTCLASS, RequestClasses.MDF_RC_QUOTE));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Fields.MDF_F_INSREFLIST, "1146"));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTID, requestIds[1]));
            Assert.AreEqual(2, message.ActiveCount);
            Assert.AreEqual(4, message.FieldCount);

            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTCLASS, RequestClasses.MDF_RC_TRADE));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Fields.MDF_F_INSREFLIST, "105"));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTID, requestIds[2]));
            Assert.AreEqual(3, message.ActiveCount);
            Assert.AreEqual(4, message.FieldCount);

            //send a request with 3 messages
            Assert.IsTrue(mdf.Send(message));
            //consume the response messages
            Assert.IsTrue(Consume(mdf, requestIds));

            message.Reset();
            Assert.AreEqual(0, message.ActiveCount);
            Assert.AreEqual(0, message.FieldCount);

            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTCLASS, RequestClasses.MDF_RC_ORDER));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Fields.MDF_F_INSREFLIST, "354"));
            string requestId = Guid.NewGuid().ToString();
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTID, requestId));
            Assert.AreEqual(1, message.ActiveCount);
            Assert.AreEqual(4, message.FieldCount);

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

            using MarketDataFeed mdf = new();
            //connect
            Assert.IsTrue(mdf.Connect(GetTestRunParameter("host")));
            //log on
            using Message message = new();
            Assert.IsTrue(LogOn(mdf, message));
            //set some custom user callback data
            mdf.CallbackUserData = UserData;
            Assert.AreEqual(UserData, mdf.CallbackUserData);
            //register a callback
            bool requestFinished = false;
            void OnDataReceived(object userData, MarketDataFeed<object, object> mdf)
            {
                Assert.AreEqual(UserData, userData);
                while (mdf.GetNextMessage(out int _, out int mclass, out ulong _))
                {
                    Assert.AreEqual((ulong)mclass, mdf.MessageClass);
                    while (mdf.GetNextField(out uint field, out ReadOnlySpan<byte> value))
                        if (field == Fields.MDF_F_REQUESTID && Encoding.UTF8.GetString(value.ToArray()) == RequestId)
                            requestFinished = true;
                }
            }
            mdf.DataCallback = OnDataReceived;
            //request some data
            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTCLASS, RequestClasses.MDF_RC_BASICDATA));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Fields.MDF_F_INSREFLIST, "772"));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTID, RequestId));
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
            string host = GetTestRunParameter("host");
            Assert.IsFalse(string.IsNullOrEmpty(host));
            int index = host.LastIndexOf(':');
            string hostWithoutPort = index > -1 ? host.Substring(0, index) : host;

            HashSet<ConnectionStatus> receivedStatuses = new();
            using MarketDataFeed<object, HashSet<ConnectionStatus>> mdf = new()
            {
                StatusCallbackUserData = receivedStatuses
            };

            void OnStatusChanged(HashSet<ConnectionStatus> statuses, ConnectionStatus status, ReadOnlySpan<byte> host, ReadOnlySpan<byte> ip)
            {
                statuses.Add(status);

                if (host != default)
                    Assert.AreEqual(hostWithoutPort, Encoding.UTF8.GetString(host.ToArray()));

                if (ip != default)
                    Assert.IsTrue(IPAddress.TryParse(Encoding.UTF8.GetString(ip.ToArray()), out _));
            }
            mdf.StatusCallback = OnStatusChanged;

            //connect
            Assert.IsTrue(mdf.Connect(host));
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
            using MarketDataFeed mdf = new();
            using Message message = new();
            //connect
            Assert.IsTrue(mdf.Connect(GetTestRunParameter("host")));
            //log on
            Assert.IsTrue(LogOn(mdf, message));
            //subscribe to basic data and quotes for instrument 772 (ERIC B)
            string requestId = "rid";
            const string RequestClasses = "4 1";
            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_REQUEST));
            Assert.IsTrue(message.AddList(Fields.MDF_F_REQUESTCLASS, RequestClasses));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_FULL));
            const string InsRef = "772";
            Assert.IsTrue(message.AddList(Fields.MDF_F_INSREFLIST, InsRef));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTID, requestId));
            Assert.IsTrue(mdf.Send(message));
            message.Reset();
            //consume the request
            Assert.IsTrue(Consume(mdf, new string[1] { requestId }));
            //unsubscribe
            requestId = "rid2";
            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_UNSUBSCRIBE));
            Assert.IsTrue(message.AddList(Fields.MDF_F_REQUESTCLASS, RequestClasses));
            Assert.IsTrue(message.AddList(Fields.MDF_F_INSREFLIST, InsRef));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTID, requestId));
            Assert.IsTrue(mdf.Send(message));
            message.Reset();
            //consume
            Assert.IsTrue(Consume(mdf, new string[1] { requestId }));
        }

        [TestMethod]
        public void WildcardSubscriptionsTest()
        {
            using MarketDataFeed mdf = new();
            using Message message = new();
            //connect
            Assert.IsTrue(mdf.Connect(GetTestRunParameter("host")));
            //log on
            Assert.IsTrue(LogOn(mdf, message));
            
            //subscribe to a specific request class (MDF_RC_BASICDATA) for a couple of instruments
            string requestId = "rid";
            const string RequestClass = "4";
            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_REQUEST));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTCLASS, RequestClass));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTID, requestId));
            string instrumentReferences = "354 772 928 1168";
            Assert.IsTrue(message.AddList(Fields.MDF_F_INSREFLIST, instrumentReferences));
            Assert.IsTrue(mdf.Send(message));
            message.Reset();
            //consume
            Dictionary<int, int> receivedMessageTypes = new();
            Dictionary<ulong, int> receivedInstrumentReferences = new();

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
                        while (mdf.GetNextMessage(out ushort mref, out ulong instrumentReference))
                        {
                            while (mdf.GetNextField(out uint field, out ReadOnlySpan<byte> value))
                            {
                                if (field == Fields.MDF_F_REQUESTID && Encoding.UTF8.GetString(value.ToArray()) == requestId)
                                    requestFinished = true;
                            }

                            if (mref != MessageReferences.MDF_M_REQUESTFINISHED)
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

            void UnsubscribeAndClear(string requestClass, string instrumentReferences)
            {
                string requestId = Guid.NewGuid().ToString();
                Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_UNSUBSCRIBE));
                if (!string.IsNullOrEmpty(requestClass))
                    Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTCLASS, requestClass));
                if (!string.IsNullOrEmpty(instrumentReferences))
                    Assert.IsTrue(message.AddList(Fields.MDF_F_INSREFLIST, instrumentReferences));
                Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTID, requestId));
                Assert.IsTrue(mdf.Send(message));
                message.Reset();
                Assert.IsTrue(Consume(mdf, new string[1] { requestId }));
                receivedMessageTypes.Clear();
                receivedInstrumentReferences.Clear();
                Assert.AreEqual(0, receivedMessageTypes.Count);
                Assert.AreEqual(0, receivedInstrumentReferences.Count);
            }
            UnsubscribeAndClear(RequestClass, instrumentReferences);

            //subscribe to all messages for a particular instrument
            requestId = "rid2";
            instrumentReferences = "772";
            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_REQUEST));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTCLASS, RequestClasses.All));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_IMAGE));
            Assert.IsTrue(message.AddList(Fields.MDF_F_INSREFLIST, instrumentReferences));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTID, requestId));
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
            using MarketDataFeed mdf = new();
            using Message message = new();
            //connect
            Assert.IsTrue(mdf.Connect(GetTestRunParameter("host")));
            //log on
            Assert.IsTrue(LogOn(mdf, message));
            //send request
            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_REQUEST));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTCLASS, RequestClasses.MDF_RC_INSREF));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_STREAM));
            Assert.IsTrue(message.AddString(Fields.MDF_F_REQUESTID, "rid"));
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
                    while (mdf.GetNextMessage(out int mref, out _, out ulong instrumentReference))
                    {
                        if (mref == MessageReferences.MDF_M_INSREF)
                            succeeded = true;

                        while (mdf.GetNextField(out uint field, out ReadOnlySpan<byte> value))
                        {
                            //no permissions to create instruments
                            if (field == (uint)Fields.MDF_F_REQUESTSTATUS
                                && Utf8Parser.TryParse(value, out uint @uint, out int _)
                                && @uint == 101)
                                succeeded = true;
                        }
                    }
                }
            } while (!succeeded && DateTime.UtcNow.Subtract(time).TotalSeconds < TimeoutInSeconds);

            Assert.IsTrue(succeeded);
        }

        [TestMethod]
        public void ExtractAndInjectMessageTest()
        {
            using MarketDataFeed mdf = new();
            using Message message = new();
            //connect
            Assert.IsTrue(mdf.Connect(GetTestRunParameter("host")));
            //log on
            Assert.IsTrue(LogOn(mdf, message));
            //subscribe to quotes for instrument 772
            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_REQUEST));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTCLASS, RequestClasses.MDF_RC_QUOTE));
            Assert.IsTrue(message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_IMAGE));
            const ulong InsRef = 772;
            Assert.IsTrue(message.AddString(Fields.MDF_F_INSREFLIST, InsRef.ToString()));
            Assert.IsTrue(mdf.Send(message));
            //consume the request
            Assert.AreEqual(1, mdf.Consume(3000));
            //extract the message
            IntPtr ptr = mdf.Extract(out ushort mref, out ulong insref, out uint len);
            Assert.AreNotEqual(default, ptr);
            Assert.AreEqual(MessageReferences.MDF_M_QUOTE, mref);
            Assert.AreEqual(InsRef, insref);
            Assert.IsTrue(len > 0);
            //inject the message into the another handle using the pointer
            using MarketDataFeed targetMdf = new();
            Assert.AreEqual(1, targetMdf.Inject(ptr, len));
            Assert.IsTrue(targetMdf.GetNextMessage(out ushort injectedMref, out ulong injectedInsRef));
            Assert.AreEqual(MessageReferences.MDF_M_QUOTE, injectedMref);
            Assert.AreEqual(InsRef, injectedInsRef);
            //copy the data
            byte[] data = new byte[len];
            Marshal.Copy(ptr, data, 0, (int)len);
            //inject the message using the copied data
            using MarketDataFeed targetMdf2 = new();
            Assert.AreEqual(1, targetMdf2.Inject(data));
            Assert.IsTrue(targetMdf2.GetNextMessage(out injectedMref, out injectedInsRef));
            Assert.AreEqual(MessageReferences.MDF_M_QUOTE, injectedMref);
            Assert.AreEqual(InsRef, injectedInsRef);
            //inject the message into a handle that has a callback
            using MarketDataFeed targetMdf3 = new();
            targetMdf3.DataCallback = (data, handle) =>
            {
                while (handle.GetNextMessage(out injectedMref, out injectedInsRef))
                {
                    CallbackUserData callbackUserData = data as CallbackUserData;
                    Assert.IsNotNull(callbackUserData);
                    callbackUserData.Count++;
                    Assert.AreEqual(MessageReferences.MDF_M_QUOTE, injectedMref);
                    Assert.AreEqual(InsRef, injectedInsRef);
                }
            };
            CallbackUserData callbackUserData = new();
            targetMdf3.CallbackUserData = callbackUserData;
            Assert.AreEqual(0, targetMdf3.Inject(ptr, len));
            Assert.AreEqual(1, callbackUserData.Count);

            while (mdf.GetNextField(out uint _, out _)) ;
            Assert.IsFalse(mdf.GetNextMessage(out ushort _, out _));
            mdf.Disconnect();
        }

        private string GetTestRunParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException(nameof(parameterName));

            string parameterValue = TestContext.Properties[parameterName] as string;
            if (string.IsNullOrEmpty(parameterValue))
                Assert.Fail($"No {parameterName} was specified in the .runsettings file.");
            return parameterValue;
        }

        private bool LogOn(MarketDataFeed mdf, Message message)
        {
            Assert.IsTrue(message.Add(0, MessageReferences.MDF_M_LOGON));
            Assert.IsTrue(message.AddString(Fields.MDF_F_USERNAME, GetTestRunParameter("username")));
            Assert.IsTrue(message.AddString(Fields.MDF_F_PASSWORD, GetTestRunParameter("password")));
            Assert.IsTrue(mdf.Send(message));
            message.Reset();
            return Consume(mdf, MessageReferences.MDF_M_LOGONGREETING);
        }

        private static bool Consume(MarketDataFeed mdf, ushort messageReference)
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
                        while (mdf.GetNextMessage(out ushort mref, out ulong _))
                            if (mref == messageReference)
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
                        while (mdf.GetNextField(out uint field, out ReadOnlySpan<byte> value))
                            if (field == Fields.MDF_F_REQUESTID && requestIds.Contains(Encoding.UTF8.GetString(value.ToArray())))
                                numberOfFinishedRequests++;
            } while (numberOfFinishedRequests < requestIds.Count && DateTime.UtcNow.Subtract(time).TotalSeconds < TimeoutInSeconds);

            return numberOfFinishedRequests == requestIds.Count;
        }

        private static long GetTotalAllocatedBytes()
        {
#if NETFRAMEWORK
            //GC statistics are guaranteed to be accurate only after a full, blocking collection.
            GC.Collect();
            return AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize;
#else
            return GC.GetTotalAllocatedBytes(true);
#endif
        }

        private class CallbackUserData
        {
            public int Count { get; set; }
        }
    }
}