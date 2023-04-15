using Millistream.Streaming.Interop;
using System;
using System.Buffers;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
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
    public unsafe sealed class MarketDataFeed<TCallbackUserData, TStatusCallbackUserData> : IMarketDataFeed<TCallbackUserData, TStatusCallbackUserData>, IDisposable
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
        private static readonly ImmutableHashSet<int> s_messageReferences = ImmutableHashSet.Create((int[])Enum.GetValues(typeof(MessageReference)));
        private static readonly ImmutableHashSet<uint> s_fields = ImmutableHashSet.Create((uint[])Enum.GetValues(typeof(Field)));
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

        ~MarketDataFeed() => Dispose();
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
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_BIND_ADDRESS"/> option cannot be fetched or modified.</exception>
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
                if (_nativeImplementation.mdf_get_long_property(_feedHandle, MDF_OPTION.MDF_OPT_TIME_DIFFERENCE_NS, ref value) != 1)
                    throw new InvalidOperationException();
                return value;
            }
        }

        /// <summary>
        /// Gets or sets a comma separated list of the message digests that the client will offer to the server upon connect.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_CRYPT_DIGESTS"/> option cannot be fetched or modified.</exception>
        public string MessageDigests
        {
            get => GetStringProperty(MDF_OPTION.MDF_OPT_CRYPT_DIGESTS);
            set => SetProperty(MDF_OPTION.MDF_OPT_CRYPT_DIGESTS, value);
        }

        /// <summary>
        /// Gets or sets a comma separated list of the encryption ciphers that the client will offer to the server upon connect.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_CRYPT_CIPHERS"/> option cannot be fetched or modified.</exception>
        public string Ciphers
        {
            get => GetStringProperty(MDF_OPTION.MDF_OPT_CRYPT_CIPHERS);
            set => SetProperty(MDF_OPTION.MDF_OPT_CRYPT_CIPHERS, value);
        }

        /// <summary>
        /// Gets the digest chosen by the server. Only available after <see cref="Connect"/> returns.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_CRYPT_DIGEST"/> option cannot be fetched.</exception>
        public string MessageDigest => GetStringProperty(MDF_OPTION.MDF_OPT_CRYPT_DIGEST);

        /// <summary>
        /// Gets the cipher chosen by the server. Only available after <see cref="Connect"/> returns.
        /// </summary>
        /// <exception cref="InvalidOperationException">The native value of the <see cref="MDF_OPTION.MDF_OPT_CRYPT_CIPHER"/> option cannot be fetched.</exception>
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
        #endregion

        #region Methods
        /// <summary>
        /// Consumes data sent from the server. If there currently is no data the function waits for <paramref name="timeout"/> number of seconds, if <paramref name="timeout"/> is zero (0) the function will return immediately. If <paramref name="timeout"/> is negative then the wait period is treated as number of microseconds instead of number of seconds (i.e. -1000 will wait a maximum of 1000µs).
        /// </summary>
        /// <param name="timeout">The wait period in seconds if positive. If negative, the value is treated as the number of microseconds to wait instead of the number of seconds.</param>
        /// <returns>1 if data has been consumed that needs to be handled by <see cref="GetNextMessage(out ushort, out ulong)" /> and no callback function has been registered. The function returns 0 on timeout or if a callback function is registered and there was data. On errors, -1 will be returned (and the connection will be dropped).</returns>
        /// <remarks>The corresponding native function is mdf_consume.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Consume(int timeout) => _nativeImplementation.mdf_consume(_feedHandle, timeout);

        /// <summary>
        /// Fetches a message from the current consumed data if one is present and fills the output parameters with values representing the message fetched.
        /// </summary>
        /// <param name="mref">The fetched message reference. This should match a <see cref="MessageReference"/> value.</param>
        /// <param name="mclass">The fetched message class. This should match a <see cref="MessageClasses"/> value. The message class is normally only used internally and is supplied to the client for completeness and transparency. The client should under most circumstances only use the message reference in order to determine which message it has received.</param>
        /// <param name="insref">The fetched instrument reference, which is the unique id of an instrument.</param>
        /// <returns><see langword="true" /> if a message was returned (and the <paramref name="mref"/>, <paramref name="mclass"/> and <paramref name="insref"/> fields will be filled) or <see langword="false" /> if there are no more messages in the current consumed data (or an error occured).</returns>
        /// <remarks>The corresponding native function is mdf_get_next_message.</remarks>
        public bool GetNextMessage(out int mref, out int mclass, out ulong insref)
        {
            mref = default;
            mclass = default;
            insref = default;
            return _nativeImplementation.mdf_get_next_message(_feedHandle, ref mref, ref mclass, ref insref) == 1;
        }

        /// <summary>
        /// Fetches a message from the current consumed data if one is present and fills the output parameters with values representing the message fetched.
        /// </summary>
        /// <param name="mref">The fetched message reference. This should match a <see cref="MessageReference"/> value.</param>
        /// <param name="insref">The fetched instrument reference, which is the unique id of an instrument.</param>
        /// <returns><see langword="true" /> if a message was returned (and the <paramref name="mref"/> and <paramref name="insref"/> fields will be filled) or <see langword="false" /> if there are no more messages in the current consumed data (or an error occured).</returns>
        /// <remarks>The corresponding native function is mdf_get_next_message2. If this function isn't included in the installed version of the native library, the mdf_get_next_message function will be called instead.</remarks>
        public bool GetNextMessage(out ushort mref, out ulong insref)
        {
            mref = default;
            insref = default;

            if (_nativeImplementation.mdf_get_next_message2 == default)
            {
                bool ret = GetNextMessage(out int messageReference, out _, out insref);
                if (messageReference >= ushort.MinValue && messageReference <= ushort.MaxValue)
                    mref = (ushort)messageReference;
                return ret;
            }

            return _nativeImplementation.mdf_get_next_message2(_feedHandle, ref mref, ref insref) == 1;
        }

        /// <summary>
        /// Fetches a message from the current consumed data if one is present and fills the output parameters with values representing the message fetched.
        /// </summary>
        /// <param name="messageReference">The fetched message reference.</param>
        /// <param name="messageClasses">The fetched message class(es). The message class is normally only used internally and is supplied to the client for completeness and transparency. The client should under most circumstances only use the message reference in order to determine which message it has received.</param>
        /// <param name="insref">The fetched instrument reference, which is the unique id of an instrument.</param>
        /// <returns><see langword="true" /> if a message was returned (and the <paramref name="messageReference"/>, <paramref name="messageClasses"/> and <paramref name="insref"/> fields will be filled) or <see langword="false" /> if there are no more messages in the current consumed data (or an error occured).</returns>
        /// <exception cref="InvalidOperationException">An unknown/undefined message reference was fetched.</exception>
        /// <remarks>The corresponding native function is mdf_get_next_message.</remarks>
        public bool GetNextMessage(out MessageReference messageReference, out MessageClasses messageClasses, out ulong insref)
        {
            bool ret = GetNextMessage(out int mref, out int mclass, out insref);
            switch (ret)
            {
                case true:
                    if (!s_messageReferences.Contains(mref))
                        throw new InvalidOperationException($"{mref} is an unknown message reference.");
                    messageReference = (MessageReference)mref;
                    messageClasses = (MessageClasses)mclass;
                    break;
                default:
                    messageReference = default;
                    messageClasses = default;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Fetches a message from the current consumed data if one is present and fills the output parameters with values representing the message fetched.
        /// </summary>
        /// <param name="messageReference">The fetched message reference.</param>
        /// <param name="insref">The fetched instrument reference, which is the unique id of an instrument.</param>
        /// <returns><see langword="true" /> if a message was returned (and the <paramref name="messageReference"/> and <paramref name="insref"/> fields will be filled) or <see langword="false" /> if there are no more messages in the current consumed data (or an error occured).</returns>
        /// <exception cref="InvalidOperationException">An unknown/undefined message reference was fetched.</exception>
        /// <remarks>The corresponding native function is mdf_get_next_message.</remarks>
        public bool GetNextMessage(out MessageReference messageReference, out ulong insref)
        {
            bool ret = GetNextMessage(out ushort mref, out insref);
            switch (ret)
            {
                case true:
                    if (!s_messageReferences.Contains(mref))
                        throw new InvalidOperationException($"{mref} is an unknown message reference.");
                    messageReference = (MessageReference)mref;
                    break;
                default:
                    messageReference = default;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Fetches the next field from the current message.
        /// </summary>
        /// <param name="tag">The field tag. This should match a <see cref="Field"/> value.</param>
        /// <param name="value">A memory span that contains the bytes of the UTF-8 string representation of the field value.</param>
        /// <returns><see langword="true" /> if a field was returned, or <see langword="false" /> if there are no more fields in the current message.</returns>
        /// <remarks>The corresponding native function is mdf_get_next_field.</remarks>
        public bool GetNextField(out uint tag, out ReadOnlySpan<byte> value)
        {
            tag = default;
            IntPtr pointer = default;

            int ret = _nativeImplementation.mdf_get_next_field(_feedHandle, ref tag, ref pointer);
            if (ret != 1)
            {
                value = default;
                return false;
            }

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

            return true;
        }

        /// <summary>
        /// Fetches the next field from the current message.
        /// </summary>
        /// <param name="field">The field tag.</param>
        /// <param name="value">A memory span that contains the bytes of the UTF-8 string representation of the field value.</param>
        /// <returns><see langword="true" /> if a field was returned, or <see langword="false" /> if there are no more fields in the current message.</returns>
        /// <exception cref="InvalidOperationException">An unknown/undefined field tag was fetched.</exception>
        /// <remarks>The corresponding native function is mdf_get_next_field.</remarks>
        public bool GetNextField(out Field field, out ReadOnlySpan<byte> value)
        {
            bool ret = GetNextField(out uint tag, out value);
            switch (ret)
            {
                case true:
                    if (!s_fields.Contains(tag))
                        throw new InvalidOperationException($"{tag} is an unknown tag / field.");
                    field = (Field)tag;
                    break;
                default:
                    field = default;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// <para>Connects to the first server in servers, which can be a comma separated list of 'host:port' pairs, where 'host' can be a DNS host name or an ip address(IPv6 addressed must be enclosed in brackets). If the server does not respond in time (<see cref="ConnectionTimeout"/>), the next server in the list will be tried until the list is empty and the function finally fails.</para>
        /// <para>Upon connect, the API will verify the authenticity of the server using it's public RSA key, and a secure channel will be set up between the client and the server before the function signals success.</para>
        /// <para>If this is the first successful connect on the API handle, or the templates has been updated since the last time the API was connected, the server will send a <see cref="MessageReference.MDF_M_MESSAGESREFERENCE"/> message to the client containing the new message templates. So you could receive one message before a successful logon request.</para>
        /// </summary>
        /// <param name="servers">A comma separated list of 'host:port' pairs, where 'host' can be a DNS host name or an ip address (IPv6 addressed must be enclosed in brackets).</param>
        /// <returns><see langword="true" /> if a connection has been set up or <see langword="false" /> if a connection attempt failed with every server on the list.</returns>
        /// <remarks>The corresponding native function is mdf_connect.</remarks>
        public bool Connect(string servers)
        {
            if (string.IsNullOrEmpty(servers))
                return false;

            int length = Encoding.UTF8.GetMaxByteCount(servers.Length);
            byte[] bytes = ArrayPool<byte>.Shared.Rent(length + 1);
            try
            {
                unsafe
                {
                    fixed (char* c = servers)
                    fixed (byte* b = bytes)
                    {
                        int bytesWritten = Encoding.UTF8.GetBytes(c, servers.Length, b, length);
                        b[bytesWritten] = 0;
                        return _nativeImplementation.mdf_connect(_feedHandle, (IntPtr)b) == 1;
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        /// <summary>
        /// Disconnect a connected API handle. Safe to call even if the handle is already disconnected.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_disconnect.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Disconnect() => _nativeImplementation.mdf_disconnect(_feedHandle);

        /// <summary>
        /// Sends all the active messages in a managed message handle to the server. The message handle will not be reset, so this has to be performed manually by calling <see cref="Message.Reset()"/>.
        /// </summary>
        /// <param name="message">The managed message handle.</param>
        /// <returns><see langword="true" /> if there were no errors detected when sending the data, or <see langword="false" /> if an error was detected (such as not connected to any server). Due to the nature of TCP/IP, a successful return code does not guarantee that the server has received the messages.</returns>
        /// <remarks>The corresponding native function is mdf_message_send.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Send(Message message) =>
            message != null && _nativeImplementation.mdf_message_send(_feedHandle, message.Handle) == 1;

        /// <summary>
        /// Calls <see cref="Send(Message)"/> to send all the active messages in a managed message handle to the server if <paramref name="message"/> is a <see cref="Message"/>. For any other implementation of <see cref="IMessage" />, the method always returns <see langword="false" />.
        /// </summary>
        /// <param name="message">An implementation of the managed message handle.</param>
        /// <returns><see langword="true" /> if <paramref name="message"/> is a <see cref="Message"/> and there were no errors detected when sending the data, or <see langword="false" /> if an error was detected or if <paramref name="message"/> is not a <see cref="Message"/>.</returns>
        bool IMarketDataFeed<TCallbackUserData, TStatusCallbackUserData>.Send(IMessage message) =>
            message is Message messageWithHandle && Send(messageWithHandle);

        /// <summary>
        /// Releases any resources used by the <see cref="MarketDataFeed{TCallbackData,TStatusCallbackData}"/> instance.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_destroy.</remarks>
        public void Dispose()
        {
            _nativeImplementation?.mdf_destroy(_feedHandle);
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
            if (_nativeImplementation.mdf_get_int_property(_feedHandle, option, ref value) != 1)
                throw new InvalidOperationException(UnknownOptionMessage);
            return value;
        }

        private ulong GetUInt64Property(MDF_OPTION option)
        {
            ulong value = default;
            if (_nativeImplementation.mdf_get_ulong_property(_feedHandle, option, ref value) != 1)
                throw new InvalidOperationException(UnknownOptionMessage);
            return value;
        }

        private string GetStringProperty(MDF_OPTION option)
        {
            IntPtr value = default;
            if (_nativeImplementation.mdf_get_property(_feedHandle, option, ref value) != 1)
                throw new InvalidOperationException(UnknownOptionMessage);

            if (value == IntPtr.Zero)
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
                        Encoding.UTF8.GetChars(p, byteCount, c, charCount);
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