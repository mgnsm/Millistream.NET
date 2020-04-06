using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a free format string in UTF-8 encoding.
    /// </summary>
    public readonly struct String : IComparable, IComparable<String>, IEquatable<String>, IEnumerable, IEnumerable<char>, ICloneable
    {
        #region Fields
        private readonly string _string;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="String"/>.
        /// </summary>
        /// <param name="string">A string value in UTF-8 encoding.</param>
        public String(string @string) => _string = @string ?? throw new ArgumentNullException(nameof(@string));

        /// <summary>
        /// Creates an instance of a <see cref="String"/>.
        /// </summary>
        /// <param name="value">A memory span that contains a UTF-8 string value.</param>
        public String(ReadOnlySpan<char> value) : this(value.ToString()) { }

        /// <summary>
        /// Creates an instance of a <see cref="String"/>.
        /// </summary>
        /// <param name="value">A memory span that contains the bytes of a UTF-8 string value.</param>
        public String(ReadOnlySpan<byte> value)
        {
            Span<char> chars = stackalloc char[value.Length];
            Encoding.UTF8.GetChars(value, chars);
            _string = chars.ToString();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of characters in the current <see cref="String"/> object.
        /// </summary>
        public readonly int Length => _string?.Length ?? 0;
        #endregion

        #region Methods
        /// <summary>
        /// Compares this instance with a specified <see cref="object"/> and indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified <see cref="object"/>.
        /// </summary>
        /// <param name="obj">An object that evaluates to a <see cref="String"/>.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the value parameter.</returns>
        public readonly int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (!(obj is String @string))
                throw new ArgumentException($"Argument must be of type {nameof(String)}", nameof(obj));

            return CompareTo(@string);
        }

        /// <summary>
        /// Compares this instance with a specified <see cref="String"/> object and indicates whether this instance precedes, follows, or appears in the same position in the sort order as the specified <see cref="String"/>.
        /// </summary>
        /// <param name="other">The <see cref="String"/> to compare with this instance.</param>
        /// <returns>A 32-bit signed integer that indicates whether this instance precedes, follows, or appears in the same position in the sort order as the other parameter.</returns>
        public readonly int CompareTo(String other) => CultureInfo.CurrentCulture.CompareInfo.Compare(_string, other._string, CompareOptions.None);

        /// <summary>
        /// Determines whether this instance and another specified <see cref="String"/> object have the same value.
        /// </summary>
        /// <param name="other">The <see cref="String"/> to compare to this instance.</param>
        /// <returns> true if the value of the value parameter is the same as the value of this instance; otherwise, false.</returns>
        public readonly bool Equals(String other) => _string == other._string;

        /// <summary>
        /// Determines whether this instance and a specified object have the same value.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>true if obj is a <see cref="String"/> and its value is the same as this instance; otherwise, false. If obj is null, the method returns false.</returns>
        public readonly override bool Equals(object obj) => obj is String @string && Equals(@string);

        /// <summary>
        /// Returns the hash code for this <see cref="String"/>.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _string?.GetHashCode() ?? 0;

        /// <summary>
        /// Returns the <see cref="string"/> representation of this instance.
        /// </summary>
        /// <returns>The underlying <see cref="string"/> value.</returns>
        public readonly override string ToString() => _string;

        /// <summary>
        /// Retrieves an object that can iterate through the individual characters in this <see cref="String"/>.
        /// </summary>
        /// <returns>An enumerator object.</returns>
        public readonly IEnumerator<char> GetEnumerator() => _string?.GetEnumerator();

        /// <summary>
        /// Retrieves an object that can iterate through the individual characters in this <see cref="String"/>.
        /// </summary>
        /// <returns>An enumerator object.</returns>
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Returns a reference to this instance of <see cref="String"/>.
        /// </summary>
        /// <returns>This instance of <see cref="String"/>.</returns>
        public readonly object Clone() => this;
        #endregion

        #region Operators
        /// <summary>
        /// Determines whether two specified <see cref="String"/> objects have the same value.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the value of <paramref name="left"/> is the same as the value of <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator ==(String left, String right) => string.Equals(left._string, right._string);

        /// <summary>
        /// Determines whether two specified <see cref="String"/> objects have different values.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns> true if the value of <paramref name="left"/> is different from the value of <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator !=(String left, String right) => !string.Equals(left._string, right._string);

        /// <summary>
        /// Converts a <see cref="String"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="string">A <see cref="String"/>.</param>
        public static implicit operator string(String @string) => @string._string;

        /// <summary>
        /// Converts a <see cref="string"/> to a <see cref="String"/>.
        /// </summary>
        /// <param name="string">A <see cref="string"/>.</param>
        public static explicit operator String(string @string) => new String(@string);

        /// <summary>
        /// Converts a <see cref="String"/> to a memory span of characters.
        /// </summary>
        /// <param name="string">A <see cref="String"/></param>
        public static implicit operator ReadOnlySpan<char>(String @string) => @string._string;

        /// <summary>
        /// Gets the <see cref="char"/> object at a specified position in the current <see cref="String"/> object.
        /// </summary>
        /// <param name="index">A position in the current <see cref="String"/>.</param>
        /// <returns>The object at position <paramref name="index"/>.</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public char this[int index] => _string[index];
        #endregion
    }
}