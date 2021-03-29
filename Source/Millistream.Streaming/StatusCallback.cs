using System;

namespace Millistream.Streaming
{
    /// <summary>
    /// Encapsulates a data status callback method that will be called whenever there is a change of the status of the connection.
    /// </summary>
    /// <typeparam name="T">The type of the custom user data that will be available to the status callback method.</typeparam>
    /// <param name="userData">The custom user data that will be available to the status callback method.</param>
    /// <param name="connectionStatus">The current status of the connection.</param>
    /// <param name="host">A memory span that contains the bytes of a string representation of the hostname of the server.</param>
    /// <param name="ip">A memory span that contains the bytes of a string representation of the IP address of the server.</param>
    public delegate void StatusCallback<in T>(T userData, ConnectionStatus connectionStatus, ReadOnlySpan<byte> host, ReadOnlySpan<byte> ip);
}