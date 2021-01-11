using Millistream.Streaming.Rx;
using System;
using System.Threading;
using System.Threading.Tasks;
using MarketDataFeed = Millistream.Streaming.MarketDataFeed<object, object>;

namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a thread-safe request based data feed that can be used to subscribe to messages sent by the server.
    /// </summary>
    public class DataFeed : IDataFeed, IDisposable
    {
        #region Fields
        private static readonly Func<ResponseMessage> s_responseMessageFactory = () => new ResponseMessage();
        private readonly object _lock = new object();
        private readonly ObjectPool<ResponseMessage> _objectPool = new ObjectPool<ResponseMessage>(s_responseMessageFactory);
        private readonly Subject<ResponseMessage> _subject = new Subject<ResponseMessage>();
        private readonly MarketDataFeed _mdf;
        private readonly Message _message;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _hasConnected;
        private Task _consumeTask;
        private bool _isDisposed;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of the <see cref="DataFeed"/> class.
        /// </summary>
        public DataFeed() : this(NativeImplementation.Get()) { }

        internal DataFeed(INativeImplementation nativeImplementation)
        {
            _mdf = new MarketDataFeed(nativeImplementation);
            _message = new Message(nativeImplementation);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The number of seconds before determining that a connect attempt has timed out. Valid values are 1 to 60. The default value is 5.
        /// </summary>
        public int ConnectionTimeout
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    return _mdf.ConnectionTimeout;
                }
            }
            set
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    _mdf.ConnectionTimeout = value;
                }
            }
        }

        private int _consumeTimeout = -10000;
        /// <summary>
        /// The number of seconds to wait for if there currently is no data when consuming the feed. If set to zero (0) the consume function will return immediately. If set to a negative value, the wait period is treated as a number of microseconds instead of a number of seconds (i.e. -1000 will wait a maximum of 1000µs). The default value is -10000.
        /// </summary>
        public int ConsumeTimeout
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    return _consumeTimeout;
                }
            }
            set
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    _consumeTimeout = value;
                }
            }
        }

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
        public Error ErrorCode
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    return _mdf.ErrorCode;
                }
            }
        }

        /// <summary>
        /// The number of seconds the connection must be idle before the API sends a heartbeat request to the server. Valid values are 1 to 86400. The default is 30.
        /// </summary>
        public int HeartbeatInterval
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    return _mdf.HeartbeatInterval;
                }
            }
            set
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    _mdf.HeartbeatInterval = value;
                }
            }
        }

        /// <summary>
        /// How many outstanding hearbeat requests to allow before the connection is determined to be disconnected. Valid values are 1 to 100. The default is 2.
        /// </summary>
        public int MaximumMissedHeartbeats
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    return _mdf.MaximumMissedHeartbeats;
                }
            }
            set
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    _mdf.MaximumMissedHeartbeats = value;
                }
            }
        }

        /// <summary>
        /// Controls whether Nagle's algorithm is used on the TCP connection. It's enabled by default.
        /// </summary>
        public bool NoDelay
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    return _mdf.NoDelay;
                }
            }
            set
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    _mdf.NoDelay = value;
                }
            }
        }

        /// <summary>
        /// The number of bytes received from the server since connecting.
        /// </summary>
        public ulong ReceivedBytes
        {
            get
            {
                lock(_lock)
                {
                    ThrowIfDisposed();
                    return _mdf.ReceivedBytes;
                }
            }
        }

        /// <summary>
        /// The total number of bytes sent to the server.
        /// </summary>
        public ulong SentBytes
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    return _mdf.SentBytes;
                }
            }
        }

        /// <summary>
        /// The time difference in number of seconds between the client and the server. The value should be added to the current time on the client in order to get the server time. Please not that this value can be negative if the client clock is ahead of the server clock.
        /// </summary>
        public int TimeDifference
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    return _mdf.TimeDifference;
                }
            }
        }
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
        /// <exception cref="ArgumentNullException" />
        public bool Connect(string host, string username, string password) =>
            Connect(host, username, password, null);

        /// <summary>
        /// Connects to the first server in <paramref name="host"/>, which can be a comma separated list of 'host:port' pairs, where 'host' can be a DNS host name or an ip address (IPv6 addressed must be enclosed in brackets).
        /// The method will try each server in turn until it find one that answers.
        /// </summary>
        /// <param name="host">A comma separated list of 'host:port' pairs to try to connect to.</param>
        /// <param name="username">The username to authenticate with the server.</param>
        /// <param name="password">The password to authenticate with the server.</param>
        /// <param name="extraCredential">An extra credential that is required if the account is setup to use two-factor authentication.</param>
        /// <returns>A value indicating whether the connect attempt was successful.</returns>
        /// <exception cref="ArgumentNullException" />
        public bool Connect(string host, string username, string password, string extraCredential)
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
                    _mdf.StatusCallback = OnConnectionStatusChanged;

                //try to connect
                if (!_mdf.Connect(host))
                    return false;

                //try to login
                _message.Add(0, (int)MessageReference.MDF_M_LOGON);
                _message.AddString((uint)Field.MDF_F_USERNAME, username);
                _message.AddString((uint)Field.MDF_F_PASSWORD, password);
                if (!string.IsNullOrEmpty(extraCredential))
                    _message.AddString((uint)Field.MDF_F_EXTRACREDENTIAL, extraCredential);
                _mdf.Send(_message);
                _message.Reset();

                DateTime startTime = DateTime.UtcNow;
                int connectionTimeout = ConnectionTimeout;
                do
                {
                    int ret = _mdf.Consume(1);
                    switch (ret)
                    {
                        case 1:
                            while (_mdf.GetNextMessage(out int mref, out int mclass, out ulong insref))
                            {
                                switch (mref)
                                {
                                    case (int)MessageReference.MDF_M_LOGONGREETING:
                                        //enable data callbacks
                                        _mdf.DataCallback = OnDataReceived;
                                        //start consuming the feed on a background thread
                                        _cancellationTokenSource = new CancellationTokenSource();
                                        _consumeTask = Task.Factory.StartNew(Consume, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
                                        _hasConnected = true;
                                        return true;
                                    case (int)MessageReference.MDF_M_LOGOFF:
                                        return false;
                                }
                            }
                            break;
                        case -1:
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
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    //release lock and wait for the consume thread to finish
                    Monitor.Wait(_lock);
                    _consumeTask?.Wait();
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }
                Logout();
                _mdf.Disconnect();
                _hasConnected = false;
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
        /// <param name="requestMessage">The request message to be sent to the server.</param>
        /// <remarks>
        /// You must call the <see cref="Connect(string, string, string)"/> method before calling this method.
        /// </remarks>
        /// <exception cref="ArgumentNullException" />
        public void Request(RequestMessage requestMessage)
        {
            if (requestMessage == null)
                throw new ArgumentNullException(nameof(requestMessage));

            lock (_lock)
            {
                ThrowIfDisposed();

                if (!_hasConnected)
                    throw new InvalidOperationException($"You must call the {nameof(Connect)} method to connect to the feed and authenticate before you call {nameof(Request)}.");

                requestMessage.AddFields(_message);
                _mdf.Send(_message);
                _message.Reset();
            }
        }

        private void Consume(object state)
        {
            CancellationToken cancellationToken = (CancellationToken)state;
            while (true)
            {
                lock (_lock)
                {
                    if (cancellationToken.IsCancellationRequested || _mdf.Consume(_consumeTimeout) == -1)
                    {
                        Monitor.Pulse(_lock);
                        break;
                    }
                }
            }
        }

        private void Logout()
        {
            //disable the any data callbacks
            _mdf.DataCallback = null;
            //try to log out
            _message.Add(0, (int)MessageReference.MDF_M_LOGOFF);
            bool sent = _mdf.Send(_message);
            _message.Reset();
            if (sent)
            {
                DateTime startTime = DateTime.UtcNow;
                int connectionTimeout = ConnectionTimeout;
                do
                {
                    int consume = _mdf.Consume(1);
                    switch (consume)
                    {
                        case 1:
                            while (_mdf.GetNextMessage(out int mref, out int _, out ulong _))
                                if (mref == (int)MessageReference.MDF_M_LOGOFF)
                                    return;
                            break;
                        case -1:
                            return;
                    }
                } while (DateTime.UtcNow.Subtract(startTime).TotalSeconds < connectionTimeout);
            }
        }

        private void OnConnectionStatusChanged(object _, ConnectionStatus connectionStatus, string host, string ip)
        {
            ConnnectionStatusChangedEventHandler eventHandler;
            lock (_lock)
                eventHandler = _connectionStatusChangedEvent;
            eventHandler?.Invoke(this, new ConnectionStatusChangedEventArgs(host, ip, connectionStatus));
        }

        private void OnDataReceived(object _, MarketDataFeed<object, object> handle)
        {
            while (_mdf.GetNextMessage(out int messageReference, out int messageClass, out ulong instrumentId))
            {
                if (MarketDataFeed.s_messageReferences.Contains(messageReference))
                {
                    ResponseMessage message = _objectPool.Allocate() ?? s_responseMessageFactory();
                    message.InstrumentReference = instrumentId;
                    message.MessageReference = (MessageReference)messageReference;
                    message.MessageClass = messageClass;

                    int messageOffset = 0;
                    while (_mdf.GetNextField(out uint fieldTag, out ReadOnlySpan<byte> value))
                    {
                        if (MarketDataFeed.s_fields.Contains(fieldTag))
                        {
                            Field field = (Field)fieldTag;
                            int length = value.Length;
                            if (length > 0)
                            {
                                try
                                {
                                    for (int i = 0; i < length; i++)
                                        message.Data.Add(value[i]);
                                    message.SetField(field, new ReadOnlyMemory<byte>(message.Data.Items, messageOffset, length));
                                    messageOffset += length;
                                }
                                catch (Exception ex)
                                {
                                    _subject.OnError(ex);
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
                        _mdf.Dispose();
                        _message.Dispose();
                    }
                    _isDisposed = true;
                }
            }
        }

        ~DataFeed() => Dispose(false);
        #endregion
        #endregion
    }
}