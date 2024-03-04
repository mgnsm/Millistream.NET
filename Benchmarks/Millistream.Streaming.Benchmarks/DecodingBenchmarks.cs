using BenchmarkDotNet.Attributes;
using System;
using System.Runtime.InteropServices;
using MarketDataFeed = Millistream.Streaming.MarketDataFeed<object, object>;

namespace Millistream.Streaming.Benchmarks
{
    [MemoryDiagnoser]
    public class DecodingBenchmarks : IDisposable
    {
        private const string Server = "sandbox.millistream.com:9100";
        private const string Username = "sandbox";
        private const string Password = "sandbox";
        
        private static readonly DataCallback<object, object> s_dataCallback = DataCallback;
        private static readonly mdf_data_callback s_functionPointerDataCallback = DataCallbackUsingFunctionPointer;
        private static readonly IntPtr s_ptrToFunctionPointerDataCallback = Marshal.GetFunctionPointerForDelegate(s_functionPointerDataCallback);

        private static readonly StatusCallback<object> s_statusCallback = StatusCallback;
        private static readonly mdf_status_callback s_functionPointerStatusCallback = StatusCallbackUsingFunctionPointer;
        private static readonly IntPtr s_ptrToFunctionPointerStatusCallback = Marshal.GetFunctionPointerForDelegate(s_functionPointerStatusCallback);

        private static volatile bool s_dataCallbackInvoked;
        private static int s_statusCallbacks;
        private MarketDataFeed _mdf;
        private IntPtr _mdfHandle;
        private Streaming.Message _message;
        private IntPtr _messageHandle;

        [IterationSetup]
        public unsafe void IterationSetup()
        {
            _mdf = new();
            _mdfHandle = FunctionPointers.mdf_create();
            _message = new();
            _messageHandle = FunctionPointers.mdf_message_create();
            s_dataCallbackInvoked = false;
            s_statusCallbacks = 0;
        }

        [IterationCleanup]
        public unsafe void IterationCleanup()
        {
            _message.Dispose();
            FunctionPointers.mdf_message_destroy(_messageHandle);
            _mdf.Dispose();
            FunctionPointers.mdf_destroy(_mdfHandle);
        }

        [Benchmark]
        public void Consume()
        {
            Logon();

            Request();

            if (!Consume(10, 90))
                throw new InvalidOperationException("Failed to consume.");

            _mdf.Disconnect();
        }

        [Benchmark]
        public unsafe void ConsumeUsingFunctionPointer()
        {
            LogonUsingFunctionPointer();

            RequestUsingFunctionPointer();

            if (!ConsumeUsingFunctionPointer(10, 90))
                throw new InvalidOperationException("Failed to consume.");

            FunctionPointers.mdf_disconnect(_mdfHandle);
        }

        [Benchmark]
        public void ConsumeWithDataCallback()
        {
            Logon();

            _mdf.DataCallback = s_dataCallback;

            Request();

            while (!s_dataCallbackInvoked)
                if (_mdf.Consume(1) == -1)
                    break;

            if (!s_dataCallbackInvoked)
                throw new InvalidOperationException("Data callback was not invoked.");

            _mdf.Disconnect();
        }

        [Benchmark]
        public unsafe void ConsumeWithDataCallbackUsingFunctionPointer()
        {
            LogonUsingFunctionPointer();

            if (FunctionPointers.mdf_set_property(_mdfHandle, MDF_OPTION.MDF_OPT_DATA_CALLBACK_FUNCTION,
                s_ptrToFunctionPointerDataCallback) != 1)
                throw new InvalidOperationException("Could not set data callback.");

            RequestUsingFunctionPointer();

            while (!s_dataCallbackInvoked)
                if (FunctionPointers.mdf_consume(_mdfHandle, 1) == -1)
                    break;

            if (!s_dataCallbackInvoked)
                throw new InvalidOperationException("Data callback was not invoked.");

            FunctionPointers.mdf_disconnect(_mdfHandle);
        }

        [Benchmark]
        public void StatusCallback()
        {
            _mdf.StatusCallback = s_statusCallback;

            Logon();

            if (s_statusCallbacks < 4)
                throw new InvalidOperationException("Status callback was not invoked as expected.");

            _mdf.Disconnect();

            if (s_statusCallbacks < 5)
                throw new InvalidOperationException("Status callback was not invoked as expected.");
        }

        [Benchmark]
        public unsafe void StatusCallbackUsingFunctionPointer()
        {
            if (FunctionPointers.mdf_set_property(_mdfHandle, MDF_OPTION.MDF_OPT_STATUS_CALLBACK_FUNCTION,
                s_ptrToFunctionPointerStatusCallback) != 1)
                throw new InvalidOperationException("Could not set status callback.");

            LogonUsingFunctionPointer();

            if (s_statusCallbacks < 4)
                throw new InvalidOperationException("Status callback was not invoked as expected.");

            FunctionPointers.mdf_disconnect(_mdfHandle);

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

        private void Logon()
        {
            if (!_mdf.Connect(Server))
                throw new InvalidOperationException("Failed to connect.");

            _ = _message.Add(0, MessageReferences.MDF_M_LOGON);
            _ = _message.AddString(Fields.MDF_F_USERNAME, Username);
            _ = _message.AddString(Fields.MDF_F_PASSWORD, Password);
            _ = _mdf.Send(_message);
            _message.Reset();
            if (!Consume(1, 10))
                throw new InvalidOperationException("Failed to logon.");
        }

        private unsafe void LogonUsingFunctionPointer()
        {
            if (FunctionPointers.mdf_connect(_mdfHandle, Server) != 1)
                throw new InvalidOperationException("Failed to connect.");

            _ = FunctionPointers.mdf_message_add(_messageHandle, 0, MessageReferences.MDF_M_LOGON);
            _ = FunctionPointers.mdf_message_add_string_str(_messageHandle, Fields.MDF_F_USERNAME, Username);
            _ = FunctionPointers.mdf_message_add_string_str(_messageHandle, Fields.MDF_F_PASSWORD, Password);
            _ = FunctionPointers.mdf_message_send(_mdfHandle, _messageHandle);
            FunctionPointers.mdf_message_reset(_messageHandle);
            if (!ConsumeUsingFunctionPointer(1, 10))
                throw new InvalidOperationException("Failed to logon.");
        }

        private void Request()
        {
            _ = _message.Add(0, MessageReferences.MDF_M_REQUEST);
            _ = _message.AddList(Fields.MDF_F_REQUESTCLASS, RequestClasses.MDF_RC_BASICDATA);
            _ = _message.AddNumeric(Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_IMAGE);
            _ = _message.AddList(Fields.MDF_F_INSREFLIST, "772");
            _ = _message.AddString(Fields.MDF_F_REQUESTID, "rid");
            _ = _mdf.Send(_message);
            _message.Reset();
        }

        private unsafe void RequestUsingFunctionPointer()
        {
            _ = FunctionPointers.mdf_message_add(_messageHandle, 0, MessageReferences.MDF_M_REQUEST);
            _ = FunctionPointers.mdf_message_add_list_str(_messageHandle, Fields.MDF_F_REQUESTCLASS, RequestClasses.MDF_RC_BASICDATA);
            _ = FunctionPointers.mdf_message_add_numeric_str(_messageHandle, Fields.MDF_F_REQUESTTYPE, RequestTypes.MDF_RT_IMAGE);
            _ = FunctionPointers.mdf_message_add_list_str(_messageHandle, Fields.MDF_F_INSREFLIST, "772");
            _ = FunctionPointers.mdf_message_add_string_str(_messageHandle, Fields.MDF_F_REQUESTID, "rid");
            _ = FunctionPointers.mdf_message_send(_mdfHandle, _messageHandle);
            FunctionPointers.mdf_message_reset(_messageHandle);
        }

        private bool Consume(int consumeTimeout, int waitTimeout)
        {
            DateTime time = DateTime.UtcNow;
            do
            {
                int ret = _mdf.Consume(consumeTimeout);
                if (ret == -1)
                    break;

                if (ret == 1)
                {
                    while (_mdf.GetNextMessage(out int mref, out int _, out _))
                    {
                        if (mref == MessageReferences.MDF_M_LOGONGREETING)
                            return true;

                        while (_mdf.GetNextField(out uint field, out ReadOnlySpan<byte> value))
                            if (field == Fields.MDF_F_REQUESTID && !value.IsEmpty)
                                return true;
                    }
                }
            }
            while (DateTime.UtcNow.Subtract(time).TotalSeconds < waitTimeout);
            return false;
        }

        private unsafe bool ConsumeUsingFunctionPointer(int consumeTimeout, int waitTimeout)
        {
            DateTime time = DateTime.UtcNow;
            do
            {
                int ret = FunctionPointers.mdf_consume(_mdfHandle, consumeTimeout);
                if (ret == -1)
                    break;

                if (ret == 1)
                {
                    int mref = default;
                    int mclass = default;
                    ulong insref = default;
                    while (FunctionPointers.mdf_get_next_message(_mdfHandle, ref mref, ref mclass, ref insref) == 1)
                    {
                        if (mref == MessageReferences.MDF_M_LOGONGREETING)
                            return true;

                        uint tag = default;
                        IntPtr value = default;
                        while (FunctionPointers.mdf_get_next_field(_mdfHandle, ref tag, ref value) == 1)
                        {
                            ReadOnlySpan<byte> span;
                            if (value != IntPtr.Zero)
                            {
                                unsafe
                                {
                                    byte* p = (byte*)value;
                                    int fieldOffset = 0;
                                    while (*(p + fieldOffset++) != 0) ;
                                    span = new ReadOnlySpan<byte>(p, fieldOffset - 1);
                                }
                            }
                            else
                            {
                                span = default;
                            }

                            if (tag == Fields.MDF_F_REQUESTID && !span.IsEmpty)
                                return true;
                        }
                    }
                }
            }
            while (DateTime.UtcNow.Subtract(time).TotalSeconds < waitTimeout);
            return false;
        }

        private static void DataCallback(object userData, MarketDataFeed<object, object> handle)
        {
            while (handle.GetNextMessage(out int _, out _, out _))
                while (handle.GetNextField(out uint tag, out ReadOnlySpan<byte> value))
                    if (tag == Fields.MDF_F_REQUESTID && !value.IsEmpty)
                        s_dataCallbackInvoked = true;
        }

        private static unsafe void DataCallbackUsingFunctionPointer(IntPtr userdata, IntPtr handle)
        {
            int mref = default;
            int mclass = default;
            ulong insref = default;
            while (FunctionPointers.mdf_get_next_message(handle, ref mref, ref mclass, ref insref) == 1)
            {
                uint tag = default;
                IntPtr pointer = default;
                while (FunctionPointers.mdf_get_next_field(handle, ref tag, ref pointer) == 1)
                {
                    ReadOnlySpan<byte> value;
                    if (pointer != IntPtr.Zero)
                    {
                        unsafe
                        {
                            byte* p = (byte*)pointer;
                            int fieldOffset = 0;
                            while (*(p + fieldOffset++) != 0) ;
                            value = new ReadOnlySpan<byte>(p, fieldOffset - 1);
                        }
                    }
                    else
                    {
                        value = default;
                    }

                    if (tag == Fields.MDF_F_REQUESTID && !value.IsEmpty)
                        s_dataCallbackInvoked = true;
                }
            }
        }

        private static void StatusCallback(object userData, ConnectionStatus connectionStatus, ReadOnlySpan<byte> host, ReadOnlySpan<byte> ip) =>
            s_statusCallbacks++;

        private static void StatusCallbackUsingFunctionPointer(IntPtr userdata, ConnectionStatus status, IntPtr host, IntPtr ip) =>
            s_statusCallbacks++;
    }
}