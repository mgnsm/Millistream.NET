using System;

namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a managed API handle that can be connected to the system.
    /// </summary>
    /// <typeparam name="TCallbackUserData">The type of the custom user data that will be available to the data callback function.</typeparam>
    /// <typeparam name="TStatusCallbackUserData">The type of the custom user data that will be available to the status callback function.</typeparam>
    public interface IMarketDataFeed<TCallbackUserData, TStatusCallbackUserData>
    {
        /// <summary>
        /// The file descriptor used by the connection. Will be -1 (or INVALID_SOCKET on Windows) if there is no connection.
        /// </summary>
        int FileDescriptor { get; }

        /// <summary>
        /// The current API error code.
        /// </summary>
        Error ErrorCode { get; set; }

        /// <summary>
        /// The number of bytes received by the server since the handle was created.
        /// </summary>
        ulong ReceivedBytes { get; set; }

        /// <summary>
        /// The number of bytes sent by the client since the handle was created.
        /// </summary>
        ulong SentBytes { get; set; }

        /// <summary>
        /// A callback function that will be called by the consume function if there are any messages to decode.
        /// </summary>
        DataCallback<TCallbackUserData, TStatusCallbackUserData> DataCallback { get; set; }

        /// <summary>
        /// Custom userdata that will be available to the data callback function.
        /// </summary>
        TCallbackUserData CallbackUserData { get; set; }

        /// <summary>
        /// A callback function that will be called whenever there is a change of the status of the connection.
        /// </summary>
        StatusCallback<TStatusCallbackUserData> StatusCallback { get; set; }

        /// <summary>
        /// Custom userdata that will be available to the status callback function.
        /// </summary>
        TStatusCallbackUserData StatusCallbackUserData { get; set; }

        /// <summary>
        /// The number of seconds before determining that a connect attempt has timed out. Valid values are 1 to 60. The default value is 5.
        /// </summary>
        int ConnectionTimeout { get; set; }

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
        /// Used internally to enable or disable encryption.
        /// </summary>
        bool NoEncryption { get; set; }

        /// <summary>
        /// The time difference in number of seconds between the client and the server. The value should be added to the current time on the client in order to get the server time. Please not that this value can be negative if the client clock is ahead of the server clock.
        /// </summary>
        int TimeDifference { get; }

        /// <summary>
        /// A numerical address to which the API will bind before attempting to connect to a server in <see cref="Connect"/>. If the bind fails then <see cref="Connect"/> also fails. The string is copied by the API and a <see langword="NULL" /> value can be used in order to "unset" the bind address.
        /// </summary>
        string BindAddress { get; set; }

        /// <summary>
        /// The time difference in number of nanoseconds between the client and the server. The value should be added to the current time on the client in order to get the server time. Please not that this value can be negative if the client clock is ahead of the server clock.
        /// </summary>
        long TimeDifferenceNs { get; }

        /// <summary>
        /// A comma separated list of the message digests that the client will offer to the server upon connect.
        /// </summary>
        public string MessageDigests { get; set; }

        /// <summary>
        /// A comma separated list of the encryption ciphers that the client will offer to the server upon connect.
        /// </summary>
        public string Ciphers { get; set; }

        /// <summary>
        /// The digest chosen by the server. Only available after <see cref="Connect"/> returns.
        /// </summary>
        string MessageDigest { get; }

        /// <summary>
        /// The cipher chosen by the server. Only available after <see cref="Connect"/> returns.
        /// </summary>
        string Cipher { get; }

        /// <summary>
        /// The number of seconds to wait before having to call <see cref="Consume(int)"/>.
        /// </summary>
        public int Timeout { get; }

        /// <summary>
        /// Enables or disables delay-mode in where the server adds the intended delay to each message sent. This also enables the client to set the intended delay of the messages the client sends to the server. It's disabled by default.
        /// </summary>
        public bool HandleDelay { get; set; }

        /// <summary>
        /// Consumes data sent from the server. If there currently is no data the function waits for <paramref name="timeout"/> number of seconds, if <paramref name="timeout"/> is zero (0) the function will return immediately. If <paramref name="timeout"/> is negative then the wait period is treated as number of microseconds instead of number of seconds (i.e. -1000 will wait a maximum of 1000µs).
        /// </summary>
        /// <param name="timeout">The wait period in seconds if positive. If negative, the value is treated as the number of microseconds to wait instead of the number of seconds.</param>
        /// <returns>1 if data has been consumed that needs to be handled by <see cref="GetNextMessage(out int, out int, out ulong)" /> and no callback function has been registered. The function returns 0 on timeout or if a callback function is registered and there was data. On errors, -1 will be returned (and the connection will be dropped).</returns>
        int Consume(int timeout);

        /// <summary>
        /// Fetches a message from the current consumed data if one is present and fills the output parameters with values representing the message fetched.
        /// </summary>
        /// <param name="mref">The fetched message reference. This should match a <see cref="MessageReference"/> value.</param>
        /// <param name="mclass">The fetched message class. This should match a <see cref="MessageClasses"/> value. The message class is normally only used internally and is supplied to the client for completeness and transparency. The client should under most circumstances only use the message reference in order to determine which message it has received.</param>
        /// <param name="insref">The fetched instrument reference, which is the unique id of an instrument.</param>
        /// <returns><see langword="true" /> if a message was returned (and the <paramref name="mref"/>, <paramref name="mclass"/> and <paramref name="insref"/> fields will be filled) or <see langword="false" /> if there are no more messages in the current consumed data (or an error occured).</returns>
        bool GetNextMessage(out int mref, out int mclass, out ulong insref);

        /// <summary>
        /// Fetches a message from the current consumed data if one is present and fills the output parameters with values representing the message fetched.
        /// </summary>
        /// <param name="messageReference">The fetched message reference.</param>
        /// <param name="messageClasses">The fetched message class(es). The message class is normally only used internally and is supplied to the client for completeness and transparency. The client should under most circumstances only use the message reference in order to determine which message it has received.</param>
        /// <param name="insref">The fetched instrument reference, which is the unique id of an instrument.</param>
        /// <returns><see langword="true" /> if a message was returned (and the <paramref name="messageReference"/>, <paramref name="messageClasses"/> and <paramref name="insref"/> fields will be filled) or <see langword="false" /> if there are no more messages in the current consumed data (or an error occured).</returns>
        bool GetNextMessage(out MessageReference messageReference, out MessageClasses messageClasses, out ulong insref);

        /// <summary>
        /// Fetches the next field from the current message.
        /// </summary>
        /// <param name="tag">The field tag. This should match a <see cref="Field"/> value.</param>
        /// <param name="value">A memory span that contains the bytes of the UTF-8 string representation of the field value.</param>
        /// <returns><see langword="true" /> if a field was returned, or <see langword="false" /> if there are no more fields in the current message.</returns>
        bool GetNextField(out uint tag, out ReadOnlySpan<byte> value);

        /// <summary>
        /// Fetches the next field from the current message.
        /// </summary>
        /// <param name="field">The field tag.</param>
        /// <param name="value">A memory span that contains the bytes of the UTF-8 string representation of the field value.</param>
        /// <returns><see langword="true" /> if a field was returned, or <see langword="false" /> if there are no more fields in the current message.</returns>
        bool GetNextField(out Field field, out ReadOnlySpan<byte> value);

        /// <summary>
        /// <para>Connects to the first server in servers, which can be a comma separated list of 'host:port' pairs, where 'host' can be a DNS host name or an ip address(IPv6 addressed must be enclosed in brackets). If the server does not respond in time (<see cref="ConnectionTimeout"/>), the next server in the list will be tried until the list is empty and the function finally fails.</para>
        /// <para>Upon connect, the API will verify the authenticity of the server using it's RSA key, and a secure channel will be set up between the client and the server before the function signals success.</para>
        /// <para>If this is the first successful connect on the API handle, or the templates has been updated since the last time the API was connected, the server will send a <see cref="MessageReference.MDF_M_MESSAGESREFERENCE"/> message to the client containing the new message templates. So you could receive one message before a successful logon request.</para>
        /// </summary>
        /// <param name="servers">A comma separated list of 'host:port' pairs, where 'host' can be a DNS host name or an ip address (IPv6 addressed must be enclosed in brackets).</param>
        /// <returns><see langword="true" /> if a connection has been set up or <see langword="false" /> if a connection attempt failed with every server on the list.</returns>
        bool Connect(string servers);

        /// <summary>
        /// Disconnect a connected API handle. Safe to call even if the handle is already disconnected.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends all the active messages in a managed message handle to the server. The message handle will not be reset, so this has to be performed manually by calling <see cref="Message.Reset()"/>.
        /// </summary>
        /// <param name="message">The managed message handle.</param>
        /// <returns><see langword="true" /> if there were no errors detected when sending the data, or <see langword="false" /> if an error was detected (such as not connected to any server). Due to the nature of TCP/IP, a successful return code does not guarantee that the server has received the messages.</returns>
        bool Send(IMessage message);
    }
}