using System;

namespace Millistream.Streaming
{
    /// <summary>
    /// Provides data for the DataReceived event.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        internal DataReceivedEventArgs(ResponseMessage message) => Message = message;

        /// <summary>
        /// The received response message that contains data.
        /// </summary>
        public ResponseMessage Message { get; }
    }
}