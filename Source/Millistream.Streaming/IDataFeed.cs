namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a thread-safe request based data feed that can be used to subscribe to messages sent by the server.
    /// </summary>
    public interface IDataFeed
    {
        /// <summary>
        /// The number of seconds before determining that a connect attempt has timed out. Valid values are 1 to 60. The default value is 5.
        /// </summary>
        int ConnectionTimeout { get; set; }

        /// <summary>
        /// The current API error code.
        /// </summary>
        Error ErrorCode { get; }

        /// <summary>
        /// The number of seconds the connection must be idle before the API sends a heartbeat request to the server. Valid values are 1 to 86400. The default is 30.
        /// </summary>
        int HeartbeatInterval { get; set; }

        /// <summary>
        /// How many outstanding hearbeat requests to allow before the connection is determined to be disconnected. Valid values are 1 to 100. The default is 2.
        /// </summary>
        int MaximumMissedHeartbeats { get; set; }

        /// <summary>
        /// Controls whether Nagle's algorithm is used on the TCP connection. It's enabled by default.
        /// </summary>
        bool NoDelay { get; set; }

        /// <summary>
        /// The number of bytes received from the server since connecting.
        /// </summary>
        ulong ReceivedBytes { get; }

        /// <summary>
        /// The total number of bytes sent to the server.
        /// </summary>
        ulong SentBytes { get; }

        /// <summary>
        /// The time difference in number of seconds between the client and the server. The value should be added to the current time on the client in order to get the server time. Please not that this value can be negative if the client clock is ahead of the server clock.
        /// </summary>
        int TimeDifference { get; }

        /// <summary>
        /// Occurs whenever there is a change of the status of the connection.
        /// </summary>
        event ConnnectionStatusChangedEventHandler ConnectionStatusChanged;

        /// <summary>
        /// Occurs whenever any data is received.
        /// </summary>
        event DataReceivedEventHandler DataReceived;

        /// <summary>
        /// Connects to the first server in <paramref name="host"/>, which can be a comma separated list of 'host:port' pairs, where 'host' can be a DNS host name or an ip address (IPv6 addressed must be enclosed in brackets).
        /// The method will try each server in turn until it find one that answers.
        /// </summary>
        /// <param name="host">A comma separated list of 'host:port' pairs to try to connect to.</param>
        /// <param name="username">The username to authenticate with the server.</param>
        /// <param name="password">The password to authenticate with the server.</param>
        /// <returns>A value indicating whether the connect attempt was successful.</returns>
        bool Connect(string host, string username, string password);

        /// <summary>
        /// Disconnects from the feed.
        /// </summary>
        /// <remarks>
        /// You must call the <see cref="Connect(string, string, string)"/> method before calling this method.
        /// </remarks>
        void Disconnect();

        /// <summary>
        /// Sends a request to the server.
        /// </summary>
        /// <param name="requestMessage">The reqest message to be sent to the server.</param>
        /// <remarks>
        /// You must call the <see cref="Connect(string, string, string)"/> method before calling this method.
        /// </remarks>
        void Request(RequestMessage requestMessage);
    }
}