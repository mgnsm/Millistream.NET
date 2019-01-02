using System;
using System.Collections.Generic;

namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a message sent from the server as a response to a client request.
    /// </summary>
    public class ResponseMessage
    {
        #region Fields
        private readonly Dictionary<Field, ReadOnlyMemory<byte>> _fields = new Dictionary<Field, ReadOnlyMemory<byte>>();
        #endregion

        #region Constructors
        internal ResponseMessage() { }
        #endregion

        #region Properties
        /// <summary>
        /// All fields that were returned in the message.
        /// </summary>
        public IReadOnlyDictionary<Field, ReadOnlyMemory<byte>> Fields => _fields;

        /// <summary>
        /// The unique id of the instrument. There are a limited number of messages in which this property will not be used to carry the instrument reference. Please consult the official documentation for more information.
        /// </summary>
        public uint InstrumentReference { get; internal set; }

        /// <summary>
        /// Used internally and supplied only for completeness and transparency. The <see cref="MessageReference"/> property should be used to determine the type of the message.
        /// </summary>
        public int MessageClass { get; internal set; }

        /// <summary>
        /// The message reference used to to determine the type of message.
        /// </summary>
        public MessageReference MessageReference { get; internal set; }

        /// <summary>
        /// The data received from the server for all fields of the response message.
        /// </summary>
        internal ExtendableArray<byte> Data { get; } = new ExtendableArray<byte>();
        #endregion

        #region Methods
        internal void SetField(Field key, ReadOnlyMemory<byte> value) => _fields[key] = value;

        internal void ResetState()
        {
            Data.Clear();
            _fields.Clear();
        }
        #endregion
    }
}
