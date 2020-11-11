using Millistream.Streaming.DataTypes.Formatting;
using System;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents an arbitrarily large signed integer with any number of decimals between 0 and 2,147,483,647.
    /// </summary>
    public readonly struct Number : IFormattable, IComparable, IComparable<Number>, IEquatable<Number>
    {
        #region Constants
        private const int MaxNumberOfDecimals = 100;
        #endregion

        #region Fields
        internal readonly bool _isNull;
        internal readonly BigInteger _unscaledNumber;
        #endregion

        #region Contructors
        /// <summary>
        /// Creates an instance of a <see cref="Number"/>.
        /// </summary>
        /// <param name="unscaledNumber">A signed integer.</param>
        /// <param name="scale">A value that represents the number of decimal digits in <paramref name="unscaledNumber"/> that should stored to the right of the decimal point.</param>
        public Number(BigInteger unscaledNumber, int scale)
        {
            if (scale < 0)
                throw new ArgumentOutOfRangeException(nameof(scale));

            if (scale > 0)
            {
                // Trailing zeros don't count.
                BigInteger value = BigInteger.DivRem(unscaledNumber, NumberFormatter.s_ten, out BigInteger lastDigit);
                while (value != BigInteger.Zero && lastDigit == BigInteger.Zero)
                {
                    unscaledNumber = value;
                    scale--;
                    value = BigInteger.DivRem(value, NumberFormatter.s_ten, out lastDigit);
                }
            }

            _unscaledNumber = unscaledNumber;
            _isNull = false;
            Scale = scale;
        }

        private Number(BigInteger unscaledNumber, int scale, object _)
        {
            // Bypass validation of scale.
            _unscaledNumber = unscaledNumber;
            _isNull = false;
            Scale = scale;
        }

        private Number(bool _)
        {
            _unscaledNumber = default;
            _isNull = true;
            Scale = default;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Indicates whether the value of the current <see cref="Number"/> object is negative (less than 0).
        /// </summary>
        public readonly bool IsNegative => _unscaledNumber < BigInteger.Zero;

        /// <summary>
        /// Gets a value that represents a <see cref="Number"/> with a value of NULL.
        /// </summary>
        public static Number Null { get; } = new Number(true);

        /// <summary>
        /// Gets the number of digits that are stored to the right of the decimal point in the current <see cref="Number"/>.
        /// </summary>
        public readonly int Scale { get; }
        #endregion

        #region Methods
        /// <summary>
        ///  Gets the absolute value of a <see cref="Number"/> object.
        /// </summary>
        /// <param name="value">A <see cref="Number"/>.</param>
        /// <returns>The absolute value of <paramref name="value"/>.</returns>
        public static Number Abs(Number value) => value._isNull ? Null : value._unscaledNumber.Sign < 0 ? -value : value;

        /// <summary>
        /// Adds two <see cref="Number"/> values and returns the result.
        /// </summary>
        /// <param name="left">The first number to add.</param>
        /// <param name="right">The second number to add.</param>
        /// <returns> The sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Number Add(Number left, Number right)
        {
            if (left._isNull || right._isNull)
                return Null;

            int scaleDiff = left.Scale - right.Scale;
            if (scaleDiff == 0)
            {
                BigInteger unscaledNumber = left._unscaledNumber + right._unscaledNumber;
                return new Number(unscaledNumber, left.Scale, default);
            }
            else if (scaleDiff < 0) //right has more decimals. Multiply it by 10^scalediff to get the same scale as left.
            {
                BigInteger value = left._unscaledNumber * BigInteger.Pow(10, Math.Abs(scaleDiff));
                return new Number(value + right._unscaledNumber, right.Scale, default);
            }
            else //left has more decimals. Muliply it by 10^scalediff to get the same scale as right.
            {
                BigInteger value1 = right._unscaledNumber * BigInteger.Pow(10, scaleDiff);
                return new Number(left._unscaledNumber + value1, left.Scale, default);
            }
        }

        /// <summary>
        /// Divides one <see cref="Number"/> value by another and returns the result.
        /// </summary>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <returns>The result of the division.</returns>
        /// <remarks>If the result of the division cannot be represented exactly, it will be truncated to a maximum scale of 100.</remarks>
        /// <exception cref="DivideByZeroException"></exception>
        public static Number Divide(Number dividend, Number divisor) => Divide(dividend, divisor, MaxNumberOfDecimals);

        /// <summary>
        /// Divides one <see cref="Number"/> value by another and returns the result.
        /// </summary>
        /// <param name="dividend">The value to be divided.</param>
        /// <param name="divisor">The value to divide by.</param>
        /// <param name="maximumScale">The maximum scale (number of decimals) to use if the result of the division cannot be represented exactly.</param>
        /// <remarks>If the result of the division cannot be represented exactly, it will be truncated to a maximum scale of <paramref name="maximumScale"/>.</remarks>
        /// <returns>The result of the division.</returns>
        /// <exception cref="DivideByZeroException"></exception>
        public static Number Divide(Number dividend, Number divisor, int maximumScale)
        {
            if (dividend._isNull || divisor._isNull)
                return Null;
            if (divisor._unscaledNumber == BigInteger.Zero)
                throw new DivideByZeroException();
            if (maximumScale < 0)
                throw new ArgumentOutOfRangeException(nameof(maximumScale));

            BigInteger unscaledDividend = dividend._unscaledNumber;
            BigInteger unscaledDivisor = divisor._unscaledNumber;
            int scaleDiff = dividend.Scale - divisor.Scale;
            if (scaleDiff < 0)
                unscaledDividend *= BigInteger.Pow(10, Math.Abs(scaleDiff));
            else if (scaleDiff > 0)
                unscaledDivisor *= BigInteger.Pow(10, scaleDiff);

            BigInteger result = BigInteger.DivRem(unscaledDividend, unscaledDivisor, out BigInteger remainder);
            int scale = 0;
            while (remainder != BigInteger.Zero && scale < maximumScale)
            {
                remainder *= 10;
                BigInteger decimalDigit = BigInteger.DivRem(remainder, unscaledDivisor, out remainder);
                result = result * 10 + decimalDigit;
                scale++;
            }
            return new Number(result, scale, default);
        }

        /// <summary>
        /// Gets the total number of digits in the <see cref="Number"/>.
        /// </summary>
        /// <returns>The total number of digits in the <see cref="Number"/></returns>
        public readonly int GetPrecision()
        {
            if (_isNull)
                return 0;

            int precision = FormattingHelpers.CountDigits(_unscaledNumber);
            int scale = Scale;
            if (precision < scale)
                return precision + (scale - precision) + 1;
            return precision;
        }

        /// <summary>
        /// Returns the product of two <see cref="Number"/> values.
        /// </summary>
        /// <param name="left">The first value to multiply.</param>
        /// <param name="right">The second value to multiply.</param>
        /// <returns>The product of the <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Number Multiply(Number left, Number right) => left._isNull || right._isNull ? Null
            : new Number(left._unscaledNumber * right._unscaledNumber, left.Scale + right.Scale, default);


        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a sequence of digits with an optional decimal point and sign character ('-', '0'–'9' and '.') to its <see cref="Number"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Number"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Number Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out Number number) ? number : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a sequence of digits with an optional decimal point and sign character ('-', '0'–'9' and '.') to its <see cref="Number"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Number"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Number Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out Number number) ? number : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));


        /// <summary>
        ///  Subtracts one <see cref="Number"/> value from another and returns the result.
        /// </summary>
        /// <param name="left">The minuend.</param>
        /// <param name="right">The subtrahend.</param>
        /// <returns>The result of subtracting <paramref name="right"/> from <paramref name="left"/>.</returns>
        public static Number Subtract(Number left, Number right)
        {
            if (left._isNull || right._isNull)
                return Null;

            int scaleDiff = left.Scale - right.Scale;
            if (scaleDiff == 0)
            {
                BigInteger unscaledNumber = left._unscaledNumber - right._unscaledNumber;
                return new Number(unscaledNumber, left.Scale, default);
            }
            else if (scaleDiff < 0) //right has more decimals. Multiply it by 10^scalediff to get the same scale as left.
            {
                BigInteger value = left._unscaledNumber * BigInteger.Pow(10, Math.Abs(scaleDiff));
                return new Number(value - right._unscaledNumber, right.Scale, default);
            }
            else //left has more decimals. Muliply it by 10^scalediff to get the same scale as right.
            {
                BigInteger value1 = right._unscaledNumber * BigInteger.Pow(10, scaleDiff);
                return new Number(left._unscaledNumber - value1, left.Scale, default);
            }
        }

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of a sequence of digits with an optional decimal point and sign character ('-', '0'–'9' and '.') to its <see cref="Number"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="number">Contains the <see cref="Number"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out Number number)
        {
            if (value.Length == 0)
            {
                number = default;
                return false;
            }

            int index = value.Length - 1;
            while (value[index] == '0')
            {
                if (index-- == 0)
                {
                    number = new Number(BigInteger.Zero, 0, default);
                    return true;
                }
            }

            //Find the optional decimal point character (.)
            for (int i = index; i >= 0; i--)
            {
                if (value[i] == '.')
                {
                    if (i == value.Length - 1 || i == 0 || !char.IsDigit(value[i + 1]) || !char.IsDigit(value[i - 1]))
                    {
                        number = default;
                        return false;
                    }

                    Span<char> chars = stackalloc char[value.Length - 1];
                    for (int j = 0; j < i; j++)
                        chars[j] = value[j];
                    //...and remove it by shifting all digits to the right of it one step to the left
                    for (int j = i + 1; j <= index; j++)
                        chars[j - 1] = value[j];

                    return TryParse(chars, index - i, out number);
                }
            }
            return TryParse(value, 0, out number);
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of a sequence of digits with an optional decimal point and sign character ('-', '0'–'9' and '.') to its <see cref="Number"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="number">Contains the <see cref="Number"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out Number number)
        {
            Span<char> chars = stackalloc char[value.Length];
            Encoding.UTF8.GetChars(value, chars);
            return TryParse(chars, out number);
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

            if (!(obj is Number number))
                throw new ArgumentException($"Argument must be of type {nameof(Number)}.", nameof(obj));

            return CompareTo(number);
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="Number"/> object and returns an indication of their relative values.
        /// </summary>
        /// <param name="other">A <see cref="Number"/> object to compare.</param>
        /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
        public readonly int CompareTo(Number other)
        {
            if (_isNull)
                return other._isNull ? 0 : -1;
            else if (other._isNull)
                return 1;

            return (this - other)._unscaledNumber.Sign;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Number"/> value.
        /// </summary>
        /// <param name="other">A <see cref="Number"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly bool Equals(Number other) => CompareTo(other) == 0;

        /// <summary>
        /// Converts the numeric value of the current <see cref="Number"/> object to its equivalent string representation by using the specified culture-specific formatting information.
        /// </summary>
        /// <param name="formatProvider"> An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the current <see cref="Number"/> value in the format specified by the <paramref name="formatProvider"/> parameter.</returns>
        public readonly string ToString(IFormatProvider formatProvider) => ToString(null, formatProvider);

        /// <summary>
        /// Converts the numeric value of the current <see cref="Number"/> object to its equivalent string representation by using the specified format.
        /// </summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <returns>The string representation of the current <see cref="Number"/> value in the format specified by the <paramref name="format"/> parameter.</returns>
        /// <exception cref="FormatException"></exception>
        public readonly string ToString(string format) => ToString(format, NumberFormatInfo.CurrentInfo);

        /// <summary>
        /// Converts the numeric value of the current <see cref="Number"/> object to its equivalent string representation by using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="formatProvider"> An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the current <see cref="Number"/> value in the format specified by the <paramref name="formatProvider"/> and <paramref name="format"/> parameters.</returns>
        /// <exception cref="FormatException"></exception>
        public readonly string ToString(string format, IFormatProvider formatProvider) => NumberFormatter.Format(this, format, formatProvider);

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <see cref="Number"/> value.
        /// </summary>
        /// <param name="other">An <see cref="Number"/> value to compare to this instance.</param>
        /// <returns>true if <paramref name="other"/> has the same value as this instance; otherwise, false.</returns>
        public readonly override bool Equals(object other) => other is Number number && Equals(number);

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode() => HashCode.Combine(_unscaledNumber.GetHashCode(), Scale.GetHashCode());

        /// <summary>
        /// Converts the value of the current <see cref="Number"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Number"/> object.</returns>
        public readonly override string ToString() => ToString(null, NumberFormatInfo.CurrentInfo);

        private static bool TryParse(ReadOnlySpan<char> value, int scale, out Number number)
        {
            if (scale == 0 && value.Length == 4
                && (value[0] == 'N' || value[0] == 'n')
                && (value[1] == 'U' || value[1] == 'u')
                && (value[2] == 'L' || value[2] == 'l')
                && (value[3] == 'L' || value[3] == 'l'))
            {
                number = Null;
                return true;
            }
            else if (BigInteger.TryParse(value, out BigInteger bigInteger))
            {
                number = new Number(bigInteger, scale, default);
                return true;
            }
            number = default;
            return false;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="Number"/> objects are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Number left, Number right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="Number"/> objects are not equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(Number left, Number right) => !left.Equals(right);

        /// <summary>
        /// Determines whether one specified <see cref="Number"/> is less than another specified <see cref="Number"/>.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> is less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <(Number left, Number right) => left.CompareTo(right) < 0;

        /// <summary>
        /// Determines whether one specified <see cref="Number"/> is equal to or less than another specified <see cref="Number"/>.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> is equal to or less than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator <=(Number left, Number right) => left.CompareTo(right) <= 0;

        /// <summary>
        /// Determines whether one specified <see cref="Number"/> is greater than another specified <see cref="Number"/>.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> is greater than <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >(Number left, Number right) => left.CompareTo(right) > 0;

        /// <summary>
        /// Determines whether one specified <see cref="Number"/> is greater than or equal to than another specified <see cref="Number"/>.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left"/> is greater than or equal to <paramref name="right"/>; otherwise, false.</returns>
        public static bool operator >=(Number left, Number right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Returns the value of the <see cref="Number"/> operand. (The sign of the operand is unchanged.)
        /// </summary>
        /// <param name="value">A <see cref="Number"/>.</param>
        /// <returns>The value of the <paramref name="value"/> operand.</returns>
        public static Number operator +(Number value) => value;

        /// <summary>
        /// Negates a specified <see cref="Number"/> value.
        /// </summary>
        /// <param name="value">The <see cref="Number"/> to negate.</param>
        /// <returns>The result of the <paramref name="value"/> parameter multiplied by negative one (-1).</returns>
        public static Number operator -(Number value) => value._isNull ? value : new Number(-value._unscaledNumber, value.Scale, default);

        /// <summary>
        /// Increments a <see cref="Number"/> value by 1.
        /// </summary>
        /// <param name="value"> The value to increment.</param>
        /// <returns>The value of the <paramref name="value"/> parameter incremented by 1.</returns>
        public static Number operator ++(Number value) => value._isNull ? value : value + new Number(BigInteger.One, 0, default);

        /// <summary>
        /// Decrements a <see cref="Number"/> value by 1.
        /// </summary>
        /// <param name="value"> The value to decrement.</param>
        /// <returns>The value of the <paramref name="value"/> parameter decremented by 1.</returns>
        public static Number operator --(Number value) => value._isNull ? value : value - new Number(BigInteger.One, 0, default);

        /// <summary>
        /// Adds two specified <see cref="Number"/> objects.
        /// </summary>
        /// <param name="left">The first number to add.</param>
        /// <param name="right">The second number to add.</param>
        /// <returns> The sum of <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Number operator +(Number left, Number right) => Add(left, right);

        /// <summary>
        /// Subtracts a specified <see cref="Number"/> from another specified <see cref="Number"/>.
        /// </summary>
        /// <param name="left">The minuend.</param>
        /// <param name="right">The subtrahend.</param>
        /// <returns>The result of subtracting <paramref name="right"/> from <paramref name="left"/>.</returns>
        public static Number operator -(Number left, Number right) => Subtract(left, right);

        /// <summary>
        /// Multiplies two specified <see cref="Number"/> values.
        /// </summary>
        /// <param name="left">The first value to multiply.</param>
        /// <param name="right">The second value to multiply.</param>
        /// <returns>Te product of the <paramref name="left"/> and <paramref name="right"/>.</returns>
        public static Number operator *(Number left, Number right) => Multiply(left, right);

        /// <summary>
        /// Divides a specified <see cref="Number"/> value by another specified <see cref="Number"/> value.
        /// </summary>
        /// <param name="left">The value to be divided.</param>
        /// <param name="right">The value to divide by.</param>
        /// <returns>The result of the division.</returns>
        /// <remarks>If the result of the division cannot be represented exactly, it will be truncated to a maximum scale of 100.</remarks>
        /// <exception cref="DivideByZeroException"></exception>
        public static Number operator /(Number left, Number right) => Divide(left, right);
        #endregion
    }
}