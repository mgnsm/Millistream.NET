using System;
using System.Buffers.Text;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a bool (true or false) value.
    /// </summary>
    public readonly struct Bool : IComparable, IComparable<Bool>, IEquatable<Bool>
    {
        #region Fields
        private readonly bool _bool;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="Bool"/>.
        /// </summary>
        /// <param name="bool">A value indicating whether the <see cref="Bool"/> is true or false.</param>
        public Bool(bool @bool) => _bool = @bool;
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains the UTF-8 character '1' or '0' to its <see cref="Bool"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 character to parse.</param>
        /// <returns>A <see cref="Bool"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Bool Parse(ReadOnlySpan<char> value) => 
            TryParse(value, out Bool @bool) ? @bool : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the byte of the UTF-8 character '1' or '0' to its <see cref="Bool"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the byte of UTF-8 character to parse.</param>
        /// <returns>A <see cref="Bool"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Bool Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out Bool @bool) ? @bool : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that should contain the UTF-8 character '1' or '0' to its <see cref="Bool"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 character(s) to parse.</param>
        /// <param name="bool">The converted <see cref="Bool"/> value or default depending on whether the conversion succeeded or failed.</param>
        /// <returns>true if value was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out Bool @bool)
        {
            if (value.Length == 1 && byte.TryParse(value, out byte b))
            {
                switch (b)
                {
                    case 0:
                        @bool = new Bool(false);
                        return true;
                    case 1:
                        @bool = new Bool(true);
                        return true;
                }
            }
            @bool = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that should contain the byte of the UTF-8 character '1' or '0' to its <see cref="Bool"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the byte(s) of the UTF-8 character(s) to parse.</param>
        /// <param name="bool">The converted <see cref="Bool"/> value or default depending on whether the conversion succeeded or failed.</param>
        /// <returns>true if value was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out Bool @bool)
        {
            if (value.Length == 1 && Utf8Parser.TryParse(value, out byte b, out int _))
            {
                switch (b)
                {
                    case 0:
                        @bool = new Bool(false);
                        return true;
                    case 1:
                        @bool = new Bool(true);
                        return true;
                }
            }
            @bool = default;
            return false;
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Bool"/> object and returns an integer that indicates their relationship to one another.
        /// </summary>
        /// <param name="obj">A <see cref="Bool"/> object to compare to this instance.</param>
        /// <returns>A signed integer that indicates the relative values of this instance and <paramref name="obj"/>.</returns>
        public readonly int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (!(obj is Bool @bool))
                throw new ArgumentException($"Argument must be of type {nameof(Bool)}.", nameof(obj));

            return CompareTo(@bool);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Bool"/> object and returns an integer that indicates their relationship to one another.
        /// </summary>
        /// <param name="other">A <see cref="Bool"/> object to compare to this instance.</param>
        /// <returns>A signed integer that indicates the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(Bool other) => _bool.CompareTo(other._bool);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Bool"/> object.
        /// </summary>
        /// <param name="other">A <see cref="Bool"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(Bool other) => _bool == other._bool;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Bool"/>.</returns>
        public readonly override int GetHashCode() => _bool.GetHashCode();

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns>true if <paramref name="obj"/> is a <see cref="Bool"/> and has the same value as this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is Bool @bool && Equals(@bool);

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation (either "True" or "False").
        /// </summary>
        /// <returns><see cref="bool.TrueString"/> if the value of this instance is true, or <see cref="bool.FalseString"/> if the value of this instance is false.</returns>
        public readonly override string ToString() => _bool.ToString();
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="Bool"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Bool left, Bool right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="Bool"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(Bool left, Bool right) => !left.Equals(right);

        /// <summary>
        /// Converts a <see cref="Bool"/> to a <see cref="bool"/>.
        /// </summary>
        /// <param name="bool">A <see cref="Bool"/>.</param>
        public static implicit operator bool(Bool @bool) => @bool._bool;

        /// <summary>
        /// Converts a <see cref="bool"/> to a <see cref="Bool"/>.
        /// </summary>
        /// <param name="bool">A <see cref="bool"/>.</param>
        public static explicit operator Bool(bool @bool) => new Bool(@bool);
        #endregion
    }
}