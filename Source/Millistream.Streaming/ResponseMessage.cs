using System.Collections.Generic;

namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a message sent from the server as a response to a client request.
    /// </summary>
    public class ResponseMessage
    {
        private readonly Dictionary<Field, string> _fields = new Dictionary<Field, string>();

        internal ResponseMessage(uint instrumentReference, MessageReference messageReference, int messageClass)
        {
            InstrumentReference = instrumentReference;
            MessageReference = messageReference;
            MessageClass = messageClass;
        }

        #region Properties
        /// <summary>
        /// The unique id of the instrument. There are a limited number of messages in which this property will not be used to carry the instrument reference. Please consult the official documentation for more information.
        /// </summary>
        public uint InstrumentReference { get; }
        
        /// <summary>
        /// The message reference used to to determine the type of message.
        /// </summary>
        public MessageReference MessageReference { get; }
        
        /// <summary>
        /// Used internally and supplied only for completeness and transparency. The <see cref="MessageReference"/> property should be used to determine the type of the message.
        /// </summary>
        public int MessageClass { get; }
        
        /// <summary>
        /// All fields that were returned in the message. The values are represented by UTF-8 strings that me be null provided that the specific field supports null values.
        /// </summary>
        public IReadOnlyDictionary<Field, string> Fields => _fields;
        #endregion
        
        #region Methods
        internal void SetField(Field key, string value) => _fields[key] = value;
        #endregion
    }
}
