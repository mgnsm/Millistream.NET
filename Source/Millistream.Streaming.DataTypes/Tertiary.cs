using System;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a tertiary of a calendar year.
    /// </summary>
    public readonly struct Tertiary : IComparable, IComparable<Tertiary>, IEquatable<Tertiary>
    {
        #region Fields
        private readonly int _comparableValue;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="Tertiary"/>.
        /// </summary>
        /// <param name="year">A calendar year.</param>
        /// <param name="number">A value between 1 and 3 that represents the number of the tertiary.</param>
        public Tertiary(Year year, int number)
        {
            Number = (number < 1 || number > 3) ? throw new ArgumentOutOfRangeException(nameof(Number)) : number;
            Year = year;
            _comparableValue = year * 3 + number;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Year"/> of the <see cref="Tertiary"/>.
        /// </summary>
        public readonly Year Year { get; }

        /// <summary>
        /// Gets the number of the <see cref="Tertiary"/>.
        /// </summary>
        public readonly int Number { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a tertiary on the form YYYY-Tx, where x is an integer between 1 and 3, to its <see cref="Tertiary"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Tertiary"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Tertiary Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out Tertiary tertiary) ? tertiary : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a tertiary on the form YYYY-Tx, where x is an integer between 1 and 3, to its <see cref="Tertiary"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Tertiary"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Tertiary Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out Tertiary tertiary) ? tertiary : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of a tertiary on the form YYYY-Tx, where x is an integer between 1 and 3, to its <see cref="Tertiary"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="tertiary">Contains the <see cref="Tertiary"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out Tertiary tertiary)
        {
            if (value.Length == 7
                && value[4] == '-'
                && value[5] == 'T'
                && int.TryParse(value.Slice(0, 4), out int year)
                && int.TryParse(value.Slice(6, 1), out int t))
            {
                try
                {
                    tertiary = new Tertiary(new Year(year), t);
                    return true;
                }
                catch { }
            }
            tertiary = default;
            return false;
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of a tertiary on the form YYYY-Tx, where x is an integer between 1 and 3, to its <see cref="Tertiary"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="tertiary">Contains the <see cref="Tertiary"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out Tertiary tertiary)
        {
            if (value.Length == 7)
            {
                Span<char> chars = stackalloc char[value.Length];
                Encoding.UTF8.GetChars(value, chars);
                return TryParse(chars, out tertiary);
            }
            tertiary = default;
            return false;
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an indication of their relative values.
        /// </summary>
        /// <param name="obj"> An object to compare, or null.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="obj"/>.</returns>
        public readonly int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (!(obj is Tertiary Tertiary))
                throw new ArgumentException($"Argument must be of type {nameof(Tertiary)}.", nameof(obj));

            return CompareTo(Tertiary);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Tertiary"/> object and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="Tertiary"/> object to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(Tertiary other) => _comparableValue.CompareTo(other._comparableValue);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Tertiary"/> value.
        /// </summary>
        /// <param name="other">A <see cref="Tertiary"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(Tertiary other) => _comparableValue == other._comparableValue;

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => _comparableValue.GetHashCode();

        /// <summary>
        ///  Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>true if <paramref name="obj"/> is an instance of <see cref="Tertiary"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is Tertiary Tertiary && Equals(Tertiary);

        /// <summary>
        /// Converts the value of the current <see cref="Tertiary"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Tertiary"/> object.</returns>
        public readonly override string ToString() => $"{Year}-T{Number}";
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="Tertiary"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Tertiary left, Tertiary right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="Tertiary"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(Tertiary left, Tertiary right) => !left.Equals(right);

        /// <summary>
        /// Determines whether one specified <see cref="Tertiary"/> is earlier than another specified <see cref="Tertiary"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(Tertiary left, Tertiary right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="Tertiary"/> is the same as or earlier than another specified <see cref="Tertiary"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or earlier than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(Tertiary left, Tertiary right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="Tertiary"/> is later than another specified <see cref="Tertiary"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(Tertiary left, Tertiary right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="Tertiary"/> is the same as or later than another specified <see cref="Tertiary"/>.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if <paramref name="left"/> is the same as or later than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(Tertiary left, Tertiary right) => left.CompareTo(right) >= 0;
        #endregion
    }
}