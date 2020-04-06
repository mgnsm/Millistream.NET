using System;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents an unsigned 64-bit integer used to uniquely identify an instrument. 
    /// </summary>
    public readonly struct InsRef : IComparable, IComparable<InsRef>, IEquatable<InsRef>, IFormattable
    {
        #region Fields
        private readonly ulong _insRef;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of an <see cref="InsRef"/>.
        /// </summary>
        /// <param name="insRef">An unsigned 64-bit integer used to uniquely identify an instrument.</param>
        public InsRef(ulong insRef) => _insRef = insRef;
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of an unsigned 64-bit integer to its <see cref="InsRef"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>An <see cref="InsRef"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static InsRef Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out InsRef insRef) ? insRef : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of an unsigned 64-bit integer to its <see cref="InsRef"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>An <see cref="InsRef"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static InsRef Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out InsRef insRef) ? insRef : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of an unsigned 64-bit integer to its <see cref="InsRef"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="insRef">Contains the <see cref="InsRef"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out InsRef insRef)
        {
            if (ulong.TryParse(value, out ulong instrumentReference))
            {
                insRef = new InsRef(instrumentReference);
                return true;
            }
            insRef = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of an unsigned 64-bit integer to its <see cref="InsRef"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="insRef">Contains the <see cref="InsRef"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out InsRef insRef)
        {
            if (value.Length > 20)
            {
                insRef = default;
                return false;
            }
            Span<char> chars = stackalloc char[value.Length];
            Encoding.UTF8.GetChars(value, chars);
            return TryParse(chars, out insRef);
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

            if (!(obj is InsRef insRef))
                throw new ArgumentException($"Argument must be of type {nameof(InsRef)}.", nameof(obj));

            return CompareTo(insRef);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="InsRef"/> and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">An <see cref="InsRef"/> to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(InsRef other) => _insRef.CompareTo(other._insRef);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="InsRef"/> value.
        /// </summary>
        /// <param name="other">A <see cref="InsRef"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(InsRef other) => _insRef == other._insRef;

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">A numeric format string.</param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information about this instance.</param>
        /// <returns>The string representation of the value of this instance as specified by format and provider.</returns>
        public readonly string ToString(string format, IFormatProvider formatProvider) => _insRef.ToString(format, formatProvider);

        /// <summary>
        ///  Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _insRef.GetHashCode();

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj"> An object to compare to this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="InsRef"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is InsRef insRef && Equals(insRef);

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this instance, consisting of a sequence of digits ranging from 0 to 9, without a sign or leading zeroes.</returns>
        public readonly override string ToString() => _insRef.ToString();
        #endregion

        #region Operators
        /// <summary>
        /// Determines whether one specified <see cref="InsRef"/> is less than another specified <see cref="InsRef"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(InsRef left, InsRef right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="InsRef"/> is equal to or less than another specified <see cref="InsRef"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is equal to or less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(InsRef left, InsRef right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="InsRef"/> is greater than another specified <see cref="InsRef"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(InsRef left, InsRef right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="InsRef"/> is greater than or equal to than another specified <see cref="InsRef"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(InsRef left, InsRef right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Indicates whether two <see cref="InsRef"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(InsRef left, InsRef right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="InsRef"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(InsRef left, InsRef right) => !left.Equals(right);

        /// <summary>
        /// Indicates whether an <see cref="InsRef"/> is less than a <see cref="ulong"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(InsRef left, ulong right) => left._insRef.CompareTo(right) < 0;

        /// <summary>
        /// Indicates whether an <see cref="InsRef"/> is less than or equal to a <see cref="ulong"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(InsRef left, ulong right) => left._insRef.CompareTo(right) <= 0;

        /// <summary>
        /// Indicates whether an <see cref="InsRef"/> is greater than a <see cref="ulong"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(InsRef left, ulong right) => left._insRef.CompareTo(right) > 0;

        /// <summary>
        /// Indicates whether an <see cref="InsRef"/> is greater than or equal to a <see cref="ulong"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(InsRef left, ulong right) => left._insRef.CompareTo(right) >= 0;

        /// <summary>
        /// Indicates whether an <see cref="InsRef"/> and a <see cref="ulong"/> are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(InsRef left, ulong right) => left._insRef.Equals(right);

        /// <summary>
        /// Indicates whether an <see cref="InsRef"/> and a <see cref="ulong"/> are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(InsRef left, ulong right) => !left._insRef.Equals(right);

        /// <summary>
        /// Indicates whether an <see cref="InsRef"/> is less than a <see cref="ulong"/>.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(ulong left, InsRef right) => right._insRef.CompareTo(left) > 0;

        /// <summary>
        /// Indicates whether an <see cref="InsRef"/> is less than or equal to a <see cref="ulong"/>.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> is less than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(ulong left, InsRef right) => right._insRef.CompareTo(left) >= 0;

        /// <summary>
        /// Indicates whether a <see cref="ulong"/> is greater than an <see cref="InsRef"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(ulong left, InsRef right) => right._insRef.CompareTo(left) < 0;

        /// <summary>
        /// Indicates whether a <see cref="ulong"/> is greater than or equal to an <see cref="InsRef"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns> true if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(ulong left, InsRef right) => right._insRef.CompareTo(left) <= 0;

        /// <summary>
        /// Indicates whether a <see cref="ulong"/> and an <see cref="InsRef"/> are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(ulong left, InsRef right) => right._insRef.Equals(left);

        /// <summary>
        /// Indicates whether a <see cref="ulong"/> and an <see cref="InsRef"/> are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(ulong left, InsRef right) => !right._insRef.Equals(left);

        /// <summary>
        /// Converts an <see cref="InsRef"/> to a <see cref="ulong"/>.
        /// </summary>
        /// <param name="insRef">An <see cref="InsRef"/>.</param>
        public static implicit operator ulong(InsRef insRef) => insRef._insRef;

        /// <summary>
        /// Converts a <see cref="ulong"/> to an <see cref="InsRef"/>.
        /// </summary>
        /// <param name="ulong">A <see cref="ulong"/>.</param>
        public static explicit operator InsRef(ulong @ulong) => new InsRef(@ulong);
        #endregion
    }
}