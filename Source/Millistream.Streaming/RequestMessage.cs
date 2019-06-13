namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a request message to be sent to the server.
    /// </summary>
    public abstract class RequestMessage
    {
        /// <summary>
        /// Creates an instance of the <see cref="RequestMessage"/> class
        /// </summary>
        /// <param name="messageReference">The type of the request message.</param>
        internal RequestMessage(MessageReference messageReference) => MessageReference = messageReference;
        
        /// <summary>
        /// The type of the request message.
        /// </summary>
        public MessageReference MessageReference { get; }

        internal abstract void AddFields(IMessage message);
    }
}
