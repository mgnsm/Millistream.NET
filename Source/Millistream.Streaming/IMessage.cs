﻿using Millistream.Streaming.Interop;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a managed message handle that can contain several messages for efficiency.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Gets or sets the zlib compression level used for the <see cref="AddString(uint, string)"/> and <see cref="AddString(uint, string, int)"/> methods.
        /// </summary>
        [Obsolete("The CompressionLevel enumeration is deprecated and will be removed in a future version. The type of this property will then be changed to System.Byte.")]
        CompressionLevel CompressionLevel { get; set; }

        /// <summary>
        /// Gets the total number of messages in the message handle (the number of active + the number of reused messages currently not used for active messages).
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the number of active messages in the message handle.
        /// </summary>
        int ActiveCount { get; }

        /// <summary>
        /// Gets the number of added fields to the current message.
        /// </summary>
        int FieldCount { get; }

        /// <summary>
        /// Enables or disables the UTF-8 validation performed in <see cref="AddString(uint, string)"/> and <see cref="AddString(uint, string, int)"/>.
        /// </summary>
        bool Utf8Validation { get; set; }

        /// <summary>
        /// Gets or sets the intended delay of the message.
        /// </summary>
        public byte Delay { get; set; }

        /// <summary>
        /// Adds a new message to the message handle. If the current active message is empty it will be reused to carry this new message.
        /// </summary>
        /// <param name="insref">The reference for the instrument for which the message is created for.</param>
        /// <param name="mref">The type of the message to create.</param>
        /// <returns><see langword="true" /> if a new message was added to the message handle (or an empty message was reused) or <see langword="false" /> if there was an error.</returns>
        bool Add(ulong insref, int mref);

        /// <summary>
        /// Adds a new message to the message handle. If the current active message is empty it will be reused to carry this new message. Can be more convenient to use if the delay varies much between messages.
        /// </summary>
        /// <param name="insref">The reference for the instrument for which the message is created for.</param>
        /// <param name="mref">The type of the message to create.</param>
        /// <param name="delay">The intended delay of the message.</param>
        /// <returns><see langword="true" /> if a new message was added to the message handle (or an empty message was reused) or <see langword="false" /> if there was an error.</returns>
        bool Add(ulong insref, ushort mref, byte delay);

        /// <summary>
        /// Adds a new message to the message handle. If the current active message is empty it will be reused to carry this new message.
        /// </summary>
        /// <param name="instrumentReference">The reference for the instrument for which the message is created for.</param>
        /// <param name="messageReference">The type of the message to create.</param>
        /// <returns><see langword="true" /> if a new message was added to the message handle (or an empty message was reused) or <see langword="false" /> if there was an error.</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the Add(ulong insref, int mref) overload instead.")]
        bool Add(ulong instrumentReference, MessageReference messageReference);

        /// <summary>
        /// Adds a numeric field to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The numeric value as a UTF-8 string.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false"/> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddNumeric(uint tag, string value);

        /// <summary>
        /// Adds a numeric field to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The numeric value as a UTF-8 string.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false"/> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddNumeric(uint tag, string value) overload instead.")]
        bool AddNumeric(Field tag, string value);

        /// <summary>
        /// Adds a numeric field to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The numeric field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false"/> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddNumeric(uint tag, ReadOnlySpan<byte> value);
        /// <summary>
        /// Adds a numeric field to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The numeric field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false"/> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddNumeric(uint tag, ReadOnlySpan<byte> value) overload instead.")]
        bool AddNumeric(Field tag, ReadOnlySpan<byte> value);

        /// <summary>
        /// Adds a scaled and signed 64-bit integer field to the current active message. <paramref name="decimals"/> can be between 0 and 19. A value of 12345 with <paramref name="decimals"/> set to 2 will be encoded as "123.45".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled and signed 64-bit integer.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <returns><see langword = "true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddInt64(uint tag, long value, int decimals);

        /// <summary>
        /// Adds a scaled and signed 64-bit integer field to the current active message. <paramref name="decimals"/> can be between 0 and 19. A value of 12345 with <paramref name="decimals"/> set to 2 will be encoded as "123.45".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled and signed 64-bit integer.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <returns><see langword = "true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddInt64(uint tag, long value, int decimals) overload instead.")]
        bool AddInt64(Field tag, long value, int decimals);

        /// <summary>
        /// Adds a scaled and unsigned 64-bit integer field to the current active message. <paramref name="decimals"/> can be between 0 and 19. A value of 12345 with <paramref name="decimals"/> set to 2 will be encoded as "123.45".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled and unsigned 64-bit integer.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <returns><see langword = "true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddUInt64(uint tag, ulong value, int decimals);

        /// <summary>
        /// Adds a scaled and unsigned 64-bit integer field to the current active message. <paramref name="decimals"/> can be between 0 and 19. A value of 12345 with <paramref name="decimals"/> set to 2 will be encoded as "123.45".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled and unsigned 64-bit integer.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <returns><see langword = "true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddUInt64(uint tag, ulong value, int decimals) overload instead.")] 
        bool AddUInt64(Field tag, ulong value, int decimals);

        /// <summary>
        /// Adds a UTF-8 string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The UTF-8 string field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddString(uint tag, string value);

        /// <summary>
        /// Adds a UTF-8 string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The UTF-8 string field value.</param>
        /// <param name="length">The number of characters in <paramref name="value"/> to be added to the message.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddString(uint tag, string value, int length);

        /// <summary>
        /// Adds a UTF-8 string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The UTF-8 string field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddString(uint tag, string value) overload instead.")]
        bool AddString(Field tag, string value);

        /// <summary>
        /// Adds a UTF-8 string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The UTF-8 string field value.</param>
        /// <param name="length">The number of characters in <paramref name="value"/> to be added to the message.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddString(uint tag, string value, int length) overload instead.")]
        bool AddString(Field tag, string value, int length);

        /// <summary>
        /// Adds a UTF-8 encoded string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddString(uint tag, ReadOnlySpan<byte> value);

        /// <summary>
        /// Adds a UTF-8 encoded string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <param name="length">The number of bytes in <paramref name="value"/> to be added to the message.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddString(uint tag, ReadOnlySpan<byte> value, int length);

        /// <summary>
        /// Adds a UTF-8 encoded string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddString(uint tag, ReadOnlySpan<byte> value) overload instead.")]
        bool AddString(Field tag, ReadOnlySpan<byte> value);

        /// <summary>
        /// Adds a UTF-8 encoded string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <param name="length">The number of bytes in <paramref name="value"/> to be added to the message.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddString(uint tag, ReadOnlySpan<byte> value, int length) overload instead.")]
        bool AddString(Field tag, ReadOnlySpan<byte> value, int length);

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC. The format of value must be one of "YYYY-MM-DD", "YYYY-MM", "YYYY-H1", "YYYY-H2", "YYYY-T1", "YYYY-T2", "YYYY-T3", "YYYY-Q1", "YYYY-Q2", "YYYY-Q3", "YYYYQ4" or "YYYY-W[1-52]".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddDate(uint tag, string value);

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC. The format of value must be one of "YYYY-MM-DD", "YYYY-MM", "YYYY-H1", "YYYY-H2", "YYYY-T1", "YYYY-T2", "YYYY-T3", "YYYY-Q1", "YYYY-Q2", "YYYY-Q3", "YYYYQ4" or "YYYY-W[1-52]".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddDate(uint tag, string value) overload instead.")]
        bool AddDate(Field tag, string value);

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="year">The year of the date field value.</param>
        /// <param name="month">The month of the date field value.</param>
        /// <param name="day">The day of the date field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddDate(uint tag, int year, int month, int day);

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="year">The year of the date field value.</param>
        /// <param name="month">The month of the date field value.</param>
        /// <param name="day">The day of the date field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddDate(uint tag, int year, int month, int day) overload instead.")]
        bool AddDate(Field tag, int year, int month, int day);

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC. The format of value must be one of "YYYY-MM-DD", "YYYY-MM", "YYYY-H1", "YYYY-H2", "YYYY-T1", "YYYY-T2", "YYYY-T3", "YYYY-Q1", "YYYY-Q2", "YYYY-Q3", "YYYYQ4" or "YYYY-W[1-52]".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddDate(uint tag, ReadOnlySpan<byte> value);

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC. The format of value must be one of "YYYY-MM-DD", "YYYY-MM", "YYYY-H1", "YYYY-H2", "YYYY-T1", "YYYY-T2", "YYYY-T3", "YYYY-Q1", "YYYY-Q2", "YYYY-Q3", "YYYYQ4" or "YYYY-W[1-52]".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddDate(uint tag, ReadOnlySpan<byte> value) overload instead.")]
        bool AddDate(Field tag, ReadOnlySpan<byte> value);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The format of value must be "HH:MM:SS" or "HH:MM:SS.mmm" (where mmm is the milliseconds). libmdf 1.0.24 accepts up to nanoseconds resolution, i.e. "HH:MM:SS.nnnnnnnnn".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddTime(uint tag, string value);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The format of value must be "HH:MM:SS" or "HH:MM:SS.mmm" (where mmm is the milliseconds). libmdf 1.0.24 accepts up to nanoseconds resolution, i.e. "HH:MM:SS.nnnnnnnnn".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddTime(uint tag, string value) overload instead.")]
        bool AddTime(Field tag, string value);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. If <paramref name="millisecond"/> is set to 0 the timestamp is encoded as "HH:MM:SS".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddTime2(uint tag, int hour, int minute, int second, int millisecond);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. If <paramref name="millisecond"/> is set to 0 the timestamp is encoded as "HH:MM:SS".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddTime2(uint tag, int hour, int minute, int second, int millisecond) overload instead.")]
        bool AddTime2(Field tag, int hour, int minute, int second, int millisecond);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. If <paramref name="nanosecond"/> is 1 – 999 the timstamp is encoded as "HH:MM:SS.mmm". If <paramref name="nanosecond"/> is set to 0 the timestamp is encoded as "HH:MM:SS".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="nanosecond">The nanosecond.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddTime3(uint tag, int hour, int minute, int second, int nanosecond);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. If <paramref name="nanosecond"/> is 1 – 999 the timstamp is encoded as "HH:MM:SS.mmm". If <paramref name="nanosecond"/> is set to 0 the timestamp is encoded as "HH:MM:SS".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="nanosecond">The nanosecond.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddTime3(uint tag, int hour, int minute, int second, int nanosecond) overload instead.")]
        bool AddTime3(Field tag, int hour, int minute, int second, int nanosecond);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The format of value must be "HH:MM:SS" or "HH:MM:SS.mmm" (where mmm is the milliseconds). libmdf 1.0.24 accepts up to nanoseconds resolution, i.e. "HH:MM:SS.nnnnnnnnn".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddTime(uint tag, ReadOnlySpan<byte> value);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The format of value must be "HH:MM:SS" or "HH:MM:SS.mmm" (where mmm is the milliseconds). libmdf 1.0.24 accepts up to nanoseconds resolution, i.e. "HH:MM:SS.nnnnnnnnn".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddTime(uint tag, ReadOnlySpan<byte> value) overload instead.")]
        bool AddTime(Field tag, ReadOnlySpan<byte> value);

        /// <summary>
        /// Adds a list field to the current active message. A list field is a space separated list of instrument references. The first position in the value can be:
        /// <para>'+'   (the supplied list should be added to the current value)<br/>
        /// '-' (the supplied list should be removed from the current value)<br/>
        /// '=' (the supplied list is the current value)</para>
        /// If there is no such prefix it is interpreted as if it was prefixed with a '='.  There is a current soft limit of 1.000.000 instrument references per list.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The list field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddList(uint tag, string value);

        /// <summary>
        /// Adds a list field to the current active message. A list field is a space separated list of instrument references. The first position in the value can be:
        /// <para>'+'   (the supplied list should be added to the current value)<br/>
        /// '-' (the supplied list should be removed from the current value)<br/>
        /// '=' (the supplied list is the current value)</para>
        /// If there is no such prefix it is interpreted as if it was prefixed with a '='.  There is a current soft limit of 1.000.000 instrument references per list.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The list field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddList(uint tag, string value) overload instead.")]
        bool AddList(Field tag, string value);

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
        bool AddList(uint tag, ReadOnlySpan<byte> value);

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
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddList(uint tag, ReadOnlySpan<byte> value) overload instead.")]
        bool AddList(Field tag, ReadOnlySpan<byte> value);

        /// <summary>
        /// Resets the message handle (sets the number of active messages to zero) so it can be reused. The memory allocated for the current messages in the handle is retained for performance reasons and will be reused when you add new messages to the handle.
        /// </summary>
        void Reset();

        /// <summary>
        /// Removes the current active message from the message handle and all the fields that you have added for this message. Points the current message at the previous message in the message handle if it exists, so repeated calls will reset the whole message handle just like <see cref="Reset()"/> had been called.
        /// </summary>
        /// <returns><see langword="true" /> if there are more active messages in the message handle or <see langword="false" /> if the message handle is now empty.</returns>
        bool Delete();

        /// <summary>
        /// Serializes the message chain in the message handle and produces a base64 encoded string to the address pointed to by <paramref name="result"/>. It's the responsibility of the caller to free the produced unmanaged string.
        /// </summary>
        /// <param name="result">An unmanaged pointer to the base64 encoded string if the method returns <see langword="true" />, or <see cref="IntPtr.Zero"/> if the method returns <see langword="false" />.</param>
        /// <returns><see langword="true" /> if there existed a message chain and if it was successfully base64 encoded, or <see langword="false" /> if there existed no message chain or if the base64 encoding failed.</returns>
        bool Serialize(out IntPtr result);

        /// <summary>
        /// Deserializes a base64 encoded message chain and replaces the existing (if any) message chain in the message handle.
        /// </summary>
        /// <param name="data">A base64 encoded (serialized) message chain.</param>
        /// <returns><see langword="true" /> if the message chain was successfully deserialized, or <see langword="false" /> if the deserialization failed (if so the current message chain in the message handler is left untouched).</returns>
        bool Deserialize(string data);
    }
}