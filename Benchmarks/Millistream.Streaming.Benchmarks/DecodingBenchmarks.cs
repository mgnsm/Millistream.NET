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
        private static readonly mdf_data_callback s_dllImportDataCallback = DataCallbackUsingDllImport;
        private static readonly IntPtr s_ptrToDllImportDataCallback = Marshal.GetFunctionPointerForDelegate(s_dllImportDataCallback);

        private static readonly StatusCallback<object> s_statusCallback = StatusCallback;
        private static readonly mdf_status_callback s_dllImportStatusCallback = StatusCallbackUsingDllImport;
        private static readonly IntPtr s_ptrToDllImportStatusCallback = Marshal.GetFunctionPointerForDelegate(s_dllImportStatusCallback);

        private static volatile bool s_dataCallbackInvoked;
        private static int s_statusCallbacks;
        private MarketDataFeed _mdf;
        private IntPtr _mdfHandle;
        private Streaming.Message _message;
        private IntPtr _messageHandle;

        [IterationSetup]
        public void IterationSetup()
        {
            _mdf = new();
            _mdfHandle = DllImports.mdf_create();
            _message = new();
            _messageHandle = DllImports.mdf_message_create();
            s_dataCallbackInvoked = false;
            s_statusCallbacks = 0;
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _message.Dispose();
            DllImports.mdf_message_destroy(_messageHandle);
            _mdf.Dispose();
            DllImports.mdf_destroy(_mdfHandle);
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
        public void ConsumeUsingDllImport()
        {
            LogonUsingDllImport();

            RequestUsingDllImport();

            if (!ConsumeUsingDllImport(10, 90))
                throw new InvalidOperationException("Failed to consume.");

            DllImports.mdf_disconnect(_mdfHandle);
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
        public void ConsumeWithDataCallbackUsingDllImport()
        {
            LogonUsingDllImport();

            if (DllImports.mdf_set_property(_mdfHandle, MDF_OPTION.MDF_OPT_DATA_CALLBACK_FUNCTION,
                s_ptrToDllImportDataCallback) != 1)
                throw new InvalidOperationException("Could not set data callback.");

            RequestUsingDllImport();

            while (!s_dataCallbackInvoked)
                if (DllImports.mdf_consume(_mdfHandle, 1) == -1)
                    break;

            if (!s_dataCallbackInvoked)
                throw new InvalidOperationException("Data callback was not invoked.");

            DllImports.mdf_disconnect(_mdfHandle);
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
        public void StatusCallbackUsingDllImport()
        {
            if (DllImports.mdf_set_property(_mdfHandle, MDF_OPTION.MDF_OPT_STATUS_CALLBACK_FUNCTION,
                s_ptrToDllImportStatusCallback) != 1)
                throw new InvalidOperationException("Could not set status callback.");

            LogonUsingDllImport();

            if (s_statusCallbacks < 4)
                throw new InvalidOperationException("Status callback was not invoked as expected.");

            DllImports.mdf_disconnect(_mdfHandle);

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

            _ = _message.Add(0, (int)MessageReference.MDF_M_LOGON);
            _ = _message.AddString((int)Field.MDF_F_USERNAME, Username);
            _ = _message.AddString((int)Field.MDF_F_PASSWORD, Password);
            _ = _mdf.Send(_message);
            _message.Reset();
            if (!Consume(1, 10))
                throw new InvalidOperationException("Failed to logon.");
        }

        private void LogonUsingDllImport()
        {
            if (DllImports.mdf_connect(_mdfHandle, Server) != 1)
                throw new InvalidOperationException("Failed to connect.");

            _ = DllImports.mdf_message_add(_messageHandle, 0, (int)MessageReference.MDF_M_LOGON);
            _ = DllImports.mdf_message_add_string(_messageHandle, (int)Field.MDF_F_USERNAME, Username);
            _ = DllImports.mdf_message_add_string(_messageHandle, (int)Field.MDF_F_PASSWORD, Password);
            _ = DllImports.mdf_message_send(_mdfHandle, _messageHandle);
            DllImports.mdf_message_reset(_messageHandle);
            if (!ConsumeUsingDllImport(1, 10))
                throw new InvalidOperationException("Failed to logon.");
        }

        private void Request()
        {
            _ = _message.Add(0, (int)MessageReference.MDF_M_REQUEST);
            _ = _message.AddList((uint)Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_BASICDATA);
            _ = _message.AddNumeric((uint)Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE);
            _ = _message.AddList((uint)Field.MDF_F_INSREFLIST, "772");
            _ = _message.AddString((uint)Field.MDF_F_REQUESTID, "rid");
            _ = _mdf.Send(_message);
            _message.Reset();
        }

        private void RequestUsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, (int)MessageReference.MDF_M_REQUEST);
            _ = DllImports.mdf_message_add_list(_messageHandle, (uint)Field.MDF_F_REQUESTCLASS, StringConstants.RequestClasses.MDF_RC_BASICDATA);
            _ = DllImports.mdf_message_add_numeric(_messageHandle, (uint)Field.MDF_F_REQUESTTYPE, StringConstants.RequestTypes.MDF_RT_IMAGE);
            _ = DllImports.mdf_message_add_list(_messageHandle, (uint)Field.MDF_F_INSREFLIST, "772");
            _ = DllImports.mdf_message_add_string(_messageHandle, (uint)Field.MDF_F_REQUESTID, "rid");
            _ = DllImports.mdf_message_send(_mdfHandle, _messageHandle);
            DllImports.mdf_message_reset(_messageHandle);
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
                        if (mref == (int)MessageReference.MDF_M_LOGONGREETING)
                            return true;

                        while (_mdf.GetNextField(out uint field, out ReadOnlySpan<byte> value))
                            if (field == (uint)Field.MDF_F_REQUESTID && !value.IsEmpty)
                                return true;
                    }
                }
            }
            while (DateTime.UtcNow.Subtract(time).TotalSeconds < waitTimeout);
            return false;
        }

        private bool ConsumeUsingDllImport(int consumeTimeout, int waitTimeout)
        {
            DateTime time = DateTime.UtcNow;
            do
            {
                int ret = DllImports.mdf_consume(_mdfHandle, consumeTimeout);
                if (ret == -1)
                    break;

                if (ret == 1)
                {
                    int mref = default;
                    int mclass = default;
                    uint insref = default;
                    while (DllImports.mdf_get_next_message(_mdfHandle, ref mref, ref mclass, ref insref) == 1)
                    {
                        if (mref == (int)MessageReference.MDF_M_LOGONGREETING)
                            return true;

                        uint tag = default;
                        IntPtr value = default;
                        while (DllImports.mdf_get_next_field(_mdfHandle, ref tag, ref value) == 1)
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

                            if (tag == (uint)Field.MDF_F_REQUESTID && !span.IsEmpty)
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
                    if (tag == (uint)Field.MDF_F_REQUESTID && !value.IsEmpty)
                        s_dataCallbackInvoked = true;
        }

        private static void DataCallbackUsingDllImport(IntPtr userdata, IntPtr handle)
        {
            int mref = default;
            int mclass = default;
            uint insref = default;
            while (DllImports.mdf_get_next_message(handle, ref mref, ref mclass, ref insref) == 1)
            {
                uint tag = default;
                IntPtr pointer = default;
                while (DllImports.mdf_get_next_field(handle, ref tag, ref pointer) == 1)
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

                    if (tag == (uint)Field.MDF_F_REQUESTID && !value.IsEmpty)
                        s_dataCallbackInvoked = true;
                }
            }
        }

        private static void StatusCallback(object userData, ConnectionStatus connectionStatus, ReadOnlySpan<byte> host, ReadOnlySpan<byte> ip) =>
            s_statusCallbacks++;

        private static void StatusCallbackUsingDllImport(IntPtr userdata, ConnectionStatus status, IntPtr host, IntPtr ip) =>
            s_statusCallbacks++;
    }
}