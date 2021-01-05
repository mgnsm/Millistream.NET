﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a managed message handle that can contain several messages for efficiency.
    /// </summary>
    /// <remarks>Handles are not thread-safe. If multiple threads will share access to a single handle, the accesses has to be serialized using a mutex or other forms of locking mechanisms. The API as such is thread-safe so multiple threads can have local handles without the need for locks.</remarks>
    public sealed class Message : IMessage, IDisposable
    {
        private const string ListSeparator = " ";
        private readonly INativeImplementation _nativeImplementation;
        private CompressionLevel _compressionLevel = CompressionLevel.Z_BEST_SPEED;
        private bool _isDisposed;

        /// <summary>
        /// Creates an instance of the <see cref="Message"/> class.
        /// </summary>
        /// <remarks>The corresponding native method is mdf_message_create.</remarks>
        public Message() : this(NativeImplementation.Get()) { }

        internal Message(INativeImplementation nativeImplementation)
        {
            _nativeImplementation = nativeImplementation ?? throw new ArgumentNullException(nameof(nativeImplementation));
            Handle = _nativeImplementation.mdf_message_create();
        }

        ~Message() => Dispose();

        /// <summary>
        /// The zlib compression level used for the <see cref="AddString(uint, string)"/> method.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        public CompressionLevel CompressionLevel
        {
            get
            {
                ThrowIfDisposed();
                return _compressionLevel;
            }
            set
            {
                ThrowIfDisposed();
                if (_nativeImplementation.mdf_message_set_compression_level(Handle, (int)value) == 1)
                    _compressionLevel = value;
            }
        }

        /// <summary>
        /// The total number of messages in the message handle (the number of active + the number of reused messages currently not used for active messages).
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        public int Count
        {
            get
            {
                ThrowIfDisposed();
                return _nativeImplementation.mdf_message_get_num(Handle);
            }
        }

        /// <summary>
        /// The number of active messages in the message handle.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        public int ActiveCount
        {
            get
            {
                ThrowIfDisposed();
                return _nativeImplementation.mdf_message_get_num_active(Handle);
            }
        }

        internal IntPtr Handle { get; }

        /// <summary>
        /// Adds a new message to the message handle. If the current active message is empty it will be reused to carry this new message.
        /// </summary>
        /// <param name="insref">The reference for the instrument for which the message is created for.</param>
        /// <param name="mref">The type of the message to create.</param>
        /// <returns><see langword="true" /> if a new message was added to the message handle (or an empty message was reused) or <see langword="false" /> if there was an error.</returns>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add.</remarks>
        public bool Add(ulong insref, int mref)
        {
            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add(Handle, insref, mref) == 1;
        }

        /// <summary>
        /// Adds a new message to the message handle. If the current active message is empty it will be reused to carry this new message.
        /// </summary>
        /// <param name="instrumentReference">The reference for the instrument for which the message is created for.</param>
        /// <param name="messageReference">The type of the message to create.</param>
        /// <returns><see langword="true" /> if a new message was added to the message handle (or an empty message was reused) or <see langword="false" /> if there was an error.</returns>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add.</remarks>
        public bool Add(ulong instrumentReference, MessageReference messageReference) =>
            Add(instrumentReference, (int)messageReference);

        /// <summary>
        /// Adds a numeric field to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The numeric value as a UTF-8 string.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false"/> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_numeric.</remarks>
        public bool AddNumeric(uint tag, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_numeric(Handle, tag, value) == 1;
        }

        /// <summary>
        /// Adds a numeric field to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The numeric value as a UTF-8 string.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false"/> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_numeric.</remarks>
        public bool AddNumeric(Field tag, string value) =>
            AddNumeric((uint)tag, value);

        /// <summary>
        /// Adds a scaled and signed 64-bit integer field to the current active message. <paramref name="decimals"/> can be between 0 and 19. A value of 12345 with <paramref name="decimals"/> set to 2 will be encoded as "123.45".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled and signed 64-bit integer.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <returns><see langword = "true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentException"><paramref name="decimals"/> is not between 0 and 19.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_int.</remarks>
        public bool AddInt64(uint tag, long value, int decimals)
        {
            if (decimals < 0 || decimals > 19)
                throw new ArgumentException($"{nameof(decimals)} cannot be smaller than 0 or greater than 19.", nameof(decimals));
            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_int(Handle, tag, value, decimals) == 1;
        }

        /// <summary>
        /// Adds a scaled and signed 64-bit integer field to the current active message. <paramref name="decimals"/> can be between 0 and 19. A value of 12345 with <paramref name="decimals"/> set to 2 will be encoded as "123.45".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled and signed 64-bit integer.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <returns><see langword = "true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentException"><paramref name="decimals"/> is not between 0 and 19.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_int.</remarks>
        public bool AddInt64(Field tag, long value, int decimals) =>
            AddInt64((uint)tag, value, decimals);

        /// <summary>
        /// Adds a scaled and unsigned 64-bit integer field to the current active message. <paramref name="decimals"/> can be between 0 and 19. A value of 12345 with <paramref name="decimals"/> set to 2 will be encoded as "123.45".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled and unsigned 64-bit integer.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <returns><see langword = "true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentException"><paramref name="decimals"/> is not between 0 and 19.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_uint.</remarks>
        public bool AddUInt64(uint tag, ulong value, int decimals)
        {
            if (decimals < 0 || decimals > 19)
                throw new ArgumentException($"{nameof(decimals)} cannot be smaller than 0 or greater than 19.", nameof(decimals));
            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_uint(Handle, tag, value, decimals) == 1;
        }

        /// <summary>
        /// Adds a scaled and unsigned 64-bit integer field to the current active message. <paramref name="decimals"/> can be between 0 and 19. A value of 12345 with <paramref name="decimals"/> set to 2 will be encoded as "123.45".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled and unsigned 64-bit integer.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <returns><see langword = "true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentException"><paramref name="decimals"/> is not between 0 and 19.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_uint.</remarks>
        public bool AddUInt64(Field tag, ulong value, int decimals) =>
            AddUInt64((uint)tag, value, decimals);

        /// <summary>
        /// Adds a UTF-8 string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The UTF-8 string field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_string.</remarks>
        public bool AddString(uint tag, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_string(Handle, (uint)tag, value) == 1;
        }

        /// <summary>
        /// Adds a UTF-8 string field to the current active message. The string is compressed with zlib using the compression level as set by <see cref="CompressionLevel" /> which is <see cref="CompressionLevel.Z_BEST_SPEED"/> by default.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The UTF-8 string field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_string.</remarks>
        public bool AddString(Field tag, string value) =>
            AddString((uint)tag, value);

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC. The format of value must be one of "YYYY-MM-DD", "YYYY-MM", "YYYY-H1", "YYYY-H2", "YYYY-T1", "YYYY-T2", "YYYY-T3", "YYYY-Q1", "YYYY-Q2", "YYYY-Q3", "YYYYQ4" or "YYYY-W[1-52]".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_date.</remarks>
        public bool AddDate(uint tag, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_date(Handle, tag, value) == 1;
        }

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC. The format of value must be one of "YYYY-MM-DD", "YYYY-MM", "YYYY-H1", "YYYY-H2", "YYYY-T1", "YYYY-T2", "YYYY-T3", "YYYY-Q1", "YYYY-Q2", "YYYY-Q3", "YYYYQ4" or "YYYY-W[1-52]".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_date.</remarks>
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
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_date2.</remarks>
        public bool AddDate(uint tag, int year, int month, int day)
        {
            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_date2(Handle, tag, year, month, day) == 1;
        }

        /// <summary>
        /// Adds a date field to the current active message. Please note that all dates and times are expressed in UTC.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="year">The year of the date field value.</param>
        /// <param name="month">The month of the date field value.</param>
        /// <param name="day">The day of the date field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_date2.</remarks>
        public bool AddDate(Field tag, int year, int month, int day) =>
            AddDate((uint)tag, year, month, day);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The format of value must be "HH:MM:SS" or "HH:MM:SS.mmm" (where mmm is the milliseconds).
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_time.</remarks>
        public bool AddTime(uint tag, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_time(Handle, tag, value) == 1;
        }

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The format of value must be "HH:MM:SS" or "HH:MM:SS.mmm" (where mmm is the milliseconds).
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_time.</remarks>
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
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_time2.</remarks>
        public bool AddTime2(uint tag, int hour, int minute, int second, int millisecond)
        {
            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_time2(Handle, tag, hour, minute, second, millisecond) == 1;
        }

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. If <paramref name="millisecond"/> is set to 0 the timestamp is encoded as "HH:MM:SS".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_time2.</remarks>
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
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_time3.</remarks>
        public bool AddTime3(uint tag, int hour, int minute, int second, int nanosecond)
        {
            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_time3(Handle, tag, hour, minute, second, nanosecond) == 1;
        }

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. If <paramref name="nanosecond"/> is 1 – 999 the timstamp is encoded as "HH:MM:SS.mmm". If <paramref name="nanosecond"/> is set to 0 the timestamp is encoded as "HH:MM:SS".
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="nanosecond">The nanosecond.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_time3.</remarks>
        public bool AddTime3(Field tag, int hour, int minute, int second, int nanosecond) =>
            AddTime3((uint)tag, hour, minute, second, nanosecond);

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
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_list.</remarks>
        public bool AddList(uint tag, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));

            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_list(Handle, tag, value) == 1;
        }

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
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_list.</remarks>
        public bool AddList(Field tag, string value) =>
            AddList((uint)tag, value);

        /// <summary>
        /// Adds a list field of instrument references to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="instrumentReferences">The list of instrument references.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instrumentReferences"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="instrumentReferences"/> contains more than 1.000.000 elements.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_list.</remarks>
        public bool AddList(uint tag, IEnumerable<ulong> instrumentReferences)
        {
            if (instrumentReferences == null)
                throw new ArgumentNullException(nameof(instrumentReferences));

            if (instrumentReferences.Count() > 1_000_000)
                throw new ArgumentException("There is a current soft limit of 1,000,000 instrument references per list.", nameof(instrumentReferences));

            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_list(Handle, tag, string.Join(ListSeparator, instrumentReferences)) == 1;
        }

        /// <summary>
        /// Adds a list field of instrument references to the current active message.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="instrumentReferences">The list of instrument references.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="instrumentReferences"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException"><paramref name="instrumentReferences"/> contains more than 1.000.000 elements.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_list.</remarks>
        public bool AddList(Field tag, IEnumerable<ulong> instrumentReferences) =>
            AddList((uint)tag, instrumentReferences);

        /// <summary>
        /// Adds a list of request classes to the <see cref="Field.MDF_F_REQUESTCLASS"/> field of the current active message.
        /// </summary>
        /// <param name="requestClasses">The list of request classes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="requestClasses"/> is <see langword="null" />.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_add_list.</remarks>
        public bool AddList(IEnumerable<RequestClass> requestClasses)
        {
            if (requestClasses == null)
                throw new ArgumentNullException(nameof(requestClasses));

            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_add_list(Handle, (uint)Field.MDF_F_REQUESTCLASS, string.Join(ListSeparator, requestClasses.Select(x => ((uint)x).ToString()))) == 1;
        }

        /// <summary>
        /// Resets the message handle (sets the number of active messages to zero) so it can be reused. The memory allocated for the current messages in the handle is retained for performance reasons and will be reused when you add new messages to the handle.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_reset.</remarks>
        public void Reset()
        {
            ThrowIfDisposed();
            _nativeImplementation.mdf_message_reset(Handle);
        }

        /// <summary>
        /// Removes the current active message from the message handle and all the fields that you have added for this message. Points the current message at the previous message in the message handle if it exists, so repeated calls will reset the whole message handle just like <see cref="Reset()"/> had been called.
        /// </summary>
        /// <returns><see langword="true" /> if there are more active messages in the message handle or <see langword="false" /> if the message handle is now empty.</returns>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_del.</remarks>
        public bool Delete()
        {
            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_del(Handle) == 1;
        }

        /// <summary>
        /// Serializes the message chain in the message handle and produces a base64 encoded string to the address pointed to by <paramref name="result"/>. It's the responsibility of the caller to free the produced unmanaged string.
        /// </summary>
        /// <param name="result">An unmanaged pointer to the base64 encoded string if the method returns <see langword="true" />, or <see cref="IntPtr.Zero"/> if the method returns <see langword="false" />.</param>
        /// <returns><see langword="true" /> if there existed a message chain and if it was successfully base64 encoded, or <see langword="false" /> if there existed no message chain or if the base64 encoding failed.</returns>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_serialize.</remarks>
        public bool Serialize(out IntPtr result)
        {
            ThrowIfDisposed();
            result = IntPtr.Zero;
            return _nativeImplementation.mdf_message_serialize(Handle, ref result) == 1;
        }

        /// <summary>
        /// Deserializes a base64 encoded message chain and replaces the existing (if any) message chain in the message handle.
        /// </summary>
        /// <param name="data">A base64 encoded (serialized) message chain.</param>
        /// <returns><see langword="true" /> if the message chain was successfully deserialized, or <see langword="false" /> if the deserialization failed (if so the current message chain in the message handler is left untouched).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="Message"/> instance has been disposed.</exception>
        /// <remarks>The corresponding native method is mdf_message_deserialize.</remarks>
        public bool Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            ThrowIfDisposed();
            return _nativeImplementation.mdf_message_deserialize(Handle, data) == 1;
        }

        /// <summary>
        /// Destroys the message handle and frees all allocated memory.
        /// </summary>
        /// <remarks>The corresponding native method is mdf_message_destroy.</remarks>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _nativeImplementation?.mdf_message_destroy(Handle);
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(typeof(Message).FullName);
        }
    }
}