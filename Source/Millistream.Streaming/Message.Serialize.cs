using System;

namespace Millistream.Streaming
{
    public partial class Message
    {
        /// <summary>
        /// Serializes the message chain in the message handle and produces a base64 encoded string to the address pointed to by <paramref name="result"/>. It's the responsibility of the caller to free the produced unmanaged string.
        /// </summary>
        /// <param name="result">An unmanaged pointer to the base64 encoded string if the method returns <see langword="true" />, or <see cref="IntPtr.Zero"/> if the method returns <see langword="false" />.</param>
        /// <returns><see langword="true" /> if there existed a message chain and if it was successfully base64 encoded, or <see langword="false" /> if there existed no message chain or if the base64 encoding failed.</returns>
        /// <remarks>The corresponding native function is mdf_message_serialize.</remarks>
        public unsafe bool Serialize(out IntPtr result)
        {
            result = IntPtr.Zero;
            return _nativeImplementation.mdf_message_serialize != default
                && _nativeImplementation.mdf_message_serialize(_handle, ref result) == 1;
        }
    }
}