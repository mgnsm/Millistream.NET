using Millistream.Streaming.DataTypes.Parsing;
using System;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    ///  Represents an unsigned integer that is used to create enumerations lists. 
    /// </summary>
    public readonly struct UInt : IComparable, IComparable<UInt>, IEquatable<UInt>, IFormattable
    {
        #region Fields
        private readonly uint _uInt;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="UInt"/>.
        /// </summary>
        /// <param name="uint">An unsigned 32-bit integer.</param>
        public UInt(uint @uint) => _uInt = @uint;
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of an unsigned 32-bit integer to its <see cref="UInt"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="UInt"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static UInt Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out UInt @uint) ? @uint : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of an unsigned 32-bit integer to its <see cref="UInt"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="UInt"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static UInt Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out UInt @uint) ? @uint : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of an unsigned 32-bit integer to its <see cref="UInt"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="uint">Contains the <see cref="UInt"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out UInt @uint)
        {
            if (uint.TryParse(value, out uint i))
            {
                @uint = new UInt(i);
                return true;
            }
            @uint = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of an unsigned 32-bit integer to its <see cref="UInt"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="uint">Contains the <see cref="UInt"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out UInt @uint)
        {
            if (Utf8Parser.TryParse(value, out uint i))
            {
                @uint = new UInt(i);
                return true;
            }
            @uint = default;
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

            if (!(obj is UInt @uint))
                throw new ArgumentException($"Argument must be of type {nameof(UInt)}.", nameof(obj));

            return CompareTo(@uint);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="UInt"/> object and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="UInt"/> object to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(UInt other) => _uInt.CompareTo(other._uInt);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="UInt"/> value.
        /// </summary>
        /// <param name="other">A <see cref="UInt"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(UInt other) => _uInt == other._uInt;

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">A numeric format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information about this instance.</param>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="format"/> and <paramref name="formatProvider"/>.</returns>
        public readonly string ToString(string format, IFormatProvider formatProvider) => _uInt.ToString(format, formatProvider);

        /// <summary>
        ///  Returns the hash code for this instance.
        /// </summary>
        /// <returns> A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _uInt.GetHashCode();

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="UInt"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is UInt @uint && Equals(@uint);

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this instance, consisting of a sequence of digits ranging from 0 to 9, without a sign or leading zeroes.</returns>
        public readonly override string ToString() => _uInt.ToString();
        #endregion

        #region Operators
        /// <summary>
        /// Determines whether one specified <see cref="UInt"/> is less than another specified <see cref="UInt"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(UInt left, UInt right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="UInt"/> is equal to or less than another specified <see cref="UInt"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is equal to or less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(UInt left, UInt right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="UInt"/> is greater than another specified <see cref="UInt"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(UInt left, UInt right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="UInt"/> is greater than or equal to than another specified <see cref="UInt"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(UInt left, UInt right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Indicates whether two <see cref="UInt"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(UInt left, UInt right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="UInt"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(UInt left, UInt right) => !left.Equals(right);

        /// <summary>
        /// Indicates whether a <see cref="UInt"/> is less than a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(UInt left, uint right) => left._uInt.CompareTo(right) < 0;

        /// <summary>
        /// Indicates whether a <see cref="UInt"/> is less than or equal to a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(UInt left, uint right) => left._uInt.CompareTo(right) <= 0;

        /// <summary>
        /// Indicates whether a <see cref="UInt"/> is greater than a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(UInt left, uint right) => left._uInt.CompareTo(right) > 0;

        /// <summary>
        /// Indicates whether a <see cref="UInt"/> is greater than or equal to a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(UInt left, uint right) => left._uInt.CompareTo(right) >= 0;

        /// <summary>
        /// Indicates whether a <see cref="UInt"/> and a <see cref="uint"/> are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(UInt left, uint right) => left._uInt.Equals(right);

        /// <summary>
        /// Indicates whether a <see cref="UInt"/> and a <see cref="uint"/> are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(UInt left, uint right) => !left._uInt.Equals(right);

        /// <summary>
        /// Indicates whether a <see cref="UInt"/> is less than a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(uint left, UInt right) => right._uInt.CompareTo(left) > 0;

        /// <summary>
        /// Indicates whether a <see cref="UInt"/> is less than or equal to a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(uint left, UInt right) => right._uInt.CompareTo(left) >= 0;

        /// <summary>
        /// Indicates whether a <see cref="uint"/> is greater than a <see cref="UInt"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(uint left, UInt right) => right._uInt.CompareTo(left) < 0;

        /// <summary>
        /// Indicates whether a <see cref="uint"/> is greater than or equal to a <see cref="UInt"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(uint left, UInt right) => right._uInt.CompareTo(left) <= 0;

        /// <summary>
        /// Indicates whether a <see cref="uint"/> and a <see cref="UInt"/> are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(uint left, UInt right) => right._uInt.Equals(left);

        /// <summary>
        /// Indicates whether a <see cref="uint"/> and a <see cref="UInt"/> are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(uint left, UInt right) => !right._uInt.Equals(left);

        /// <summary>
        /// Converts a <see cref="UInt"/> to a <see cref="uint"/>.
        /// </summary>
        /// <param name="uint">a <see cref="UInt"/>.</param>
        public static implicit operator uint(UInt @uint) => @uint._uInt;

        /// <summary>
        /// Converts a <see cref="uint"/> to a <see cref="UInt"/>.
        /// </summary>
        /// <param name="uint">A <see cref="uint"/>.</param>
        public static explicit operator UInt(uint @uint) => new UInt(@uint);
        #endregion
    }
}
