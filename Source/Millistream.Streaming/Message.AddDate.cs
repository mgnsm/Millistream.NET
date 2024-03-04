using System;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class Message
    {
        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC. The format of value must be one of "YYYY-MM-DD", "YYYY-MM", "YYYY-H1", "YYYY-H2", "YYYY-T1", "YYYY-T2", "YYYY-T3", "YYYY-Q1", "YYYY-Q2", "YYYY-Q3", "YYYYQ4" or "YYYY-W[1-52]".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_date.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddDate(uint tag, string value) =>
            _nativeImplementation.mdf_message_add_date_str(_handle, tag, value) == 1;

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC. The format of value must be one of "YYYY-MM-DD", "YYYY-MM", "YYYY-H1", "YYYY-H2", "YYYY-T1", "YYYY-T2", "YYYY-T3", "YYYY-Q1", "YYYY-Q2", "YYYY-Q3", "YYYYQ4" or "YYYY-W[1-52]".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_date.</remarks>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddDate(uint tag, string value) overload instead.")]
        public bool AddDate(Field tag, string value) =>
            AddDate((uint)tag, value);

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="year">The year of the date field value.</param>
        /// <param name="month">The month of the date field value.</param>
        /// <param name="day">The day of the date field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_date2.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddDate(uint tag, int year, int month, int day) =>
            _nativeImplementation.mdf_message_add_date2 != default && _nativeImplementation.mdf_message_add_date2(_handle, tag, year, month, day) == 1;

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="year">The year of the date field value.</param>
        /// <param name="month">The month of the date field value.</param>
        /// <param name="day">The day of the date field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_date2.</remarks>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddDate(uint tag, int year, int month, int day) overload instead.")]
        public bool AddDate(Field tag, int year, int month, int day) =>
            AddDate((uint)tag, year, month, day);

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC. The format of value must be one of "YYYY-MM-DD", "YYYY-MM", "YYYY-H1", "YYYY-H2", "YYYY-T1", "YYYY-T2", "YYYY-T3", "YYYY-Q1", "YYYY-Q2", "YYYY-Q3", "YYYYQ4" or "YYYY-W[1-52]".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_date.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddDate(uint tag, ReadOnlySpan<byte> value)
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
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddDate(uint tag, ReadOnlySpan<byte> value) overload instead.")]
        public bool AddDate(Field tag, ReadOnlySpan<byte> value) =>
            AddDate((uint)tag, value);
    }
}