using System;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class Message
    {
        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The format of value must be "HH:MM:SS" or "HH:MM:SS.mmm" (where mmm is the milliseconds). libmdf 1.0.24 accepts up to nanoseconds resolution, i.e. "HH:MM:SS.nnnnnnnnn".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_time.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddTime(uint tag, string value) =>
            _nativeImplementation.mdf_message_add_time_str(_handle, tag, value) == 1;

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The format of value must be "HH:MM:SS" or "HH:MM:SS.mmm" (where mmm is the milliseconds). libmdf 1.0.24 accepts up to nanoseconds resolution, i.e. "HH:MM:SS.nnnnnnnnn".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_time.</remarks>
        public bool AddTime(Field tag, string value) =>
            AddTime((uint)tag, value);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. If <paramref name="millisecond"/> is set to 0 the timestamp is encoded as "HH:MM:SS".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_time2.</remarks>
        public unsafe bool AddTime2(uint tag, int hour, int minute, int second, int millisecond) =>
            _nativeImplementation.mdf_message_add_time2 != default
                && _nativeImplementation.mdf_message_add_time2(_handle, tag, hour, minute, second, millisecond) == 1;

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. If <paramref name="millisecond"/> is set to 0 the timestamp is encoded as "HH:MM:SS".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_time2.</remarks>
        public bool AddTime2(Field tag, int hour, int minute, int second, int millisecond) =>
            AddTime2((uint)tag, hour, minute, second, millisecond);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. If <paramref name="nanosecond"/> is 1 – 999 the timstamp is encoded as "HH:MM:SS.mmm". If <paramref name="nanosecond"/> is set to 0 the timestamp is encoded as "HH:MM:SS".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="nanosecond">The nanosecond.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_time3.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddTime3(uint tag, int hour, int minute, int second, int nanosecond) =>
            _nativeImplementation.mdf_message_add_time3 != default
                && _nativeImplementation.mdf_message_add_time3(_handle, tag, hour, minute, second, nanosecond) == 1;

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. If <paramref name="nanosecond"/> is 1 – 999 the timstamp is encoded as "HH:MM:SS.mmm". If <paramref name="nanosecond"/> is set to 0 the timestamp is encoded as "HH:MM:SS".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="nanosecond">The nanosecond.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_time3.</remarks>
        public bool AddTime3(Field tag, int hour, int minute, int second, int nanosecond) =>
            AddTime3((uint)tag, hour, minute, second, nanosecond);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The format of value must be "HH:MM:SS" or "HH:MM:SS.mmm" (where mmm is the milliseconds). libmdf 1.0.24 accepts up to nanoseconds resolution, i.e. "HH:MM:SS.nnnnnnnnn".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_time.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddTime(uint tag, ReadOnlySpan<byte> value)
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
    }
}