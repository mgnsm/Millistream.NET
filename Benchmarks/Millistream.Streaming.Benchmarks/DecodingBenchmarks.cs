using BenchmarkDotNet.Attributes;
using System;
using MarketDataFeed = Millistream.Streaming.MarketDataFeed<object, object>;

namespace Millistream.Streaming.Benchmarks
{
    [MemoryDiagnoser]
    public class DecodingBenchmarks : IDisposable
    {
        private static readonly DataCallback<object, object> s_dataCallback = OnDataReceived;
        private static readonly StatusCallback<object> s_statusCallback = OnStatusChanged;
        private static volatile bool s_dataCallbackInvoked;
        private static int s_statusCallbacks;
        private MarketDataFeed _mdf;
        private Message _message;

        [IterationSetup]
        public void IterationSetup()
        {
            _mdf = new();
            _message = new();
            s_dataCallbackInvoked = false;
            s_statusCallbacks = 0;
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _message.Dispose();
            _mdf.Dispose();
        }

        [Benchmark]
        public void Consume()
        {
            Logon(_mdf, _message);

            Request(_mdf, _message);

            if (!Consume(_mdf, 10, 90))
                throw new InvalidOperationException("Failed to consume.");

            _mdf.Disconnect();
        }

        [Benchmark]
        public void ConsumeWithDataCallback()
        {
            Logon(_mdf, _message);

            _mdf.DataCallback = s_dataCallback;

            Request(_mdf, _message);

            while (!s_dataCallbackInvoked)
                if (_mdf.Consume(1) == -1)
                    break;

            if (!s_dataCallbackInvoked)
                throw new InvalidOperationException("Data callback was not invoked.");

            _mdf.Disconnect();
        }

        [Benchmark]
        public void StatusCallback()
        {
            _mdf.StatusCallback = s_statusCallback;

            Logon(_mdf, _message);

            if (s_statusCallbacks < 4)
                throw new InvalidOperationException("Status callback was not invoked as expected.");

            _mdf.Disconnect();

            if (s_statusCallbacks < 5)
                throw new InvalidOperationException("Status callback was not invoked as expected.");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                IterationCleanup();
        }

        private static void Logon(MarketDataFeed mdf, Message message)
        {
            if (!mdf.Connect("sandbox.millistream.com:9100"))
                throw new InvalidOperationException("Failed to connect.");

            message.Add(0, MessageReference.MDF_M_LOGON);
            message.AddString(Field.MDF_F_USERNAME, "sandbox");
            message.AddString(Field.MDF_F_PASSWORD, "sandbox");
            mdf.Send(message);
            message.Reset();
            if (!Consume(mdf, 1, 10))
                throw new InvalidOperationException("Failed to logon.");
        }

        private static void Request(MarketDataFeed mdf, Message message)
        {
            message.Add(0, MessageReference.MDF_M_REQUEST);
            message.AddList(Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_BASICDATA);
            message.AddNumeric(Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE);
            message.AddList(Field.MDF_F_INSREFLIST, "772");
            message.AddString(Field.MDF_F_REQUESTID, "rid");
            mdf.Send(message);
            message.Reset();
        }

        private static bool Consume(MarketDataFeed mdf, int consumeTimeout, int waitTimeout)
        {
            DateTime time = DateTime.UtcNow;
            do
            {
                int ret = mdf.Consume(consumeTimeout);
                if (ret == -1)
                    break;

                if (ret == 1)
                {
                    while (mdf.GetNextMessage(out MessageReference mref, out _))
                    {
                        if (mref == MessageReference.MDF_M_LOGONGREETING)
                            return true;

                        while (mdf.GetNextField(out Field field, out _))
                            if (field == Field.MDF_F_REQUESTID)
                                return true;
                    }
                }
            }
            while (DateTime.UtcNow.Subtract(time).TotalSeconds < waitTimeout);
            return false;
        }

        private static void OnDataReceived(object userData, MarketDataFeed<object, object> handle)
        {
            while (handle.GetNextMessage(out ushort _, out _))
                while (handle.GetNextField(out Field field, out _))
                    if (field == Field.MDF_F_REQUESTID)
                        s_dataCallbackInvoked = true;
        }

        private static void OnStatusChanged(object userData, ConnectionStatus connectionStatus, ReadOnlySpan<byte> host, ReadOnlySpan<byte> ip) =>
            s_statusCallbacks++;
    }
}