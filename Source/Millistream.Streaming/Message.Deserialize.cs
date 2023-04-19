using System;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class Message
    {
        /// <summary>
        /// Deserializes a base64 encoded message chain and replaces the existing (if any) message chain in the message handle.
        /// </summary>
        /// <param name="data">A base64 encoded (serialized) message chain.</param>
        /// <returns><see langword="true" /> if the message chain was successfully deserialized, or <see langword="false" /> if the deserialization failed (if so the current message chain in the message handler is left untouched).</returns>
        /// <remarks>The corresponding native function is mdf_message_deserialize.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool Deserialize(string data) => !string.IsNullOrEmpty(data)
            && _nativeImplementation.mdf_message_deserialize_str != default
            && _nativeImplementation.mdf_message_deserialize_str(_handle, data) == 1;

        /// <summary>
        /// Deserializes a base64 encoded message chain and replaces the existing (if any) message chain in the message handle.
        /// </summary>
        /// <param name="data">An unmanaged pointer to a base64 encoded (serialized) message chain.</param>
        /// <returns><see langword="true" /> if the message chain was successfully deserialized, or <see langword="false" /> if the deserialization failed (if so the current message chain in the message handler is left untouched).</returns>
        /// <remarks>The corresponding native function is mdf_message_deserialize.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool Deserialize(IntPtr data) => _nativeImplementation.mdf_message_deserialize != default
            && _nativeImplementation.mdf_message_deserialize(_handle, data) == 1;

        /// <summary>
        /// Deserializes a base64 encoded message chain and replaces the existing (if any) message chain in the message handle.
        /// </summary>
        /// <param name="data">A memory span that contains a base64 encoded (serialized) message chain.</param>
        /// <returns><see langword="true" /> if the message chain was successfully deserialized, or <see langword="false" /> if the deserialization failed (if so the current message chain in the message handler is left untouched).</returns>
        /// <remarks>The corresponding native function is mdf_message_deserialize.</remarks>
        public unsafe bool Deserialize(ReadOnlySpan<byte> data)
        {
            fixed (byte* bytes = data)
                return _nativeImplementation.mdf_message_deserialize != default
                    && _nativeImplementation.mdf_message_deserialize(Handle, (IntPtr)bytes) == 1;
        }
    }
}