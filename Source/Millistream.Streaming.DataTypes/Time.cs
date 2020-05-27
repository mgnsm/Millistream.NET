using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a Universal Time Coordinated (UTC) time in one of the following formats: HH:MM:SS, HH:MM:SS.mmm or HH:MM:SS.nnnnnnnnn. 
    /// </summary>
    public readonly struct Time : IComparable, IComparable<Time>, IEquatable<Time>
    {
        #region Constants
        private const char Colon = ':';
        private const char Dot = '.';
        private const char Zero = '0';
        #endregion

        #region Fields
        private readonly long _totalNumberOfNanoseconds;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <see cref="Time"/> with a specified number of hours, minutes, and seconds.
        /// </summary>
        /// <param name="hours">A value between 0 and 23 that specifies the number of hours.</param>
        /// <param name="minutes">A value between 0 and 59 that specifies the number of minutes.</param>
        /// <param name="seconds">A value between 0 and 59 that specifies the number of seconds.</param>
        public Time(int hours, int minutes, int seconds)
            : this(hours, minutes, seconds, default, default) { }

        /// <summary>
        /// Creates a new instance of a <see cref="Time"/> with a specified number of hours, minutes, seconds, milliseconds, and nanoseconds.
        /// </summary>
        /// <param name="hours">A value between 0 and 23 that specifies the number of hours.</param>
        /// <param name="minutes">A value between 0 and 59 that specifies the number of minutes.</param>
        /// <param name="seconds">A value between 0 and 59 that specifies the number of seconds.</param>
        /// <param name="milliseconds">A value between 0 and 999 that specifies the number of milliseconds.</param>
        /// <param name="nanoseconds">A value between 0 and 999 999 that specifies the number of nanoseconds.</param>
        public Time(int hours, int minutes, int seconds, int milliseconds, int nanoseconds)
        {
            if (hours < 0 || hours > 23)
                throw new ArgumentOutOfRangeException(nameof(hours));
            if (minutes < 0 || minutes > 59)
                throw new ArgumentOutOfRangeException(nameof(minutes));
            if (seconds < 0 || seconds > 59)
                throw new ArgumentOutOfRangeException(nameof(seconds));
            if (milliseconds < 0 || milliseconds > 999)
                throw new ArgumentOutOfRangeException(nameof(seconds));
            if (nanoseconds < 0 || nanoseconds > 999_999)
                throw new ArgumentOutOfRangeException(nameof(nanoseconds));

            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds = milliseconds;
            Nanoseconds = nanoseconds;

            _totalNumberOfNanoseconds = ((hours * 3_600L + minutes * 60L + seconds) 
                * 1_000 + milliseconds) 
                * 1_000_000 + nanoseconds;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the hours component of the time interval represented by the current <see cref="Time"/> structure.
        /// </summary>
        public readonly int Hours { get; }

        /// <summary>
        /// Gets the milliseconds component of the time interval represented by the current <see cref="Time"/> structure.
        /// </summary>
        public readonly int Milliseconds { get; }

        /// <summary>
        ///  Gets the minutes component of the time interval represented by the current <see cref="Time"/> structure.
        /// </summary>
        public readonly int Minutes { get; }

        /// <summary>
        ///  Gets the nanoseconds component of the time interval represented by the current <see cref="Time"/> structure.
        /// </summary>
        public readonly int Nanoseconds { get; }

        /// <summary>
        ///  Gets the seconds component of the time interval represented by the current <see cref="Time"/> structure.
        /// </summary>
        public readonly int Seconds { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a time to its <see cref="Time"/> equivalent. Valid formats are HH:MM:SS, HH:MM:SS.mmm and HH:MM:SS.nnnnnnnnn.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Time"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Time Parse(ReadOnlySpan<char> value) => 
            TryParse(value, out Time time) ? time : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a time to its <see cref="Time"/> equivalent. Valid formats are HH:MM:SS, HH:MM:SS.mmm and HH:MM:SS.nnnnnnnnn.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Time"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Time Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out Time time) ? time : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of a time to its <see cref="Time"/> equivalent and returns a value that indicates whether the conversion succeeded. Valid formats are HH:MM:SS, HH:MM:SS.mmm and HH:MM:SS.nnnnnnnnn.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="time">Contains the <see cref="Time"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out Time time)
        {
            if (value.Length >= 8
                && value[2] == Colon && value[5] == Colon
                && int.TryParse(value.Slice(0, 2), out int hours)
                && int.TryParse(value.Slice(3, 2), out int minutes)
                && int.TryParse(value.Slice(6, 2), out int seconds))
            {
                try
                {
                    switch (value.Length)
                    {
                        case 8: //HH:MM:SS
                            time = new Time(hours, minutes, seconds);
                            return true;
                        default:
                            if (value[8] == Dot && int.TryParse(value[9..], out int fractionalSeconds))
                            {
                                switch (value.Length)
                                {
                                    case 12: //HH:MM:SS.mmm
                                        time = new Time(hours, minutes, seconds, fractionalSeconds, default);
                                        return true;
                                    case 18: //HH:MM:SS.nnnnnnnnn
                                        Span<char> destination = stackalloc char[9];
                                        if (fractionalSeconds.TryFormat(destination, out int charsWritten))
                                        {
                                            int milliseconds = 0;
                                            if ((charsWritten > 7
                                                && int.TryParse(destination.Slice(0, 3), out milliseconds)
                                                && int.TryParse(destination.Slice(3, 6), out int nanoseconds))
                                                || int.TryParse(destination.Slice(0, charsWritten), out nanoseconds))
                                            {
                                                time = new Time(hours, minutes, seconds, milliseconds, nanoseconds);
                                                return true;
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                }
                catch { }
            }
            time = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of a time to its <see cref="Time"/> equivalent and returns a value that indicates whether the conversion succeeded. Valid formats are HH:MM:SS, HH:MM:SS.mmm and HH:MM:SS.nnnnnnnnn.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="time">Contains the <see cref="Time"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out Time time)
        {
            if (value.Length < 8 || value.Length > 18)
            {
                time = default;
                return false;
            }
            Span<char> chars = stackalloc char[value.Length];
            Encoding.UTF8.GetChars(value, chars);
            return TryParse(chars, out time);
        }

        /// <summary>
        /// Returns a new <see cref="Time"/> object whose value is the sum of the specified <see cref="Time"/> object and this instance.
        /// </summary>
        /// <param name="time">The time interval to add.</param>
        /// <returns>A new object that represents the value of this instance plus the value of <paramref name="time"/>.</returns>
        public readonly Time Add(Time time)
        {
            int hours = Hours + time.Hours;
            int minutes = Minutes + time.Minutes;
            int seconds = Seconds + time.Seconds;
            int milliseconds = Milliseconds + time.Milliseconds;
            int nanoseconds = Nanoseconds + time.Nanoseconds;

            if (nanoseconds > 999_999)
            {
                nanoseconds -= 1_000_000;
                milliseconds += 1;
            }
            if (milliseconds > 999)
            {
                milliseconds -= 1_000;
                seconds += 1;
            }
            if (seconds > 59)
            {
                seconds -= 60;
                minutes += 1;
            }
            if (minutes > 59)
            {
                minutes -= 60;
                hours += 1;
            }
            if (hours > 23)
                hours -= 24;

            return new Time(hours, minutes, seconds, milliseconds, nanoseconds);
        }

        /// <summary>
        /// Returns a new <see cref="Time"/> object whose value is the difference between the specified <see cref="Time"/> object and this instance.
        /// </summary>
        /// <param name="time">The time interval to be subtracted.</param>
        /// <returns>A new time interval whose value is the result of the value of this instance minus the value of <paramref name="time"/>.</returns>
        public readonly Time Subtract(Time time)
        {
            int hours = Hours - time.Hours;
            int minutes =  Minutes - time.Minutes;
            int seconds = Seconds - time.Seconds;
            int milliseconds = Milliseconds - time.Milliseconds;
            int nanoseconds = Nanoseconds - time.Nanoseconds;

            if (nanoseconds < 0)
            {
                nanoseconds += 1_000_000;
                milliseconds -= 1;
            }
            if (milliseconds < 0)
            {
                milliseconds += 1_000;
                seconds -= 1;
            }
            if (seconds < 0)
            {
                seconds += 60;
                minutes -= 1;
            }
            if (minutes < 0)
            {
                minutes += 60;
                hours -= 1;
            }
            if (hours < 0)
                hours += 24;

            return new Time(hours, minutes, seconds, milliseconds, nanoseconds);
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an indication of their relative values.
        /// </summary>
        /// <param name="obj">An object to compare, or null.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="obj"/>.</returns>
        public readonly int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (!(obj is Time time))
                throw new ArgumentException($"Argument must be of type {nameof(Time)}.", nameof(obj));

            return CompareTo(time);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Time"/> object and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="Time"/> object to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(Time other) => _totalNumberOfNanoseconds.CompareTo(other._totalNumberOfNanoseconds);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Time"/> value.
        /// </summary>
        /// <param name="other">A <see cref="Time"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(Time other) => _totalNumberOfNanoseconds == other._totalNumberOfNanoseconds;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns> A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _totalNumberOfNanoseconds.GetHashCode();

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="Time"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is Time time && Equals(time);

        /// <summary>
        /// Converts the value of the current <see cref="Time"/> object to its equivalent string representation of HH:MM:SS, HH:MM:SS.mmm or HH:MM:SS.nnnnnnnnn.
        /// </summary>
        /// <returns>The string representation of the current <see cref="Time"/> value.</returns>
        public readonly override string ToString()
        {
            Span<char> chars = stackalloc char[18]; //HH:MM:SS.nnnnnnnnn
            Span<char> format = stackalloc char[2] { 'D', '2' };
            Span<char> destination = stackalloc char[2];
            if (Hours.TryFormat(destination, out int charsWritten, format, CultureInfo.InvariantCulture) && charsWritten == 2)
            {
                chars[0] = destination[0]; //H
                chars[1] = destination[1]; //H
                chars[2] = Colon; //:
                if (Minutes.TryFormat(destination, out charsWritten, format, CultureInfo.InvariantCulture) && charsWritten == 2)
                {
                    chars[3] = destination[0]; //M
                    chars[4] = destination[1]; //M
                    chars[5] = Colon; //:
                    if (Seconds.TryFormat(destination, out charsWritten, format, CultureInfo.InvariantCulture) && charsWritten == 2)
                    {
                        chars[6] = destination[0]; //S
                        chars[7] = destination[1]; //S

                        int length = 8;
                        if (Milliseconds != default)
                        {
                            destination = stackalloc char[3];
                            format[1] = '3';
                            if (Milliseconds.TryFormat(destination, out charsWritten, format, CultureInfo.InvariantCulture))
                            {
                                chars[8] = Dot; //.
                                chars[9] = destination[0]; //m
                                chars[10] = destination[1]; //m
                                chars[11] = destination[2]; //m
                                length = 12;
                            }
                            else
                                return SlowToString();
                        }

                        if (Nanoseconds != default)
                        {
                            if (Milliseconds == default)
                            {
                                chars[8] = Dot; //.
                                chars[9] = Zero; //n
                                chars[10] = Zero; //n
                                chars[11] = Zero; //n
                                length = 12;
                            }

                            destination = stackalloc char[6];
                            format[1] = '6';
                            if (Nanoseconds.TryFormat(destination, out charsWritten, format, CultureInfo.InvariantCulture))
                            {
                                chars[12] = destination[0]; //n
                                chars[13] = destination[1]; //n
                                chars[14] = destination[2]; //n
                                chars[15] = destination[3]; //n
                                chars[16] = destination[4]; //n
                                chars[17] = destination[5]; //n
                                length = 18;
                            }
                            else
                                return SlowToString();
                        }
                        return chars.Slice(0, length).ToString();
                    }
                }
            }
            return SlowToString();
        }

        private readonly string SlowToString()
        {
            Debug.Fail($"Unexpectedly failed to format one of the properties property using the {nameof(int.TryFormat)} method and ended up in the {nameof(SlowToString)} method.");

            const string Format = "D2";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{Hours.ToString(Format)}{Colon}{Minutes.ToString(Format)}{Colon}{Seconds.ToString(Format)}");

            if (Milliseconds != default)
                stringBuilder.Append($".{Milliseconds.ToString("D3", CultureInfo.InvariantCulture)}");

            if (Nanoseconds != default)
            {
                if (Milliseconds == default)
                    stringBuilder.Append($".000");
                stringBuilder.Append(Nanoseconds.ToString("D6", CultureInfo.InvariantCulture));
            }

            return stringBuilder.ToString().TrimEnd(Zero);
        }
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="Time"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Time left, Time right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="Time"/> instances are mot equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(Time left, Time right) => !left.Equals(right);

        /// <summary>
        /// Determines whether one specified <see cref="Time"/> is earlier than another specified <see cref="Time"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(Time left, Time right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="Time"/> is the same as or earlier than another specified <see cref="Time"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(Time left, Time right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="Time"/> is later than another specified <see cref="Time"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(Time left, Time right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="Time"/> is the same as or later than another specified <see cref="Time"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(Time left, Time right) => left.CompareTo(right) >= 0;

        /// <summary>
        ///  Adds two specified <see cref="Time"/> instances.
        /// </summary>
        /// <param name="left">The first time interval to add.</param>
        /// <param name="right">The second time interval to add.</param>
        /// <returns>An object whose value is the sum of the values of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Time operator +(Time left, Time right) => left.Add(right);

        /// <summary>
        /// Subtracts a specified <see cref="Time"/> from another specified <see cref="Time"/>.
        /// </summary>
        /// <param name="left">The minuend.</param>
        /// <param name="right">The subtrahend.</param>
        /// <returns>An object whose value is the result of the value of <paramref name="left"/> minus the value of <paramref name="right"/>.</returns>
        public static Time operator -(Time left, Time right) => left.Subtract(right);
        #endregion
    }
}