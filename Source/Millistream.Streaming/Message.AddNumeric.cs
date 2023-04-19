using System;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class Message
    {
        /// <summary>
        /// Adds a numeric field to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The numeric field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false"/> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_numeric.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddNumeric(uint tag, string value) =>
            _nativeImplementation.mdf_message_add_numeric_str(_handle, tag, value) == 1;

        /// <summary>
        /// Adds a numeric field to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The numeric field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false"/> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_numeric.</remarks>
        public bool AddNumeric(Field tag, string value) =>
            AddNumeric((uint)tag, value);

        /// <summary>
        /// Adds a numeric field to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The numeric field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false"/> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_numeric.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddNumeric(uint tag, ReadOnlySpan<byte> value)
        {
            fixed (byte* bytes = value)
                return _nativeImplementation.mdf_message_add_numeric(Handle, tag, (IntPtr)bytes) == 1;
        }

        /// <summary>
        /// Adds a numeric field to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The numeric field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false"/> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_numeric.</remarks>
        public unsafe bool AddNumeric(Field tag, ReadOnlySpan<byte> value) =>
            AddNumeric((uint)tag, value);
    }
}