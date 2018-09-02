using System;

namespace Millistream.Streaming
{
    /// <summary>
    /// Provides data for the ConnnectionStatus event.
    /// </summary>
    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        internal ConnectionStatusChangedEventArgs(string host, string ip, ConnectionStatus connectionStatus)
        {
            Host = host;
            Ip = ip;
            ConnectionStatus = connectionStatus;
        }

        /// <summary>
        /// The host name of the server.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// The IP address of the server.
        /// </summary>
        public string Ip { get; }

        /// <summary>
        /// The current connection status.
        /// </summary>
        public ConnectionStatus ConnectionStatus { get; }
    }
}
