using Millistream.Streaming.DataTypes.Formatting;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming.DataTypes
{
    public readonly partial struct Number : IFormattable, IComparable, IComparable<Number>, IEquatable<Number>
    {
        private const byte Minus = (byte)'-';
        private const byte Period = (byte)'.';
        private const byte Plus = (byte)'+';
        private const byte LowercaseN = (byte)'n';
        private const byte UppercaseN = (byte)'N';
        private const byte LowercaseU = (byte)'u';
        private const byte UppercaseU = (byte)'U';
        private const byte LowercaseL = (byte)'l';
        private const byte UppercaseL = (byte)'L';

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a sequence of digits with an optional decimal point and sign character ('-', '0'–'9' and '.') to its <see cref="Number"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Number"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Number Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out Number number) ? number : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of a sequence of digits with an optional decimal point and sign character ('-', '0'–'9' and '.') to its <see cref="Number"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="number">Contains the <see cref="Number"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out Number number)
        {
            if (value.Length < 1)
            {
                number = default;
                return false;
            }

            // Consume the leading sign if any.
            int index = 0;
            bool isNegative = false;
            byte c = value[index];
            switch (c)
            {
                case Minus:
                    isNegative = true;
                    goto case Plus;
                case Plus:
                    index++;
                    // Sign must be followed by a digit.
                    if (index == value.Length || !IsDigit(value[index] - 48))
                    {
                        number = default;
                        return false;
                    }
                    break;
                case LowercaseN:
                case UppercaseN:
                    // 'NULL'
                    if (value.Length != 4
                        || (value[1] != LowercaseU && value[1] != UppercaseU)
                        || (value[2] != LowercaseL && value[2] != UppercaseL)
                        || (value[3] != LowercaseL && value[3] != UppercaseL))
                    {
                        number = default;
                        return false;
                    }
                    number = Null;
                    return true;
                default:
                    break;
            }

            // Throw away any leading zeroes
            while (index != value.Length)
            {
                c = value[index];
                if (c != '0')
                    break;
                index++;
            }

            long parsedValue = 0;
            int length = Math.Min(value.Length, 18 + index); // 18 digits always fit into an Int64 without overflowing.
            for (; index < length; index++)
            {
                int nextDigit = value[index] - 48;
                if (!IsDigit(nextDigit))
                    break;
                parsedValue = parsedValue * 10 + nextDigit;
            }

            if (index == value.Length)
            {
                number = new Number(parsedValue, 0, isNegative);
                return true;
            }

            c = value[index];
            if (c == Period)
            {
                // Parse the digits after the decimal point.
                index++;
                int startIndexDigitsAfterDecimal = index;
                for (; index < length; index++)
                {
                    c = value[index];
                    int nextDigit = c - 48;
                    if (!IsDigit(nextDigit))
                    {
                        number = default;
                        return false;
                    }
                    parsedValue = parsedValue * 10 + nextDigit;
                }

                if (index == value.Length)
                {
                    if (index == 1)
                    {
                        // "." is not a valid value.
                        number = default;
                        return false;
                    }

                    // Throw away any trailing zeroes after the decimal point.
                    int decimals = index - startIndexDigitsAfterDecimal;
                    while (index > startIndexDigitsAfterDecimal && value[index - 1] == '0')
                    {
                        decimals--;
                        index--;
                        parsedValue /= 10;
                    }

                    number = new Number(parsedValue, decimals, isNegative);
                    return true;
                }

                // Try to squeeze in another digit into the Int64.
                int digit = value[index] - 48;
                if (!IsDigit(digit))
                {
                    number = default;
                    return false;
                }

                // If parsedValue > (long.MaxValue / 10), any more appended digits will cause overflow.
                // if parsedValue == (long.MaxValue / 10), any nextDigit greater than 7 or 8 (depending on sign) implies overflow.
                if (parsedValue < long.MaxValue / 10 || (parsedValue == long.MaxValue / 10 && (isNegative ? digit < 9 : digit < 8)))
                {
                    parsedValue = parsedValue * 10 + digit;
                    index++;
                }

                // Parsed value is too big to fit into an Int64.
                BigInteger bigParsedValue = new BigInteger(parsedValue);
                for (; index < value.Length; index++)
                {
                    int nextDigit = value[index] - 48;
                    if (!IsDigit(nextDigit))
                    {
                        number = default;
                        return false;
                    }
                    bigParsedValue = bigParsedValue * NumberFormatter.s_ten + nextDigit;
                }

                // Throw away any trailing zeroes after the decimal point.
                int scale = index - startIndexDigitsAfterDecimal;
                while (index > startIndexDigitsAfterDecimal && value[index - 1] == '0')
                {
                    scale--;
                    index--;
                    bigParsedValue /= NumberFormatter.s_ten;
                }

                number = new Number(bigParsedValue, scale, isNegative);
                return true;
            }
            else
            {
                // The parsed value may be too big to fit into an Int64.
                int nextDigit = c - 48;
                if (!IsDigit(nextDigit))
                {
                    number = default;
                    return false;
                }

                if (++index == value.Length)
                {
                    // If parsedValue > (long.MaxValue / 10), any more appended digits will cause overflow.
                    // if parsedValue == (long.MaxValue / 10), any nextDigit greater than 7 or 8 (depending on sign) implies overflow.
                    if (parsedValue < long.MaxValue / 10 || (parsedValue == long.MaxValue / 10 && (isNegative ? nextDigit < 9 : nextDigit < 8)))
                        number = new Number(parsedValue * 10 + nextDigit, 0, isNegative);
                    else
                        number = new Number(new BigInteger(parsedValue) * NumberFormatter.s_ten + nextDigit, 0, isNegative);
                    return true;
                }

                // The parsed value is indeed to big to fit into an Int64.
                BigInteger bigParsedValue = new BigInteger(parsedValue) * NumberFormatter.s_ten + nextDigit;
                for (; index < value.Length; index++)
                {
                    c = value[index];
                    nextDigit = c - 48;
                    if (!IsDigit(nextDigit))
                    {
                        if (c != Period)
                        {
                            number = default;
                            return false;
                        }
                        break;
                    }
                    bigParsedValue = bigParsedValue * NumberFormatter.s_ten + nextDigit;
                }

                if (index == value.Length)
                {
                    number = new Number(bigParsedValue, 0, isNegative);
                    return true;
                }

                // Parse the digits after the decimal point.
                index++;
                int startIndexDigitsAfterDecimal = index;
                for (; index < value.Length; index++)
                {
                    nextDigit = value[index] - 48;
                    if (!IsDigit(nextDigit))
                    {
                        number = default;
                        return false;
                    }
                    bigParsedValue = bigParsedValue * NumberFormatter.s_ten + nextDigit;
                }

                // Throw away any trailing zeroes after the decimal point.
                int scale = index - startIndexDigitsAfterDecimal;
                while (index > startIndexDigitsAfterDecimal && value[index - 1] == '0')
                {
                    scale--;
                    index--;
                    bigParsedValue /= NumberFormatter.s_ten;
                }

                number = new Number(bigParsedValue, scale, isNegative);
                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDigit(int digit) => digit > -1 && digit < 10;
    }
}