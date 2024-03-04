using System;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class Message
    {
        /// <summary>
        /// Adds a new message to the message handle. If the current active message is empty it will be reused to carry this new message.
        /// </summary>
        /// <param name="insref">The reference for the instrument for which the message is created for.</param>
        /// <param name="mref">The type of the message to create.</param>
        /// <returns><see langword="true" /> if a new message was added to the message handle (or an empty message was reused) or <see langword="false" /> if there was an error.</returns>
        /// <remarks>The corresponding native function is mdf_message_add.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool Add(ulong insref, int mref) =>
            _nativeImplementation.mdf_message_add(_handle, insref, mref) == 1;

        /// <summary>
        /// Adds a new message to the message handle. If the current active message is empty it will be reused to carry this new message.
        /// </summary>
        /// <param name="insref">The reference for the instrument for which the message is created for.</param>
        /// <param name="mref">The type of the message to create.</param>
        /// <returns><see langword="true" /> if a new message was added to the message handle (or an empty message was reused) or <see langword="false" /> if there was an error.</returns>
        /// <remarks>The corresponding native function is mdf_message_add.</remarks>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the Add(ulong insref, int mref) overload instead.")]
        public bool Add(ulong insref, MessageReference mref) =>
            Add(insref, (int)mref);
    }
}