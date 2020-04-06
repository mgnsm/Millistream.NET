using System;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a calendar week.
    /// </summary>
    public readonly struct Week : IComparable, IComparable<Week>, IEquatable<Week>
    {
        #region Fields
        private readonly int _comparableValue;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="Week"/>.
        /// </summary>
        /// <param name="year">A calendar year.</param>
        /// <param name="number">An value between 1 and 53 that represents the ISO-8601 week number.</param>
        public Week(Year year, int number)
        {
            Number = (number < 1 || number > 53) ? throw new ArgumentOutOfRangeException(nameof(number)) : number;
            Year = year;
            _comparableValue = year * 53 + number;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Year"/> of the <see cref="Week"/>.
        /// </summary>
        public readonly Year Year { get; }

        /// <summary>
        /// Gets the ISO-8601 week number of the <see cref="Week"/>.
        /// </summary>
        public readonly int Number { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a week on the form YYYY-Wxx, where xx is an ISO-8601 week number between 1 and 53, to its <see cref="Week"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Week"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Week Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out Week week) ? week : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a week on the form YYYY-Wxx, where xx is an ISO-8601 week number between 1 and 53, to its <see cref="Week"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Week"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Week Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out Week week) ? week : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of a week on the form YYYY-Wxx, where xx is an ISO-8601 week number between 1 and 53, to its <see cref="Week"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="week">Contains the <see cref="Week"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out Week week)
        {
            if ((value.Length == 7 || value.Length == 8)
                && value[4] == '-'
                && value[5] == 'W'
                && int.TryParse(value.Slice(0, 4), out int year)
                && int.TryParse(value.Slice(6, value.Length - 6), out int weekNumber))
            {
                try
                {
                    week = new Week(new Year(year), weekNumber);
                    return true;
                }
                catch { }
            }
            week = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of a week on the form YYYY-Wxx, where xx is an ISO-8601 week number between 1 and 53, to its <see cref="Week"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="week">Contains the <see cref="Week"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out Week week)
        {
            if (value.Length < 7 || value.Length > 8)
            {
                week = default;
                return false;
            }
            Span<char> chars = stackalloc char[value.Length];
            Encoding.UTF8.GetChars(value, chars);
            return TryParse(chars, out week);
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

            if (!(obj is Week Week))
                throw new ArgumentException($"Argument must be of type {nameof(Week)}.", nameof(obj));

            return CompareTo(Week);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Week"/> object and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="Week"/> object to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(Week other) => _comparableValue.CompareTo(other._comparableValue);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Week"/> value.
        /// </summary>
        /// <param name="other">A <see cref="Week"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(Week other) => _comparableValue == other._comparableValue;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _comparableValue.GetHashCode();

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="Week"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is Week Week && Equals(Week);

        /// <summary>
        /// Converts the value of the current <see cref="Week"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Week"/> object.</returns>
        public readonly override string ToString() => $"{Year}-W{(Number < 10 ? "0" : string.Empty)}{Number}";
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="Week"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Week left, Week right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="Week"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(Week left, Week right) => !left.Equals(right);


        /// <summary>
        /// Determines whether one specified <see cref="Week"/> is earlier than another specified <see cref="Week"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(Week left, Week right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="Week"/> is the same as or earlier than another specified <see cref="Week"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(Week left, Week right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="Week"/> is later than another specified <see cref="Week"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(Week left, Week right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="Week"/> is the same as or later than another specified <see cref="Week"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(Week left, Week right) => left.CompareTo(right) >= 0;
        #endregion
    }
}