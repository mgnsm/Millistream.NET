using System;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a semi-annual calendar year.
    /// </summary>
    public readonly struct SemiAnnual : IComparable, IComparable<SemiAnnual>, IEquatable<SemiAnnual>
    {
        #region Fields
        private readonly int _comparableValue;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="SemiAnnual"/>.
        /// </summary>
        /// <param name="year">A calendar year.</param>
        /// <param name="number">A value between 1 and 2 that represents the half of the calendar year.</param>
        public SemiAnnual(Year year, int number)
        {
            Number = (number < 1 || number > 2) ? throw new ArgumentOutOfRangeException(nameof(number)) : number;
            Year = year;
            _comparableValue = year * 2 + number;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Year"/> of the <see cref="SemiAnnual"/>.
        /// </summary>
        public readonly Year Year { get; }

        /// <summary>
        /// Gets the half of the <see cref="Year"/> of the <see cref="SemiAnnual"/>.
        /// </summary>
        public readonly int Number { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a semi-annual year on the format YYYY-Hx, where x is an integer between 1 and 2, to its <see cref="SemiAnnual"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="SemiAnnual"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static SemiAnnual Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out SemiAnnual semiAnnual) ? semiAnnual : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a semi-annual year on the format YYYY-Hx, where x is an integer between 1 and 2, to its <see cref="SemiAnnual"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="SemiAnnual"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static SemiAnnual Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out SemiAnnual semiAnnual) ? semiAnnual : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of a semi-annual year on the format YYYY-Hx, where x is an integer between 1 and 2, to its <see cref="Date"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="semiAnnual">Contains the <see cref="SemiAnnual"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out SemiAnnual semiAnnual)
        {
            if (value.Length == 7
                && value[4] == '-'
                && value[5] == 'H'
                && int.TryParse(value.Slice(0, 4), out int year)
                && int.TryParse(value.Slice(6, 1), out int h))
            {
                try
                {
                    semiAnnual = new SemiAnnual(new Year(year), h);
                    return true;
                }
                catch { }
            }
            semiAnnual = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that contains bet bytes of a UTF-8 string representation of a semi-annual year on the format YYYY-Hx, where x is an integer between 1 and 2, to its <see cref="SemiAnnual"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="semiAnnual">Contains the <see cref="SemiAnnual"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out SemiAnnual semiAnnual)
        {
            if (value.Length == 7)
            {
                Span<char> chars = stackalloc char[value.Length];
                Encoding.UTF8.GetChars(value, chars);
                return TryParse(chars, out semiAnnual);
            }
            semiAnnual = default;
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

            if (!(obj is SemiAnnual semiAnnual))
                throw new ArgumentException($"Argument must be of type {nameof(SemiAnnual)}.", nameof(obj));

            return CompareTo(semiAnnual);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="SemiAnnual"/> object and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="SemiAnnual"/> object to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(SemiAnnual other) => _comparableValue.CompareTo(other._comparableValue);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="SemiAnnual"/> value.
        /// </summary>
        /// <param name="other">A <see cref="SemiAnnual"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(SemiAnnual other) => _comparableValue == other._comparableValue;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _comparableValue.GetHashCode();

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="SemiAnnual"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is SemiAnnual semiAnnual && Equals(semiAnnual);

        /// <summary>
        /// Converts the value of the current <see cref="SemiAnnual"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="SemiAnnual"/> object.</returns>
        public readonly override string ToString() => $"{Year}-H{Number}";
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="SemiAnnual"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(SemiAnnual left, SemiAnnual right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="SemiAnnual"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(SemiAnnual left, SemiAnnual right) => !left.Equals(right);

        /// <summary>
        /// Determines whether one specified <see cref="SemiAnnual"/> is earlier than another specified <see cref="SemiAnnual"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(SemiAnnual left, SemiAnnual right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="SemiAnnual"/> is the same as or earlier than another specified <see cref="SemiAnnual"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(SemiAnnual left, SemiAnnual right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="SemiAnnual"/> is later than another specified <see cref="SemiAnnual"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(SemiAnnual left, SemiAnnual right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="SemiAnnual"/> is the same as or later than another specified <see cref="SemiAnnual"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(SemiAnnual left, SemiAnnual right) => left.CompareTo(right) >= 0;
        #endregion
    }
}