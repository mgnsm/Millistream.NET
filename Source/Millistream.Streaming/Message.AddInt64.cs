using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class Message
    {
        /// <summary>
        /// Adds a scaled and signed 64-bit integer field to the current active message. <paramref name="decimals"/> can be between 0 and 19. A value of 12345 with <paramref name="decimals"/> set to 2 will be encoded as "123.45".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled and signed 64-bit integer.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <returns><see langword = "true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_int.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddInt64(uint tag, long value, int decimals) =>
            _nativeImplementation.mdf_message_add_int != default && _nativeImplementation.mdf_message_add_int(_handle, tag, value, decimals) == 1;

        /// <summary>
        /// Adds a scaled and signed 64-bit integer field to the current active message. <paramref name="decimals"/> can be between 0 and 19. A value of 12345 with <paramref name="decimals"/> set to 2 will be encoded as "123.45".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled and signed 64-bit integer.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <returns><see langword = "true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_int.</remarks>
        public bool AddInt64(Field tag, long value, int decimals) =>
            AddInt64((uint)tag, value, decimals);
    }
}