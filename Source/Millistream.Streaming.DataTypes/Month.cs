using Millistream.Streaming.DataTypes.Parsing;
using System;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a calendar month.
    /// </summary>
    public readonly struct Month : IComparable, IComparable<Month>, IEquatable<Month>
    {
        #region Fields
        private readonly int _comparableValue;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="Month"/>.
        /// </summary>
        /// <param name="year">A calendar year.</param>
        /// <param name="number">A value between 1 and 12 that represents the number of the month.</param>
        public Month(Year year, int number)
        {
            if (number < 1 || number > 12)
                throw new ArgumentOutOfRangeException(nameof(number));

            Year = year;
            Number = number;
            _comparableValue = year * 12 + number;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Year"/> of the <see cref="Month"/>.
        /// </summary>
        public readonly Year Year { get; }

        /// <summary>
        /// Gets the number of the <see cref="Month"/>.
        /// </summary>
        public readonly int Number { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a month on the format YYYY-MM to its <see cref="Month"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Month"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Month Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out Month month) ? month : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a month on the format YYYY-MM to its <see cref="Month"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Month"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Month Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out Month month) ? month : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of a month on the format YYYY-MM to its <see cref="Month"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="month">Contains the <see cref="Month"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out Month month)
        {
            if (value.Length == 7)
            {
                Span<byte> bytes = stackalloc byte[7];
                Encoding.UTF8.GetBytes(value, bytes);
                return TryParse(bytes, out month);
            }
            month = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of a month on the format YYYY-MM to its <see cref="Month"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="month">Contains the <see cref="Month"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out Month month)
        {
            if (value.Length == 7
                && value[4] == (byte)'-'
                && Utf8Parser.TryParse(value.Slice(0, 4), out uint year)
                && Utf8Parser.TryParse(value.Slice(5, 2), out uint monthNumber))
            {
                try
                {
                    month = new Month(new Year((int)year), (int)monthNumber);
                    return true;
                }
                catch { }
            }
            month = default;
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

            if (!(obj is Month month))
                throw new ArgumentException($"Argument must be of type {nameof(Month)}.", nameof(obj));

            return CompareTo(month);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Month"/> object and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="Month"/> object to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(Month other) => _comparableValue.CompareTo(other._comparableValue);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Month"/> value.
        /// </summary>
        /// <param name="other">A <see cref="Month"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(Month other) => _comparableValue == other._comparableValue;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _comparableValue.GetHashCode();

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="Month"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is Month month && Equals(month);

        /// <summary>
        /// Converts the value of the current <see cref="Month"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Month"/> object.</returns>
        public readonly override string ToString() => $"{Year}-{(Number < 10 ? "0" : string.Empty)}{Number}";
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="Month"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Month left, Month right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="Month"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(Month left, Month right) => !left.Equals(right);

        /// <summary>
        /// Determines whether one specified <see cref="Month"/> is earlier than another specified <see cref="Month"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(Month left, Month right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="Month"/> is the same as or earlier than another specified <see cref="Month"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(Month left, Month right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="Month"/> is later than another specified <see cref="Month"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(Month left, Month right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="Month"/> is the same as or later than another specified <see cref="Month"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(Month left, Month right) => left.CompareTo(right) >= 0;
        #endregion
    }
}