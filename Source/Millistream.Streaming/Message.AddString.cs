using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Millistream.Streaming
{
    public partial class Message
    {
        /// <summary>
        /// Adds a UTF-8 string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The UTF-8 string field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_string.</remarks>
        public unsafe bool AddString(uint tag, string value)
        {
            if (value == null)
                return _nativeImplementation.mdf_message_add_string(_handle, tag, IntPtr.Zero) == 1;

            int length = Encoding.UTF8.GetMaxByteCount(value.Length);
            byte[] bytes = ArrayPool<byte>.Shared.Rent(length + 1);
            try
            {
                fixed (char* c = value)
                fixed (byte* b = bytes)
                {
                    int bytesWritten = Encoding.UTF8.GetBytes(c, value.Length, b, length);
                    b[bytesWritten] = 0;
                    return _nativeImplementation.mdf_message_add_string(_handle, tag, (IntPtr)b) == 1;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        /// <summary>
        /// Adds a UTF-8 string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The UTF-8 string field value.</param>
        /// <param name="length">The number of characters in <paramref name="value"/> to be added to the message.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_string2.</remarks>
        public unsafe bool AddString(uint tag, string value, int length)
        {
            if (_nativeImplementation.mdf_message_add_string2 == default)
                return false;

            if (value == null)
                return _nativeImplementation.mdf_message_add_string2(_handle, tag, IntPtr.Zero, length) == 1;

            if (length < 0)
                return false;

            fixed (char* c = value)
            {
                int byteCount = Encoding.UTF8.GetByteCount(c, length);
                byte[] bytes = ArrayPool<byte>.Shared.Rent(byteCount + 1);
                try
                {
                    fixed (byte* b = bytes)
                    {
                        _ = Encoding.UTF8.GetBytes(c, length, b, byteCount);
                        return _nativeImplementation.mdf_message_add_string2(_handle, tag, (IntPtr)b, byteCount) == 1;
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(bytes);
                }
            }
        }

        /// <summary>
        /// Adds a UTF-8 string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The UTF-8 string field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_string.</remarks>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddString(uint tag, string value) overload instead.")]
        public bool AddString(Field tag, string value) =>
            AddString((uint)tag, value);

        /// <summary>
        /// Adds a UTF-8 string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The UTF-8 string field value.</param>
        /// <param name="length">The number of characters in <paramref name="value"/> to be added to the message.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_string2.</remarks>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddString(uint tag, string value, int length) overload instead.")]
        public bool AddString(Field tag, string value, int length) =>
            AddString((uint)tag, value, length);

        /// <summary>
        /// Adds a UTF-8 encoded string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_string.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddString(uint tag, ReadOnlySpan<byte> value)
        {
            fixed (byte* bytes = value)
                return _nativeImplementation.mdf_message_add_string(Handle, tag, (IntPtr)bytes) == 1;
        }

        /// <summary>
        /// Adds a UTF-8 encoded string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <param name="length">The number of bytes in <paramref name="value"/> to be added to the message.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_string2.</remarks>
        public unsafe bool AddString(uint tag, ReadOnlySpan<byte> value, int length)
        {
            if (_nativeImplementation.mdf_message_add_string2 == default || value != null && length < 0)
                return false;

            fixed (byte* bytes = value)
                return _nativeImplementation.mdf_message_add_string2(Handle, tag, (IntPtr)bytes, length) == 1;
        }

        /// <summary>
        /// Adds a UTF-8 encoded string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_string.</remarks>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddString(uint tag, ReadOnlySpan<byte> value) overload instead.")]
        public bool AddString(Field tag, ReadOnlySpan<byte> value) =>
            AddString((uint)tag, value);

        /// <summary>
        /// Adds a UTF-8 encoded string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <param name="length">The number of bytes in <paramref name="value"/> to be added to the message.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_string2.</remarks>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddString(uint tag, ReadOnlySpan<byte> value, int length) overload instead.")]
        public bool AddString(Field tag, ReadOnlySpan<byte> value, int length) =>
            AddString((uint)tag, value, length);
    }
}