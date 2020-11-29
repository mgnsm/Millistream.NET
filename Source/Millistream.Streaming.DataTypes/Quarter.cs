using Millistream.Streaming.DataTypes.Parsing;
using System;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a calendar quarter.
    /// </summary>
    public readonly struct Quarter : IComparable, IComparable<Quarter>, IEquatable<Quarter>
    {
        #region Fields
        private readonly int _comparableValue;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="Quarter"/>.
        /// </summary>
        /// <param name="year">A calendar year.</param>
        /// <param name="number">A value between 1 and 4 that represents the number of the quarter.</param>
        public Quarter(Year year, int number)
        {
            Number = (number < 1 || number > 4) ? 
                throw new ArgumentOutOfRangeException(nameof(number)) : number;
            Year = year;
            _comparableValue = year * 4 + number;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Year"/> of the <see cref="Quarter"/>.
        /// </summary>
        public readonly Year Year { get; }

        /// <summary>
        /// Gets the number of the <see cref="Quarter"/>.
        /// </summary>
        public readonly int Number { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a quarter on the form YYYY-Qx, where x is an integer between 1 and 4, to its <see cref="Quarter"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Quarter"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Quarter Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out Quarter quarter) ? quarter : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a quarter on the form YYYY-Qx, where x is an integer between 1 and 4, to its <see cref="Quarter"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Quarter"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Quarter Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out Quarter quarter) ? quarter : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of a quarter on the form YYYY-Qx, where x is an integer between 1 and 4, to its <see cref="Quarter"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="quarter">Contains the <see cref="Quarter"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out Quarter quarter)
        {
            if (value.Length == 7)
            {
                Span<byte> bytes = stackalloc byte[7];
                Encoding.UTF8.GetBytes(value, bytes);
                return TryParse(bytes, out quarter);
            }
            quarter = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of a quarter on the form YYYY-Qx, where x is an integer between 1 and 4, to its <see cref="Quarter"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="quarter">Contains the <see cref="Quarter"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out Quarter quarter)
        {
            if (value.Length == 7
                && value[4] == (byte)'-'
                && value[5] == (byte)'Q'
                && Utf8Parser.TryParse(value.Slice(0, 4), out uint year)
                && Utf8Parser.TryParse(value.Slice(6, 1), out uint q))
            {
                try
                {
                    quarter = new Quarter(new Year((int)year), (int)q);
                    return true;
                }
                catch { }
            }
            quarter = default;
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

            if (!(obj is Quarter quarter))
                throw new ArgumentException($"Argument must be of type {nameof(Quarter)}.", nameof(obj));

            return CompareTo(quarter);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Quarter"/> object and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="Quarter"/> object to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(Quarter other) => _comparableValue.CompareTo(other._comparableValue);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Quarter"/> value.
        /// </summary>
        /// <param name="other">A <see cref="Quarter"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(Quarter other) => _comparableValue == other._comparableValue;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _comparableValue.GetHashCode();

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="Quarter"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is Quarter quarter && Equals(quarter);

        /// <summary>
        /// Converts the value of the current <see cref="Quarter"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Quarter"/> object.</returns>
        public readonly override string ToString() => $"{Year}-Q{Number}";
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="Quarter"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Quarter left, Quarter right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="Quarter"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(Quarter left, Quarter right) => !left.Equals(right);

        /// <summary>
        /// Determines whether one specified <see cref="Quarter"/> is earlier than another specified <see cref="Quarter"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(Quarter left, Quarter right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="Quarter"/> is the same as or earlier than another specified <see cref="Quarter"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(Quarter left, Quarter right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="Quarter"/> is later than another specified <see cref="Quarter"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(Quarter left, Quarter right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="Quarter"/> is the same as or later than another specified <see cref="Quarter"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(Quarter left, Quarter right) => left.CompareTo(right) >= 0;
        #endregion
    }
}