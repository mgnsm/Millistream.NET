using Millistream.Streaming.DataTypes.Parsing;
using System;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a day of a calendar month.
    /// </summary>
    public readonly struct Day : IComparable, IComparable<Day>, IEquatable<Day>
    {
        #region Fields
        private readonly DateTime _comparableValue;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="Day"/>.
        /// </summary>
        /// <param name="month">A calendar month.</param>
        /// <param name="number">A value between 1 and the number of days in <paramref name="month"/>.</param>
        public Day(Month month, int number)
        {
            if (number < 1 || number > DateTime.DaysInMonth(month.Year, month.Number))
                throw new ArgumentOutOfRangeException(nameof(number));

            Year = month.Year;
            Month = month;
            Number = number;
            _comparableValue = new DateTime(month.Year, month.Number, number);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Year"/> of the <see cref="Day"/>.
        /// </summary>
        public readonly Year Year { get; }

        /// <summary>
        /// Gets the <see cref="Month"/> of the <see cref="Day"/>.
        /// </summary>
        public readonly Month Month { get; }

        /// <summary>
        /// Gets the number of the <see cref="Day"/>.
        /// </summary>
        public readonly int Number { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a day on the format YYYY-MM-DD to its <see cref="Day"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Day"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Day Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out Day day) ? day : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a day on the format YYYY-MM-DD to its <see cref="Day"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Day"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Day Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out Day day) ? day : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of a day on the format YYYY-MM-DD to its <see cref="Day"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="day">Contains the <see cref="Day"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out Day day)
        {
            if (value.Length == 10)
            {
                Span<byte> bytes = stackalloc byte[10];
                Encoding.UTF8.GetBytes(value, bytes);
                return TryParse(bytes, out day);
            }
            day = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of a day on the format YYYY-MM-DD to its <see cref="Day"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="day">Contains the <see cref="Day"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out Day day)
        {
            const byte Separator = (byte)'-';
            if (value.Length == 10
                && value[4] == Separator
                && value[7] == Separator
                && Utf8Parser.TryParse(value.Slice(0, 4), out uint year)
                && Utf8Parser.TryParse(value.Slice(5, 2), out uint month)
                && Utf8Parser.TryParse(value.Slice(8, 2), out uint dayNumber))
            {
                try
                {
                    day = new Day(new Month(new Year((int)year), (int)month), (int)dayNumber);
                    return true;
                }
                catch { }
            }
            day = default;
            return false;
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

            if (!(obj is Day Day))
                throw new ArgumentException($"Argument must be of type {nameof(Day)}.", nameof(obj));

            return CompareTo(Day);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Day"/> object and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="Day"/> object to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(Day other) => _comparableValue.CompareTo(other._comparableValue);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Day"/> value.
        /// </summary>
        /// <param name="other">A <see cref="Day"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(Day other) => _comparableValue == other._comparableValue;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _comparableValue.GetHashCode();

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="Day"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is Day Day && Equals(Day);

        /// <summary>
        /// Converts the value of the current <see cref="Day"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Day"/> object.</returns>
        public readonly override string ToString() => $"{Month}-{(Number < 10 ? "0" : string.Empty)}{Number}";
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="Day"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Day left, Day right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="Day"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(Day left, Day right) => !left.Equals(right);

        /// <summary>
        /// Determines whether one specified <see cref="Day"/> is earlier than another specified <see cref="Day"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(Day left, Day right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="Day"/> is the same as or earlier than another specified <see cref="Day"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(Day left, Day right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="Day"/> is later than another specified <see cref="Day"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(Day left, Day right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="Day"/> is the same as or later than another specified <see cref="Day"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(Day left, Day right) => left.CompareTo(right) >= 0;
        #endregion
    }
}