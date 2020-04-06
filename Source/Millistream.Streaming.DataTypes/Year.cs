using System;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a calendar year between 1 and 9999.
    /// </summary>
    public readonly struct Year : IComparable, IComparable<Year>, IEquatable<Year>
    {
        #region Fields
        private readonly int _year;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="Year"/>.
        /// </summary>
        /// <param name="year">A value between 1 and 9999 that represents the calendar year.</param>
        public Year(int year) => _year = (year < 1 || year > 9999) ? throw new ArgumentOutOfRangeException(nameof(year)) : year;
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a year to its <see cref="Year"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Year"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Year Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out Year year) ? year : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a year to its <see cref="Year"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Year"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Year Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out Year year) ? year : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of a year to its <see cref="Year"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="year">Contains the <see cref="Year"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out Year year)
        {
            if (value.Length == 4 && int.TryParse(value, out int y))
            {
                try
                {
                    year = new Year(y);
                    return true;
                }
                catch { }
            }
            year = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of a year to its <see cref="Year"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="year">Contains the <see cref="Year"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out Year year)
        {
            if (value.Length == 4)
            {
                Span<char> chars = stackalloc char[value.Length];
                Encoding.UTF8.GetChars(value, chars);
                return TryParse(chars, out year);
            }
            year = default;
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

            if (!(obj is Year year))
                throw new ArgumentException($"Argument must be of type {nameof(Year)}.", nameof(obj));

            return CompareTo(year);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Year"/> object and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="Year"/> object to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(Year other) => _year.CompareTo(other._year);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Year"/> value.
        /// </summary>
        /// <param name="other">A <see cref="Year"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(Year other) => _year == other._year;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _year.GetHashCode();

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="Year"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is Year year && Equals(year);

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The YYYY string representation of the value of this instance, consisting of a sequence of digits ranging from 0 to 9 with up to 3 leading zeroes.</returns>
        public readonly override string ToString() => _year.ToString().PadLeft(4, '0');
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="Year"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Year left, Year right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="Year"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(Year left, Year right) => !left.Equals(right);

        /// <summary>
        /// Determines whether one specified <see cref="Year"/> is earlier than another specified <see cref="Year"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(Year left, Year right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="Year"/> is the same as or earlier than another specified <see cref="Year"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(Year left, Year right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="Year"/> is later than another specified <see cref="Year"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(Year left, Year right) => left.CompareTo(right) > 0;


        /// <summary>
        /// Determines whether one specified <see cref="Year"/> is the same as or later than another specified <see cref="Year"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(Year left, Year right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Converts a <see cref="Year"/> to an <see cref="int"/>.
        /// </summary>
        /// <param name="year">A <see cref="Year"/>.</param>
        public static implicit operator int(Year year) => year._year;
        #endregion
    }
}
