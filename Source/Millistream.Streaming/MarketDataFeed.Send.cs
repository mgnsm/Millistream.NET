using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class MarketDataFeed<TCallbackUserData, TStatusCallbackUserData>
    {
        /// <summary>
        /// Sends all the active messages in a managed message handle to the server. The message handle will not be reset, so this has to be performed manually by calling <see cref="Message.Reset()"/>.
        /// </summary>
        /// <param name="message">The managed message handle.</param>
        /// <returns><see langword="true" /> if there were no errors detected when sending the data, or <see langword="false" /> if an error was detected (such as not connected to any server). Due to the nature of TCP/IP, a successful return code does not guarantee that the server has received the messages.</returns>
        /// <remarks>The corresponding native function is mdf_message_send.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool Send(Message message) =>
            message != null && _nativeImplementation.mdf_message_send(_feedHandle, message.Handle) == 1;

        /// <summary>
        /// Calls <see cref="Send(Message)"/> to send all the active messages in a managed message handle to the server if <paramref name="message"/> is a <see cref="Message"/>. For any other implementation of <see cref="IMessage" />, the method always returns <see langword="false" />.
        /// </summary>
        /// <param name="message">An implementation of the managed message handle.</param>
        /// <returns><see langword="true" /> if <paramref name="message"/> is a <see cref="Message"/> and there were no errors detected when sending the data, or <see langword="false" /> if an error was detected or if <paramref name="message"/> is not a <see cref="Message"/>.</returns>
        bool IMarketDataFeed<TCallbackUserData, TStatusCallbackUserData>.Send(IMessage message) =>
            message is Message messageWithHandle && Send(messageWithHandle);
    }
}