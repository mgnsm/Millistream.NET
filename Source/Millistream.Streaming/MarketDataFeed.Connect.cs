using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class MarketDataFeed<TCallbackUserData, TStatusCallbackUserData>
    {
        /// <summary>
        /// <para>Connects to the first server in servers, which can be a comma separated list of 'host:port' pairs, where 'host' can be a DNS host name or an ip address(IPv6 addressed must be enclosed in brackets). If the server does not respond in time (<see cref="ConnectionTimeout"/>), the next server in the list will be tried until the list is empty and the function finally fails.</para>
        /// <para>Upon connect, the API will verify the authenticity of the server using it's public RSA key, and a secure channel will be set up between the client and the server before the function signals success.</para>
        /// <para>If this is the first successful connect on the API handle, or the templates has been updated since the last time the API was connected, the server will send a <see cref="MessageReference.MDF_M_MESSAGESREFERENCE"/> message to the client containing the new message templates. So you could receive one message before a successful logon request.</para>
        /// </summary>
        /// <param name="servers">A comma separated list of 'host:port' pairs, where 'host' can be a DNS host name or an ip address (IPv6 addressed must be enclosed in brackets).</param>
        /// <returns><see langword="true" /> if a connection has been set up or <see langword="false" /> if a connection attempt failed with every server on the list.</returns>
        /// <remarks>The corresponding native function is mdf_connect.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool Connect(string servers) => !string.IsNullOrEmpty(servers)
            && _nativeImplementation.mdf_connect(_feedHandle, servers) == 1;
    }
}