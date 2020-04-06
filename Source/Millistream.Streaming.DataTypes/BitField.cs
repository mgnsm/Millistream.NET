using System;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    ///  Represents an unsigned integer which forms a binary bit field.
    /// </summary>
    public readonly struct BitField : IComparable, IComparable<BitField>, IEquatable<BitField>, IFormattable
    {
        #region Fields
        private readonly uint _flag;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="BitField"/>.
        /// </summary>
        /// <param name="flag">A bit field value.</param>
        public BitField(uint flag) => _flag = flag;
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether one or more bit fields are set in the current <see cref="BitField"/> instance.
        /// </summary>
        /// <param name="flag">A flag that contains bit field(s) to look for.</param>
        /// <returns>true if the bit field or bit fields that are set in <paramref name="flag"/> are also set in the current instance; otherwise, false.</returns>
        public readonly bool HasFlag(uint flag) => (_flag & flag) != 0;

        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of an unsigned 32-bit integer bit field to its <see cref="BitField"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="BitField"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static BitField Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out BitField bitField) ? bitField : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of an unsigned 32-bit integer bit field to its <see cref="BitField"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="BitField"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static BitField Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out BitField bitField) ? bitField : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of an unsigned 32-bit integer bit field to its <see cref="BitField"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="bitField">Contains the <see cref="BitField"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out BitField bitField)
        {
            if (uint.TryParse(value, out uint i))
            {
                bitField = new BitField(i);
                return true;
            }
            bitField = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of an unsigned 32-bit bit integer field to its <see cref="BitField"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="bitField">Contains the <see cref="BitField"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out BitField bitField)
        {
            if (value.Length > 10)
            {
                bitField = default;
                return false;
            }
            Span<char> chars = stackalloc char[value.Length];
            Encoding.UTF8.GetChars(value, chars);
            return TryParse(chars, out bitField);
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

            if (!(obj is BitField bitField))
                throw new ArgumentException($"Argument must be of type {nameof(BitField)}.", nameof(obj));

            return CompareTo(bitField);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="BitField"/> object and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="BitField"/> object to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(BitField other) => _flag.CompareTo(other._flag);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="BitField"/> value.
        /// </summary>
        /// <param name="other">A <see cref="BitField"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(BitField other) => _flag == other._flag;

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">A numeric format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information about this instance.</param>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="format"/> and <paramref name="formatProvider"/>.</returns>
        public readonly string ToString(string format, IFormatProvider formatProvider) => _flag.ToString(format, formatProvider);

        /// <summary>
        ///  Returns the hash code for this instance.
        /// </summary>
        /// <returns> A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _flag.GetHashCode();

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="BitField"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is BitField bitField && Equals(bitField);

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this instance, consisting of a sequence of digits ranging from 0 to 9, without a sign or leading zeroes.</returns>
        public readonly override string ToString() => _flag.ToString();
        #endregion

        #region Operators
        /// <summary>
        /// Determines whether one specified <see cref="BitField"/> is less than another specified <see cref="BitField"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(BitField left, BitField right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="BitField"/> is equal to or less than another specified <see cref="BitField"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is equal to or less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(BitField left, BitField right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="BitField"/> is greater than another specified <see cref="BitField"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(BitField left, BitField right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="BitField"/> is greater than or equal to than another specified <see cref="BitField"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(BitField left, BitField right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Indicates whether two <see cref="BitField"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(BitField left, BitField right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="BitField"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(BitField left, BitField right) => !left.Equals(right);

        /// <summary>
        /// Indicates whether a <see cref="BitField"/> is less than a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(BitField left, uint right) => left._flag.CompareTo(right) < 0;

        /// <summary>
        /// Indicates whether a <see cref="BitField"/> is less than or equal to a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(BitField left, uint right) => left._flag.CompareTo(right) <= 0;

        /// <summary>
        /// Indicates whether a <see cref="BitField"/> is greater than a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(BitField left, uint right) => left._flag.CompareTo(right) > 0;

        /// <summary>
        /// Indicates whether a <see cref="BitField"/> is greater than or equal to a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(BitField left, uint right) => left._flag.CompareTo(right) >= 0;

        /// <summary>
        /// Indicates whether a <see cref="BitField"/> and a <see cref="uint"/> are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(BitField left, uint right) => left._flag.Equals(right);

        /// <summary>
        /// Indicates whether a <see cref="BitField"/> and a <see cref="uint"/> are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(BitField left, uint right) => !left._flag.Equals(right);

        /// <summary>
        /// Indicates whether a <see cref="BitField"/> is less than a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(uint left, BitField right) => right._flag.CompareTo(left) > 0;

        /// <summary>
        /// Indicates whether a <see cref="BitField"/> is less than or equal to a <see cref="uint"/>.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(uint left, BitField right) => right._flag.CompareTo(left) >= 0;

        /// <summary>
        /// Indicates whether a <see cref="uint"/> is greater than a <see cref="BitField"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(uint left, BitField right) => right._flag.CompareTo(left) < 0;

        /// <summary>
        /// Indicates whether a <see cref="uint"/> is greater than or equal to a <see cref="BitField"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(uint left, BitField right) => right._flag.CompareTo(left) <= 0;

        /// <summary>
        /// Indicates whether a <see cref="uint"/> and a <see cref="BitField"/> are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(uint left, BitField right) => right._flag.Equals(left);

        /// <summary>
        /// Indicates whether a <see cref="uint"/> and a <see cref="BitField"/> are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(uint left, BitField right) => !right._flag.Equals(left);

        /// <summary>
        /// Converts a <see cref="BitField"/> to a <see cref="uint"/>.
        /// </summary>
        /// <param name="uint">a <see cref="BitField"/>.</param>
        public static implicit operator uint(BitField @uint) => @uint._flag;

        /// <summary>
        /// Converts a <see cref="uint"/> to a <see cref="BitField"/>.
        /// </summary>
        /// <param name="uint">A <see cref="uint"/>.</param>
        public static explicit operator BitField(uint @uint) => new BitField(@uint);
        #endregion
    }
}