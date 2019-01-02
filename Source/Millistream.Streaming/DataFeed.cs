using Millistream.Streaming.Rx;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a thread-safe request based data feed that can be used to subscribe to messages sent by the server.
    /// </summary>
    public class DataFeed : IDataFeed, IDisposable
    {
        #region Constants
        internal const int MinConnectionTimeout = 1;
        internal const int MaxConnectionTimeout = 60;
        internal const int MinHeartbeatInterval = 1;
        internal const int MaxHeartbeatInterval = 86400;
        internal const int MinMissedHeartbeats = 1;
        internal const int MaxMissedHeartbeats = 100;
        #endregion

        #region Fields
        private static readonly HashSet<int> s_messageReferences = new HashSet<int>((int[])Enum.GetValues(typeof(MessageReference)));
        private static readonly HashSet<uint> s_fieldReferences = new HashSet<uint>((uint[])Enum.GetValues(typeof(Field)));
        private static readonly Func<ResponseMessage> s_responseMessageFactory = () => new ResponseMessage();
        private readonly object _lock = new object();
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ObjectPool<ResponseMessage> _objectPool = new ObjectPool<ResponseMessage>(s_responseMessageFactory);
        private readonly Subject<ResponseMessage> _subject = new Subject<ResponseMessage>();
        private readonly INativeImplementation _nativeImplementation;
        private readonly IntPtr _feedHandle;
        private readonly IntPtr _messageHandle;
        private readonly IMessage _message;
        private readonly mdf_status_callback _statusCallback;
        private readonly mdf_data_callback _dataCallback;
        private bool _hasConnected;
        private Task _consumeTask;
        private bool _isDisposed;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of the <see cref="DataFeed"/> class.
        /// </summary>
        public DataFeed() : this(GetNativeImplementation()) { }

        internal DataFeed(INativeImplementation nativeImplementation)
        {
            _nativeImplementation = nativeImplementation ?? throw new ArgumentNullException(nameof(nativeImplementation));
            _feedHandle = _nativeImplementation.mdf_create();
            _messageHandle = _nativeImplementation.mdf_message_create();
            _message = new Message(_nativeImplementation, _messageHandle);
            _statusCallback = OnConnectionStatusChanged;
            _dataCallback = OnDataReceived;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The number of seconds before determining that a connect attempt has timed out. Valid values are 1 to 60. The default value is 5.
        /// </summary>
        public int ConnectionTimeout
        {
            get => (int)GetProperty(MDF_OPTION.MDF_OPT_CONNECT_TIMEOUT);
            set => SetProperty(MDF_OPTION.MDF_OPT_CONNECT_TIMEOUT, value, MinConnectionTimeout, MaxConnectionTimeout);
        }

        /// <summary>
        /// The number of seconds to wait before if there currently is no data when consuming the feed. If set to zero (0) the consume function will return immediately. The default value is 10.
        /// </summary>
        public int ConsumeTimeout { get; set; } = 10;

        /// <summary>
        /// An observable stream of data produced by the feed. This is where all response messages are read from.
        /// </summary>
        public IObservable<ResponseMessage> Data
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    return _subject;
                }
            }
        }

        /// <summary>
        /// The current API error code.
        /// </summary>
        public Error ErrorCode => (Error)GetProperty(MDF_OPTION.MDF_OPT_ERROR);

        /// <summary>
        /// The number of seconds the connection must be idle before the API sends a heartbeat request to the server. Valid values are 1 to 86400. The default is 30.
        /// </summary>
        public int HeartbeatInterval
        {
            get => (int)GetProperty(MDF_OPTION.MDF_OPT_HEARTBEAT_INTERVAL);
            set => SetProperty(MDF_OPTION.MDF_OPT_HEARTBEAT_INTERVAL, value, MinHeartbeatInterval, MaxHeartbeatInterval);
        }

        /// <summary>
        /// How many outstanding hearbeat requests to allow before the connection is determined to be disconnected. Valid values are 1 to 100. The default is 2.
        /// </summary>
        public int MaximumMissedHeartbeats
        {
            get => (int)GetProperty(MDF_OPTION.MDF_OPT_HEARTBEAT_MAX_MISSED);
            set => SetProperty(MDF_OPTION.MDF_OPT_HEARTBEAT_MAX_MISSED, value, MinMissedHeartbeats, MaxMissedHeartbeats);
        }

        /// <summary>
        /// Controls whether Nagle's algorithm is used on the TCP connection. It's enabled by default.
        /// </summary>
        public bool NoDelay
        {
            get => Convert.ToBoolean((int)GetProperty(MDF_OPTION.MDF_OPT_TCP_NODELAY));
            set
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    _nativeImplementation.mdf_set_property(_feedHandle, MDF_OPTION.MDF_OPT_TCP_NODELAY, new IntPtr(value ? 1 : 0));
                }
            }
        }

        /// <summary>
        /// The number of bytes received from the server since connecting.
        /// </summary>
        public ulong ReceivedBytes => (ulong)GetProperty(MDF_OPTION.MDF_OPT_RCV_BYTES);

        /// <summary>
        /// The total number of bytes sent to the server.
        /// </summary>
        public ulong SentBytes => (ulong)GetProperty(MDF_OPTION.MDF_OPT_SENT_BYTES);

        /// <summary>
        /// The time difference in number of seconds between the client and the server. The value should be added to the current time on the client in order to get the server time. Please not that this value can be negative if the client clock is ahead of the server clock.
        /// </summary>
        public int TimeDifference => (int)GetProperty(MDF_OPTION.MDF_OPT_TIME_DIFFERENCE);
        #endregion

        #region Events
        private ConnnectionStatusChangedEventHandler _connectionStatusChangedEvent;
        /// <summary>
        /// Occurs whenever there is a change of the status of the connection.
        /// </summary>
        public event ConnnectionStatusChangedEventHandler ConnectionStatusChanged
        {
            add
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    _connectionStatusChangedEvent += value;
                }
            }
            remove
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    _connectionStatusChangedEvent -= value;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Connects to the first server in <paramref name="host"/>, which can be a comma separated list of 'host:port' pairs, where 'host' can be a DNS host name or an ip address (IPv6 addressed must be enclosed in brackets).
        /// The method will try each server in turn until it find one that answers.
        /// </summary>
        /// <param name="host">A comma separated list of 'host:port' pairs to try to connect to.</param>
        /// <param name="username">The username to authenticate with the server.</param>
        /// <param name="password">The password to authenticate with the server.</param>
        /// <returns>A value indicating whether the connect attempt was successful.</returns>
        public bool Connect(string host, string username, string password)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException(nameof(host));
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

            lock (_lock)
            {
                ThrowIfDisposed();
                
                //enable the connection status callback
                if (_hasConnected)
                    Disconnect();
                else
                    _nativeImplementation.mdf_set_property(_feedHandle, MDF_OPTION.MDF_OPT_STATUS_CALLBACK_FUNCTION, Marshal.GetFunctionPointerForDelegate(_statusCallback));

                //try to connect
                if (_nativeImplementation.mdf_connect(_feedHandle, host) != 1)
                    return false;

                //try to login
                _nativeImplementation.mdf_message_add(_messageHandle, 0, (int)MessageReference.MDF_M_LOGON);
                _nativeImplementation.mdf_message_add_string(_messageHandle, (uint)Field.MDF_F_USERNAME, username);
                _nativeImplementation.mdf_message_add_string(_messageHandle, (uint)Field.MDF_F_PASSWORD, password);
                _nativeImplementation.mdf_message_send(_feedHandle, _messageHandle);
                _nativeImplementation.mdf_message_reset(_messageHandle);

                DateTime startTime = DateTime.UtcNow;
                int connectionTimeout = ConnectionTimeout;
                do
                {
                    int ret = _nativeImplementation.mdf_consume(_feedHandle, 1);
                    switch (ret)
                    {
                        case 1:
                            int mref = 0;
                            int mclass = 0;
                            uint insref = 0;
                            while (_nativeImplementation.mdf_get_next_message(_feedHandle, ref mref, ref mclass, ref insref) == 1)
                            {
                                switch (mref)
                                {
                                    case (int)MessageReference.MDF_M_LOGONGREETING:
                                        //enable data callbacks
                                        _nativeImplementation.mdf_set_property(_feedHandle, MDF_OPTION.MDF_OPT_DATA_CALLBACK_FUNCTION, Marshal.GetFunctionPointerForDelegate(_dataCallback));
                                        //start consuming the feed on a background thread
                                        _consumeTask = Task.Factory.StartNew(Consume, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
                                        _hasConnected = true;
                                        return true;
                                    case (int)MessageReference.MDF_M_LOGOFF:
                                        return false;
                                }
                            }
                            break;
                        default:
                            return false;
                    }
                } while (DateTime.UtcNow.Subtract(startTime).TotalSeconds < connectionTimeout);
            }
            return false;
        }

        /// <summary>
        /// Disconnects from the feed.
        /// </summary>
        public void Disconnect()
        {
            lock (_lock)
            {
                ThrowIfDisposed();
                //wait for the consume task to finish
                if (_consumeTask != null)
                {
                    _cancellationTokenSource.Cancel();
                    _consumeTask.Wait();
                }
                Logout();
                _nativeImplementation.mdf_disconnect(_feedHandle);
            }
        }

        /// <summary>
        /// Resets and recycles an instance of a <see cref="ResponseMessage" /> for reuse.
        /// </summary>
        /// <param name="responseMessage">The <see cref="ResponseMessage" /> instance to return to the pool of recycled objects.</param>
        public void Recycle(ResponseMessage responseMessage)
        {
            if (responseMessage == null)
                throw new ArgumentNullException(nameof(responseMessage));

            lock (_lock)
                ThrowIfDisposed();

            responseMessage.ResetState();
            _objectPool.Free(responseMessage);
        }

        /// <summary>
        /// Sends a request to the server.
        /// </summary>
        /// <param name="requestMessage">The reqest message to be sent to the server.</param>
        /// <remarks>
        /// You must call the <see cref="Connect(string, string, string)"/> method before calling this method.
        /// </remarks>
        public void Request(RequestMessage requestMessage)
        {
            if (requestMessage == null)
                throw new ArgumentNullException(nameof(requestMessage));

            lock (_lock)
            {
                ThrowIfDisposed();

                if (!_hasConnected)
                    throw new InvalidOperationException($"You must call the {nameof(Connect)} method to connect to the feed before you call {nameof(Request)}.");

                requestMessage.AddFields(_message);
                _nativeImplementation.mdf_message_send(_feedHandle, _messageHandle);
                _nativeImplementation.mdf_message_reset(_messageHandle);
            }
        }

        private IntPtr GetProperty(MDF_OPTION option)
        {
            lock (_lock)
            {
                ThrowIfDisposed();
                IntPtr value = new IntPtr();
                _nativeImplementation.mdf_get_property(_feedHandle, option, ref value);
                return value;
            }
        }

        private void SetProperty(MDF_OPTION option, int value, int minValue, int maxValue)
        {
            lock (_lock)
            {
                ThrowIfDisposed();
                if (value < minValue || value > maxValue)
                    throw new ArgumentOutOfRangeException();
                _nativeImplementation.mdf_set_property(_feedHandle, option, new IntPtr(value));
            }
        }

        private void Consume(object state)
        {
            CancellationToken cancellationToken = (CancellationToken)state;
            while (_nativeImplementation.mdf_consume(_feedHandle, ConsumeTimeout) != -1)
                if (cancellationToken.IsCancellationRequested)
                    break;
        }

        private void Logout()
        {
            //disable the any data callbacks
            _nativeImplementation.mdf_set_property(_feedHandle, MDF_OPTION.MDF_OPT_DATA_CALLBACK_FUNCTION, IntPtr.Zero);
            //try to log out
            _nativeImplementation.mdf_message_add(_messageHandle, 0, (int)MessageReference.MDF_M_LOGOFF);
            int send = _nativeImplementation.mdf_message_send(_feedHandle, _messageHandle);
            _nativeImplementation.mdf_message_reset(_messageHandle);
            if (send == 1)
            {
                DateTime startTime = DateTime.UtcNow;
                int connectionTimeout = ConnectionTimeout;
                do
                {
                    int consume = _nativeImplementation.mdf_consume(_feedHandle, 1);
                    switch (consume)
                    {
                        case 1:
                            int mref = 0;
                            int mclass = 0;
                            uint insref = 0;
                            while (_nativeImplementation.mdf_get_next_message(_feedHandle, ref mref, ref mclass, ref insref) == 1)
                                if (mref == (int)MessageReference.MDF_M_LOGOFF)
                                    return;
                            break;
                        case -1:
                            return;
                    }
                } while (DateTime.UtcNow.Subtract(startTime).TotalSeconds < connectionTimeout);
            }
        }

        private void OnConnectionStatusChanged(IntPtr data, ConnectionStatus connectionStatus, string host, string ip)
        {
            ConnnectionStatusChangedEventHandler eventHandler;
            lock (_lock)
                eventHandler = _connectionStatusChangedEvent;
            eventHandler?.Invoke(this, new ConnectionStatusChangedEventArgs(host, ip, connectionStatus));
        }

        private void OnDataReceived(IntPtr userData, IntPtr handle)
        {
            int messageReference = 0;
            int messageClass = 0;
            uint instrumentId = 0;
            while (_nativeImplementation.mdf_get_next_message(_feedHandle, ref messageReference, ref messageClass, ref instrumentId) == 1)
            {
                if (s_messageReferences.Contains(messageReference))
                {
                    ResponseMessage message = _objectPool.Allocate() ?? s_responseMessageFactory();
                    message.InstrumentReference = instrumentId;
                    message.MessageReference = (MessageReference)messageReference;
                    message.MessageClass = messageClass;

                    uint fieldTag = 0;
                    int messageOffset = 0;
                    IntPtr value = new IntPtr();
                    while (_nativeImplementation.mdf_get_next_field(_feedHandle, ref fieldTag, ref value) == 1)
                    {
                        if (s_fieldReferences.Contains(fieldTag))
                        {
                            Field field = (Field)fieldTag;
                            if (value != IntPtr.Zero)
                            {
                                unsafe
                                {
                                    try
                                    {
                                        byte* pointer = (byte*)value;
                                        ReadOnlySpan<byte> span;
                                        int fieldOffset = 0;
                                        do
                                        {
                                            span = new ReadOnlySpan<byte>(pointer + fieldOffset++, 1);
                                        } while (span[0] != 0);

                                        int length = fieldOffset - 1;
                                        span = new ReadOnlySpan<byte>(pointer, length);
                                        for (int i = 0; i < length; i++)
                                            message.Data.Add(span[i]);
                                        message.SetField(field, new ReadOnlyMemory<byte>(message.Data.Items, messageOffset, length));
                                        messageOffset += length;
                                    }
                                    catch (Exception ex)
                                    {
                                        _subject.OnError(ex);
                                    }
                                }
                            }
                            else
                            {
                                message.SetField(field, ReadOnlyMemory<byte>.Empty);
                            }
                        }
                    }
                    _subject.OnNext(message);
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(typeof(DataFeed).FullName);
        }

        private static INativeImplementation GetNativeImplementation()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new NativeWindowsImplementation();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return new NativeUnixImplementation();

            throw new PlatformNotSupportedException();
        }

        #region IDisposable
        /// <summary>
        /// Releases any resources used by the <see cref="DataFeed"/> instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (_lock)
            {
                if (!_isDisposed)
                {
                    if (disposing)
                    {
                        _cancellationTokenSource?.Dispose();
                        _subject.Dispose();
                    }
                    _nativeImplementation.mdf_message_destroy(_messageHandle);
                    _nativeImplementation.mdf_destroy(_feedHandle);
                    _isDisposed = true;
                }
            }
        }

        ~DataFeed() => Dispose(false);
        #endregion
        #endregion
    }
}
