using System;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public unsafe sealed partial class Message
    {
        /// <summary>
        /// Adds a numeric field to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The numeric field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false"/> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_numeric.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddNumeric(uint tag, ReadOnlySpan<byte> value)
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
        public bool AddNumeric(Field tag, ReadOnlySpan<byte> value) =>
            AddNumeric((uint)tag, value);

        /// <summary>
        /// Adds a UTF-8 encoded string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_string.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddString(uint tag, ReadOnlySpan<byte> value)
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
        /// <exception cref="InvalidOperationException">The installed version of the native library doesn't include the mdf_message_add_string2 function.</exception>
        /// <remarks>The corresponding native function is mdf_message_add_string2.</remarks>
        public bool AddString(uint tag, ReadOnlySpan<byte> value, int length)
        {
            if (value != null && length < 0)
                return false;
            ThrowIfNativeFunctionIsMissing(_nativeImplementation.mdf_message_add_string2, nameof(_nativeImplementation.mdf_message_add_string2));
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
        public bool AddString(Field tag, ReadOnlySpan<byte> value) =>
            AddString((uint)tag, value);

        /// <summary>
        /// Adds a UTF-8 encoded string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <param name="length">The number of bytes in <paramref name="value"/> to be added to the message.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="InvalidOperationException">The installed version of the native library doesn't include the mdf_message_add_string2 function.</exception>
        /// <remarks>The corresponding native function is mdf_message_add_string2.</remarks>
        public bool AddString(Field tag, ReadOnlySpan<byte> value, int length) =>
            AddString((uint)tag, value, length);

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC. The format of value must be one of "YYYY-MM-DD", "YYYY-MM", "YYYY-H1", "YYYY-H2", "YYYY-T1", "YYYY-T2", "YYYY-T3", "YYYY-Q1", "YYYY-Q2", "YYYY-Q3", "YYYYQ4" or "YYYY-W[1-52]".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_date.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddDate(uint tag, ReadOnlySpan<byte> value)
        {
            fixed (byte* bytes = value)
                return _nativeImplementation.mdf_message_add_date(Handle, tag, (IntPtr)bytes) == 1;
        }

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC. The format of value must be one of "YYYY-MM-DD", "YYYY-MM", "YYYY-H1", "YYYY-H2", "YYYY-T1", "YYYY-T2", "YYYY-T3", "YYYY-Q1", "YYYY-Q2", "YYYY-Q3", "YYYYQ4" or "YYYY-W[1-52]".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_date.</remarks>
        public bool AddDate(Field tag, ReadOnlySpan<byte> value) =>
            AddDate((uint)tag, value);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The format of value must be "HH:MM:SS" or "HH:MM:SS.mmm" (where mmm is the milliseconds). libmdf 1.0.24 accepts up to nanoseconds resolution, i.e. "HH:MM:SS.nnnnnnnnn".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_time.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddTime(uint tag, ReadOnlySpan<byte> value)
        {
            fixed (byte* bytes = value)
                return _nativeImplementation.mdf_message_add_time(Handle, tag, (IntPtr)bytes) == 1;
        }

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The format of value must be "HH:MM:SS" or "HH:MM:SS.mmm" (where mmm is the milliseconds). libmdf 1.0.24 accepts up to nanoseconds resolution, i.e. "HH:MM:SS.nnnnnnnnn".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_time.</remarks>
        public bool AddTime(Field tag, ReadOnlySpan<byte> value) =>
            AddTime((uint)tag, value);

        /// <summary>
        /// Adds a list field to the current active message. A list field is a space separated list of instrument references. The first position in the value can be:
        /// <para>'+'   (the supplied list should be added to the current value)<br/>
        /// '-' (the supplied list should be removed from the current value)<br/>
        /// '=' (the supplied list is the current value)</para>
        /// If there is no such prefix it is interpreted as if it was prefixed with a '='.  There is a current soft limit of 1.000.000 instrument references per list.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The list field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_list.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddList(uint tag, ReadOnlySpan<byte> value)
        {
            fixed (byte* bytes = value)
                return _nativeImplementation.mdf_message_add_list(Handle, tag, (IntPtr)bytes) == 1;
        }

        /// <summary>
        /// Adds a list field to the current active message. A list field is a space separated list of instrument references. The first position in the value can be:
        /// <para>'+'   (the supplied list should be added to the current value)<br/>
        /// '-' (the supplied list should be removed from the current value)<br/>
        /// '=' (the supplied list is the current value)</para>
        /// If there is no such prefix it is interpreted as if it was prefixed with a '='.  There is a current soft limit of 1.000.000 instrument references per list.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The list field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_list.</remarks>
        public bool AddList(Field tag, ReadOnlySpan<byte> value) =>
            AddList((uint)tag, value);


        /// <summary>
        /// Deserializes a base64 encoded message chain and replaces the existing (if any) message chain in the message handle.
        /// </summary>
        /// <param name="data">A memory span that contains a base64 encoded (serialized) message chain.</param>
        /// <returns><see langword="true" /> if the message chain was successfully deserialized, or <see langword="false" /> if the deserialization failed (if so the current message chain in the message handler is left untouched).</returns>
        /// <exception cref="InvalidOperationException">The installed version of the native library doesn't include the mdf_message_deserialize function.</exception>
        /// <remarks>The corresponding native function is mdf_message_deserialize.</remarks>
        public bool Deserialize(ReadOnlySpan<byte> data)
        {
            ThrowIfNativeFunctionIsMissing(_nativeImplementation.mdf_message_deserialize, nameof(_nativeImplementation.mdf_message_deserialize));
            fixed (byte* bytes = data)
                return _nativeImplementation.mdf_message_deserialize(Handle, (IntPtr)bytes) == 1;
        }
    }
}