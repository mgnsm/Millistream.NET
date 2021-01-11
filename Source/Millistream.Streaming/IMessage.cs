using System;
using System.Collections.Generic;

namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a managed message handle (mdf_message_t) that can contain several messages for efficiency.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// The zlib compression level used for the <see cref="AddString(uint, string)"/> and <see cref="AddString(uint, string, int)"/> methods.
        /// </summary>
        CompressionLevel CompressionLevel { get; set; }

        /// <summary>
        /// The total number of messages in the message handle (the number of active + the number of reused messages currently not used for active messages).
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The number of active messages in the message handle.
        /// </summary>
        int ActiveCount { get; }

        /// <summary>
        /// Enables or disables the UTF-8 validation performed in <see cref="AddString(uint, string)"/> and <see cref="AddString(uint, string, int)"/>. It's enabled by default.
        /// </summary>
        bool Utf8Validation { get; set; }

        /// <summary>
        /// Adds a new message to the message handle. If the current active message is empty it will be reused to carry this new message.
        /// </summary>
        /// <param name="insref">The reference for the instrument for which the message is created for.</param>
        /// <param name="mref">The type of the message to create.</param>
        /// <returns><see langword="true" /> if a new message was added to the message handle (or an empty message was reused) or <see langword="false" /> if there was an error.</returns>
        bool Add(ulong insref, int mref);

        /// <summary>
        /// Adds a new message to the message handle. If the current active message is empty it will be reused to carry this new message.
        /// </summary>
        /// <param name="instrumentReference">The reference for the instrument for which the message is created for.</param>
        /// <param name="messageReference">The type of the message to create.</param>
        /// <returns><see langword="true" /> if a new message was added to the message handle (or an empty message was reused) or <see langword="false" /> if there was an error.</returns>
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
        bool AddNumeric(Field tag, string value);

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
        bool AddString(Field tag, string value);

        /// <summary>
        /// Adds a UTF-8 string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The UTF-8 string field value.</param>
        /// <param name="length">The number of characters in <paramref name="value"/> to be added to the message.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddString(Field tag, string value, int length);

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
        bool AddDate(Field tag, int year, int month, int day);

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
        bool AddTime3(Field tag, int hour, int minute, int second, int nanosecond);

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
        bool AddList(Field tag, string value);

        /// <summary>
        /// Adds a list field of instrument references to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="instrumentReferences">The list of instrument references.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddList(uint tag, IEnumerable<ulong> instrumentReferences);

        /// <summary>
        /// Adds a list field of instrument references to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="instrumentReferences">The list of instrument references.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddList(Field tag, IEnumerable<ulong> instrumentReferences);

        /// <summary>
        /// Adds a list of request classes to the <see cref="Field.MDF_F_REQUESTCLASS"/> field of the current active message.
        /// </summary>
        /// <param name="requestClasses">The list of request classes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        bool AddList(IEnumerable<RequestClass> requestClasses);

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