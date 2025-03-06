using Millistream.Streaming.Interop;
using System;
using System.Buffers;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;

namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a managed API handle (mdf_t) that can be connected to the system.
    /// </summary>
    /// <typeparam name="TCallbackUserData">The type of the custom user data that will be available to the data callback function.</typeparam>
    /// <typeparam name="TStatusCallbackUserData">The type of the custom user data that will be available to the status callback function.</typeparam>
    /// <remarks>Handles are not thread-safe. If multiple threads will share access to a single handle, the accesses has to be serialized using a mutex or other forms of locking mechanisms. The API as such is thread-safe so multiple threads can have local handles without the need for locks.</remarks>
    public sealed unsafe partial class MarketDataFeed<TCallbackUserData, TStatusCallbackUserData> : IMarketDataFeed<TCallbackUserData, TStatusCallbackUserData>, IDisposable
    {
        #region Constants
        internal const int MinConnectionTimeout = 1;
        internal const int MaxConnectionTimeout = 60;
        internal const int MinHeartbeatInterval = 1;
        internal const int MaxHeartbeatInterval = 86400;
        internal const int MinMissedHeartbeats = 1;
        internal const int MaxMissedHeartbeats = 100;
        private const string UnknownOptionMessage = "The native value of the property cannot be fetched. " +
            "Please make sure that you have installed the latest version of the native dependency.";
        #endregion

        #region Fields
#pragma warning disable CS0618
        private static readonly ImmutableHashSet<int> s_messageReferences = ImmutableHashSet.Create((int[])Enum.GetValues(typeof(MessageReference)));
        private static readonly ImmutableHashSet<uint> s_fields = ImmutableHashSet.Create((uint[])Enum.GetValues(typeof(Field)));
#pragma warning restore CS0618
        private readonly NativeImplementation _nativeImplementation;
        private readonly mdf_status_callback _nativeStatusCallback;
        private readonly mdf_data_callback _nativeDataCallback;
        private readonly IntPtr _nativeDataCallbackPointer;
        private readonly IntPtr _nativeStatusCallbackPointer;
        private IntPtr _feedHandle;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of the <see cref="MarketDataFeed{TCallbackData,TStatusCallbackData}"/> class.
        /// </summary>
        /// <exception cref="DllNotFoundException">The native dependency is missing.</exception>
        /// <remarks>The corresponding native function is mdf_create.</remarks>
        public MarketDataFeed()
            : this(default, false) { }

        /// <summary>
        /// Creates an instance of the <see cref="MarketDataFeed{TCallbackData,TStatusCallbackData}"/> class.
        /// </summary>
        /// <param name="nativeLibraryPath"></param>
        /// <exception cref="ArgumentNullException"><paramref name="nativeLibraryPath"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="DllNotFoundException">The native dependency can't be found.</exception>
        /// <remarks>The corresponding native function is mdf_create.</remarks>
        public MarketDataFeed(string nativeLibraryPath)
            : this(nativeLibraryPath, true) { }

        internal MarketDataFeed(NativeImplementation nativeImplementation)
        {
            _nativeImplementation = nativeImplementation ?? throw new ArgumentNullException(nameof(nativeImplementation));
            _feedHandle = _nativeImplementation.mdf_create();
            _nativeStatusCallback = OnStatusChanged;
            _nativeDataCallback = OnDataReceived;
            _nativeStatusCallbackPointer = Marshal.GetFunctionPointerForDelegate(_nativeStatusCallback);
            _nativeDataCallbackPointer = Marshal.GetFunctionPointerForDelegate(_nativeDataCallback);
        }

        private MarketDataFeed(string nativeLibraryPath, bool validateArgument)
            : this(string.IsNullOrEmpty(nativeLibraryPath) ?
                (validateArgument ? throw new ArgumentNullException(nameof(nativeLibraryPath)) : NativeImplementation.Default)
                : new NativeImplementation(nativeLibraryPath))
        { }

        ~MarketDataFeed() => _nativeImplementation?.mdf_destroy(_feedHandle);
        #endregion

        #region Properties
        /// <summary>
        /// Gets the file descriptor used by the connection. Will be -1 (or INVALID_SOCKET on Windows) if there is no connection.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_FD"/> option cannot be fetched.</exception>
        public int FileDescriptor => GetInt32Property(MDF_OPTION.MDF_OPT_FD);

        /// <summary>
        /// Gets or sets the current API error code.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_ERROR"/> option cannot be fetched or modified.</exception>
        public Error ErrorCode
        {
            get => (Error)GetInt32Property(MDF_OPTION.MDF_OPT_ERROR);
            set => SetProperty(MDF_OPTION.MDF_OPT_ERROR, (int)value);
        }

        /// <summary>
        /// Gets or sets the number of bytes received by the server since the handle was created.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_RCV_BYTES"/> option cannot be fetched or modified.</exception>
        public ulong ReceivedBytes
        {
            get => GetUInt64Property(MDF_OPTION.MDF_OPT_RCV_BYTES);
            set => SetProperty(MDF_OPTION.MDF_OPT_RCV_BYTES, value);
        }

        /// <summary>
        /// Gets or sets the number of bytes sent by the client since the handle was created.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_SENT_BYTES"/> option cannot be fetched or modified.</exception>
        public ulong SentBytes
        {
            get => GetUInt64Property(MDF_OPTION.MDF_OPT_SENT_BYTES);
            set => SetProperty(MDF_OPTION.MDF_OPT_SENT_BYTES, value);
        }

        private DataCallback<TCallbackUserData, TStatusCallbackUserData> _dataCallback;
        /// <summary>
        /// Gets or sets a callback function that will be called by the consume function if there are any messages to decode.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_DATA_CALLBACK_FUNCTION"/> option cannot be fetched or modified.</exception>
        public DataCallback<TCallbackUserData, TStatusCallbackUserData> DataCallback
        {
            get => _dataCallback;
            set
            {
                _dataCallback = value;
                if (_nativeImplementation.mdf_set_property(_feedHandle, MDF_OPTION.MDF_OPT_DATA_CALLBACK_FUNCTION,
                    value != null ? _nativeDataCallbackPointer : IntPtr.Zero) != 1)
                    throw new InvalidOperationException();
            }
        }

        private TCallbackUserData _callbackUserData;
        /// <summary>
        /// Gets or sets custom userdata that will be available to the data callback function.
        /// </summary>
        public TCallbackUserData CallbackUserData
        {
            get => _callbackUserData;
            set => _callbackUserData = value;
        }

        private StatusCallback<TStatusCallbackUserData> _statusCallback;
        /// <summary>
        /// Gets or sets a callback function that will be called whenever there is a change of the status of the connection.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_STATUS_CALLBACK_FUNCTION"/> option cannot be fetched or modified.</exception>
        public StatusCallback<TStatusCallbackUserData> StatusCallback
        {
            get => _statusCallback;
            set
            {
                _statusCallback = value;
                if (_nativeImplementation.mdf_set_property(_feedHandle, MDF_OPTION.MDF_OPT_STATUS_CALLBACK_FUNCTION,
                    value != null ? _nativeStatusCallbackPointer : IntPtr.Zero) != 1)
                    throw new InvalidOperationException();
            }
        }

        private TStatusCallbackUserData _statusCallbackUserData;
        /// <summary>
        /// Gets or sets custom userdata that will be available to the status callback function.
        /// </summary>
        public TStatusCallbackUserData StatusCallbackUserData
        {
            get => _statusCallbackUserData;
            set => _statusCallbackUserData = value;
        }

        /// <summary>
        /// Gets or sets the number of seconds before determining that a connect attempt has timed out. Valid values are 1 to 60. The default value is 5.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value is less than 1 or greater than 60.</exception>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_CONNECT_TIMEOUT"/> option cannot be fetched or modified.</exception>
        public int ConnectionTimeout
        {
            get => GetInt32Property(MDF_OPTION.MDF_OPT_CONNECT_TIMEOUT);
            set => SetProperty(MDF_OPTION.MDF_OPT_CONNECT_TIMEOUT, value, MinConnectionTimeout, MaxConnectionTimeout);
        }

        /// <summary>
        /// Gets or sets the number of seconds the connection must be idle before the API sends a heartbeat request to the server. Valid values are 1 to 86400. The default is 30.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value is less than 1 or greater than 86400.</exception>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_HEARTBEAT_INTERVAL"/> option cannot be fetched or modified.</exception>
        public int HeartbeatInterval
        {
            get => GetInt32Property(MDF_OPTION.MDF_OPT_HEARTBEAT_INTERVAL);
            set => SetProperty(MDF_OPTION.MDF_OPT_HEARTBEAT_INTERVAL, value, MinHeartbeatInterval, MaxHeartbeatInterval);
        }

        /// <summary>
        /// Gets or sets how many outstanding hearbeat requests to allow before the connection is determined to be disconnected. Valid values are 1 to 100. The default is 2.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value is less than 1 or greater than 100.</exception>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_HEARTBEAT_MAX_MISSED"/> option cannot be fetched or modified.</exception>
        public int MaximumMissedHeartbeats
        {
            get => GetInt32Property(MDF_OPTION.MDF_OPT_HEARTBEAT_MAX_MISSED);
            set => SetProperty(MDF_OPTION.MDF_OPT_HEARTBEAT_MAX_MISSED, value, MinMissedHeartbeats, MaxMissedHeartbeats);
        }

        /// <summary>
        /// Gets or sets a value indicating whether Nagle's algorithm is used on the TCP connection. It's enabled by default.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_TCP_NODELAY"/> option cannot be fetched or modified.</exception>
        public bool NoDelay
        {
            get => Convert.ToBoolean(GetInt32Property(MDF_OPTION.MDF_OPT_TCP_NODELAY));
            set => SetProperty(MDF_OPTION.MDF_OPT_TCP_NODELAY, value ? 1 : 0);
        }

        /// <summary>
        /// Used internally to enable or disable encryption.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_NO_ENCRYPTION"/> option cannot be fetched or modified.</exception>
        public bool NoEncryption
        {
            get => Convert.ToBoolean(GetInt32Property(MDF_OPTION.MDF_OPT_NO_ENCRYPTION));
            set => SetProperty(MDF_OPTION.MDF_OPT_NO_ENCRYPTION, value ? 1 : 0);
        }

        /// <summary>
        /// Gets the time difference in number of seconds between the client and the server. The value should be added to the current time on the client in order to get the server time. Please not that this value can be negative if the client clock is ahead of the server clock.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_TIME_DIFFERENCE"/> option cannot be fetched.</exception>
        public int TimeDifference => GetInt32Property(MDF_OPTION.MDF_OPT_TIME_DIFFERENCE);

        /// <summary>
        /// Gets or sets a numerical address to which the API will bind before attempting to connect to a server in <see cref="Connect"/>. If the bind fails then <see cref="Connect"/> also fails. The string is copied by the API and a <see langword="NULL" /> value can be used in order to "unset" the bind address.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_BIND_ADDRESS"/> option cannot be modified.</exception>
        public string BindAddress
        {
            get => GetStringProperty(MDF_OPTION.MDF_OPT_BIND_ADDRESS);
            set => SetProperty(MDF_OPTION.MDF_OPT_BIND_ADDRESS, value);
        }

        /// <summary>
        /// Gets the time difference in number of nanoseconds between the client and the server. The value should be added to the current time on the client in order to get the server time. Please not that this value can be negative if the client clock is ahead of the server clock.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_TIME_DIFFERENCE_NS"/> option cannot be fetched.</exception>
        public long TimeDifferenceNs
        {
            get
            {
                long value = default;
#if ARM64
                if (_nativeImplementation.mdf_get_long_property(_feedHandle, MDF_OPTION.MDF_OPT_TIME_DIFFERENCE_NS, 0, 0, 0, 0, 0, 0, ref value) != 1)
#else
                if (_nativeImplementation.mdf_get_long_property(_feedHandle, MDF_OPTION.MDF_OPT_TIME_DIFFERENCE_NS, ref value) != 1)
#endif
                    throw new InvalidOperationException();
                return value;
            }
        }

        /// <summary>
        /// Gets or sets a comma separated list of the message digests that the client will offer to the server upon connect.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_CRYPT_DIGESTS"/> option cannot be modified.</exception>
        public string MessageDigests
        {
            get => GetStringProperty(MDF_OPTION.MDF_OPT_CRYPT_DIGESTS);
            set => SetProperty(MDF_OPTION.MDF_OPT_CRYPT_DIGESTS, value);
        }

        /// <summary>
        /// Gets or sets a comma separated list of the encryption ciphers that the client will offer to the server upon connect.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_CRYPT_CIPHERS"/> option cannot be modified.</exception>
        public string Ciphers
        {
            get => GetStringProperty(MDF_OPTION.MDF_OPT_CRYPT_CIPHERS);
            set => SetProperty(MDF_OPTION.MDF_OPT_CRYPT_CIPHERS, value);
        }

        /// <summary>
        /// Gets the digest chosen by the server. Only available after <see cref="Connect"/> returns.
        /// </summary>
        public string MessageDigest => GetStringProperty(MDF_OPTION.MDF_OPT_CRYPT_DIGEST);

        /// <summary>
        /// Gets the cipher chosen by the server. Only available after <see cref="Connect"/> returns.
        /// </summary>
        public string Cipher => GetStringProperty(MDF_OPTION.MDF_OPT_CRYPT_CIPHER);

        /// <summary>
        /// Gets the number of seconds to wait before having to call <see cref="Consume(int)"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_TIMEOUT"/> option cannot be fetched.</exception>
        public int Timeout => GetInt32Property(MDF_OPTION.MDF_OPT_TIMEOUT);

        /// <summary>
        /// Gets or sets a value indicating whether delay-mode is enabled on the connection. It's disabled by default.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_HANDLE_DELAY"/> option cannot be fetched or modified.</exception>
        /// <remarks>Must be set prior to calling <see cref="Connect(string)"/>.</remarks>
        public bool HandleDelay
        {
            get => Convert.ToBoolean(GetInt32Property(MDF_OPTION.MDF_OPT_HANDLE_DELAY));
            set => SetProperty(MDF_OPTION.MDF_OPT_HANDLE_DELAY, value ? 1 : 0);
        }

        /// <summary>
        /// Gets the intended delay of the current message if delay-mode have been activated by setting the <see cref="HandleDelay"/> property. Note that this is the intended delay of the message and not necessarily the real delay, network latency, server latency and so on are not included.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_get_delay.</remarks>
        public byte Delay => _nativeImplementation.mdf_get_delay != default ? _nativeImplementation.mdf_get_delay(_feedHandle) : default;

        /// <summary>
        /// Gets the message class of the current received message.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_get_mclass.</remarks>
        public ulong MessageClass => _nativeImplementation.mdf_get_mclass != default ? _nativeImplementation.mdf_get_mclass(_feedHandle) : default;

        /// <summary>
        /// Gets the number of bytes waiting to be processed in the internal read buffer after a call to <see cref="Consume(int)"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_RBUF_SIZE"/> option cannot be fetched or modified.</exception>
        public uint ReadBufferSize => (uint)GetUInt64Property(MDF_OPTION.MDF_OPT_RBUF_SIZE);

        /// <summary>
        /// Gets or sets the current size of the internal read buffer.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value is less than <see cref="ReadBufferSize"/>.</exception>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_RBUF_MAXSIZE"/> option cannot be fetched or modified.</exception>
        public uint ReadBufferMaxSize
        {
            get => (uint)GetUInt64Property(MDF_OPTION.MDF_OPT_RBUF_MAXSIZE);
            set
            {
                if (value < ReadBufferSize)
                    throw new ArgumentOutOfRangeException(nameof(value));
                SetProperty(MDF_OPTION.MDF_OPT_RBUF_MAXSIZE, value);
            }
        }

        /// <summary>
        /// Gets the hostname of the currently connected server.
        /// </summary>
        public string ConnectedHost => GetStringProperty(MDF_OPTION.MDF_OPT_CONNECTED_HOST);

        /// <summary>
        /// Gets the IP address of the currently connected server.
        /// </summary>
        public string ConnectedIPAddress => GetStringProperty(MDF_OPTION.MDF_OPT_CONNECTED_IP);
        #endregion

        #region Methods
        /// <summary>
        /// Releases any resources used by the <see cref="MarketDataFeed{TCallbackData,TStatusCallbackData}"/> instance.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_destroy.</remarks>
        public void Dispose()
        {
            _nativeImplementation.mdf_destroy(_feedHandle);
            _feedHandle = default;
            GC.SuppressFinalize(this);
        }

        private void OnStatusChanged(IntPtr data, ConnectionStatus connectionStatus, IntPtr host, IntPtr ip)
        {
            static ReadOnlySpan<byte> GetSpan(IntPtr handle)
            {
                if (handle == IntPtr.Zero)
                    return default;

                byte* p = (byte*)handle;
                int byteCount = 0;
                while (*(p + byteCount) != 0)
                    byteCount++;
                return new Span<byte>(p, byteCount);
            }

            _statusCallback?.Invoke(StatusCallbackUserData, connectionStatus, GetSpan(host), GetSpan(ip));
        }

        private void OnDataReceived(IntPtr userData, IntPtr handle) => _dataCallback?.Invoke(CallbackUserData, this);

        private int GetInt32Property(MDF_OPTION option)
        {
            int value = default;
#if ARM64
            if (_nativeImplementation.mdf_get_int_property(_feedHandle, option, 0, 0, 0, 0, 0, 0, ref value) != 1)
#else
            if (_nativeImplementation.mdf_get_int_property(_feedHandle, option, ref value) != 1)
#endif
                throw new InvalidOperationException(UnknownOptionMessage);
            return value;
        }

        private ulong GetUInt64Property(MDF_OPTION option)
        {
            ulong value = default;

#if ARM64
            _nativeImplementation.mdf_get_ulong_property(_feedHandle, option, 0, 0, 0, 0, 0, 0, ref value);
#else
            _nativeImplementation.mdf_get_ulong_property(_feedHandle, option, ref value);
#endif
            return value;
        }

        private string GetStringProperty(MDF_OPTION option)
        {
            IntPtr value = default;
#if ARM64
            if (_nativeImplementation.mdf_get_property(_feedHandle, option, 0, 0, 0, 0, 0, 0, ref value) != 1 || value == IntPtr.Zero)
#else
            if (_nativeImplementation.mdf_get_property(_feedHandle, option, ref value) != 1 || value == IntPtr.Zero)
#endif
                return null;

            unsafe
            {
                byte* p = (byte*)value;
                int byteCount = 0;
                while (*(p + byteCount++) != 0) ;
                int charCount = Encoding.UTF8.GetCharCount(p, byteCount);
                char[] chars = ArrayPool<char>.Shared.Rent(charCount);
                try
                {
                    fixed (char* c = chars)
                    {
                        _ = Encoding.UTF8.GetChars(p, byteCount, c, charCount);
                        return new string(c);
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(chars);
                }
            }
        }

        private void SetProperty(MDF_OPTION option, int value)
        {
            unsafe
            {
                int* p = &value;
                if (_nativeImplementation.mdf_set_property(_feedHandle, option, (IntPtr)p) != 1)
                    throw new InvalidOperationException();
            }
        }

        private void SetProperty(MDF_OPTION option, ulong value)
        {
            unsafe
            {
                ulong* p = &value;
                if (_nativeImplementation.mdf_set_property(_feedHandle, option, (IntPtr)p) != 1)
                    throw new InvalidOperationException();
            }
        }

        private void SetProperty(MDF_OPTION option, int value, int minValue, int maxValue)
        {
            if (value < minValue || value > maxValue)
                throw new ArgumentOutOfRangeException(nameof(value));

            SetProperty(option, value);
        }

        private void SetProperty(MDF_OPTION option, string value)
        {
            int ret = 0;
            if (value != null)
            {
                int length = Encoding.UTF8.GetMaxByteCount(value.Length);
                byte[] bytes = ArrayPool<byte>.Shared.Rent(length + 1);
                try
                {
                    unsafe
                    {
                        fixed (char* c = value)
                        fixed (byte* b = bytes)
                        {
                            int bytesWritten = Encoding.UTF8.GetBytes(c, value.Length, b, length);
                            b[bytesWritten] = 0;
                            ret = _nativeImplementation.mdf_set_property(_feedHandle, option, (IntPtr)b);
                        }
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(bytes);
                }
            }
            else
            {
                ret = _nativeImplementation.mdf_set_property(_feedHandle, option, IntPtr.Zero);
            }

            if (ret != 1)
                throw new InvalidOperationException();
        }
        #endregion
    }
}