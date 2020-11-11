using System;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Millistream.Streaming.DataTypes.Formatting
{
    internal static class NumberFormatter
    {
        internal static readonly BigInteger s_ten = new BigInteger(10);

        private static readonly string[] s_posCurrencyFormats =
        {
            "$#", "#$", "$ #", "# $"
        };

        private static readonly string[] s_negCurrencyFormats =
        {
            "($#)", "-$#", "$-#", "$#-",
            "(#$)", "-#$", "#-$", "#$-",
            "-# $", "-$ #", "# $-", "$ #-",
            "$ -#", "#- $", "($ #)", "(# $)",
            "$- #"
        };

        private static readonly string[] s_posPercentFormats =
        {
            "# %", "#%", "%#", "% #"
        };

        private static readonly string[] s_negPercentFormats =
        {
            "-# %", "-#%", "-%#",
            "%-#", "%#-",
            "#-%", "#%-",
            "-% #", "# %-", "% #-",
            "% -#", "#- %"
        };

        private static readonly string[] s_negNumberFormats =
        {
            "(#)", "-#", "- #", "#-", "# -",
        };

        internal static unsafe string Format(Number value, string format, IFormatProvider formatProvider)
        {
            if (value._isNull)
                return "NULL";

            BigInteger unscaledValue = value._unscaledNumber;
            int scale = value.Scale;
            NumberFormatInfo info = NumberFormatInfo.GetInstance(formatProvider);

            if (string.IsNullOrEmpty(format))
                return FormatDigits(unscaledValue, scale, value.GetPrecision(), info);

            ReadOnlySpan<char> formatSpan = format;
            int precision = FormattingHelpers.CountDigits(unscaledValue);
            char fmt = ParseFormatSpecifier(formatSpan, out int digits);
            if (fmt == 'D' || fmt == 'd')
                return FormatDigits(unscaledValue, scale, Math.Max(digits, precision), info);

            int bufferLength = precision + 1 + 1;
            byte* digitsBuffer = stackalloc byte[bufferLength];
            NumberBufferKind kind = scale > 0 ? NumberBufferKind.Decimal : NumberBufferKind.Integer;
            NumberBuffer number = new NumberBuffer(kind, digitsBuffer, bufferLength);

            BigIntegerToNumber(ref unscaledValue, ref number, precision, scale);

            const int CharStackBufferSize = 32;
            char* charBuffer = stackalloc char[CharStackBufferSize];
            ValueStringBuilder sb = new ValueStringBuilder(new Span<char>(charBuffer, CharStackBufferSize));

            if (fmt != 0)
                NumberToString(ref sb, ref number, fmt, digits, info);
            else
                NumberToStringFormat(ref sb, ref number, formatSpan, info);

            return sb.ToString();
        }

        private static unsafe void BigIntegerToNumber(ref BigInteger d, ref NumberBuffer numberBuffer, int precision, int scale)
        {
            byte* buffer = numberBuffer.GetDigitsPointer();
            numberBuffer.DigitsCount = precision;

            BigInteger value = d;
            if (d < BigInteger.Zero)
            {
                numberBuffer.IsNegative = true;
                value = BigInteger.Abs(value);
            }

            byte* p = buffer + precision;
            int digits = precision;
            while (--digits >= 0 || value != 0)
            {
                value = BigInteger.DivRem(value, s_ten, out BigInteger remainder);
                *--p = (byte)(remainder + '0');
            }

            int i = (int)(buffer + precision - p);

            numberBuffer.DigitsCount = i;
            numberBuffer.Scale = i - scale;

            byte* dst = numberBuffer.GetDigitsPointer();
            while (--i >= 0)
                *dst++ = *p++;
            *dst = (byte)('\0');
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int DivRem(int a, int b, out int result)
        {
            int div = a / b;
            result = a - (div * b);
            return div;
        }

        private static unsafe int FindSection(ReadOnlySpan<char> format, int section)
        {
            int src;
            char ch;

            if (section == 0)
                return 0;

            fixed (char* pFormat = &MemoryMarshal.GetReference(format))
            {
                src = 0;
                while (true)
                {
                    if (src >= format.Length)
                        return 0;

                    switch (ch = pFormat[src++])
                    {
                        case '\'':
                        case '"':
                            while (src < format.Length && pFormat[src] != 0 && pFormat[src++] != ch) ;
                            break;
                        case '\\':
                            if (src < format.Length && pFormat[src] != 0)
                                src++;
                            break;
                        case ';':
                            if (--section != 0)
                                break;
                            if (src < format.Length && pFormat[src] != 0 && pFormat[src] != ';')
                                return src;
                            goto case '\0';
                        case '\0':
                            return 0;
                    }
                }
            }
        }

        private static void FormatCurrency(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info)
        {
            string fmt = number.IsNegative ?
                s_negCurrencyFormats[info.CurrencyNegativePattern] :
                s_posCurrencyFormats[info.CurrencyPositivePattern];

            foreach (char ch in fmt)
            {
                switch (ch)
                {
                    case '#':
                        FormatFixed(ref sb, ref number, nMaxDigits, info.CurrencyGroupSizes, info.CurrencyDecimalSeparator, info.CurrencyGroupSeparator);
                        break;
                    case '-':
                        sb.Append(info.NegativeSign);
                        break;
                    case '$':
                        sb.Append(info.CurrencySymbol);
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
        }

        private static unsafe string FormatDigits(BigInteger unscaledValue, int scale, int numberOfDigits, NumberFormatInfo info)
        {
            int bufferLength = numberOfDigits;
            string sNegative = string.Empty, sDecimal = string.Empty;
            if (scale > 0)
            {
                sDecimal = info.NumberDecimalSeparator ?? string.Empty;
                bufferLength += sDecimal.Length;
            }
            if (unscaledValue < BigInteger.Zero)
            {
                sNegative = info.NegativeSign ?? string.Empty;
                bufferLength += sNegative.Length;
                unscaledValue = BigInteger.Abs(unscaledValue);
            }

            char* buffer = stackalloc char[bufferLength];
            char* p = buffer + bufferLength;
            if (sDecimal.Length > 0)
            {
                for (int i = 0; i < scale; i++)
                {
                    unscaledValue = BigInteger.DivRem(unscaledValue, s_ten, out BigInteger remainder);
                    *--p = (char)(remainder + '0');
                }
                numberOfDigits -= scale;
                for (int i = sDecimal.Length - 1; i >= 0; i--)
                    *--p = sDecimal[i];
            }
            do
            {
                unscaledValue = BigInteger.DivRem(unscaledValue, s_ten, out BigInteger remainder);
                *--p = (char)(remainder + '0');
            }
            while (--numberOfDigits > 0 || unscaledValue != 0);
            if (sNegative.Length > 0)
                for (int i = sNegative.Length - 1; i >= 0; i--)
                    *--p = sNegative[i];

            return new string(buffer, 0, bufferLength);
        }

        private static unsafe void FormatExponent(ref ValueStringBuilder sb, NumberFormatInfo info, int value, char expChar, int minDigits, bool positiveSign)
        {
            sb.Append(expChar);

            if (value < 0)
            {
                sb.Append(info.NegativeSign);
                value = -value;
            }
            else if (positiveSign)
            {
                sb.Append(info.PositiveSign);
            }

            const int MaxUInt32DecDigits = 10;
            char* digits = stackalloc char[MaxUInt32DecDigits];
            char* p = digits + MaxUInt32DecDigits;
            while (--minDigits >= 0 || value != 0)
            {
                value = DivRem(value, 10, out int remainder);
                *--p = (char)(remainder + '0');
            }
            sb.Append(p, (int)(digits + MaxUInt32DecDigits - p));
        }

        private static unsafe void FormatFixed(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, int[] groupDigits, string sDecimal, string sGroup)
        {
            int digPos = number.Scale;
            byte* dig = number.GetDigitsPointer();

            if (digPos > 0)
            {
                if (groupDigits != null)
                {
                    Debug.Assert(sGroup != null, "Must be null when groupDigits != null");
                    int groupSizeIndex = 0;  // Index into the groupDigits array.
                    int bufferSize = digPos; // The length of the result buffer string.
                    int groupSize = 0;       // The current group size.

                    // Find out the size of the string buffer for the result.
                    if (groupDigits.Length != 0) // You can pass in 0 length arrays
                    {
                        int groupSizeCount = groupDigits[groupSizeIndex];   // The current total of group size.

                        while (digPos > groupSizeCount)
                        {
                            groupSize = groupDigits[groupSizeIndex];
                            if (groupSize == 0)
                                break;

                            bufferSize += sGroup.Length;
                            if (groupSizeIndex < groupDigits.Length - 1)
                                groupSizeIndex++;

                            groupSizeCount += groupDigits[groupSizeIndex];
                            if (groupSizeCount < 0 || bufferSize < 0)
                                throw new ArgumentOutOfRangeException(); // If we overflow
                        }

                        groupSize = groupSizeCount == 0 ? 0 : groupDigits[0]; // If you passed in an array with one entry as 0, groupSizeCount == 0
                    }

                    groupSizeIndex = 0;
                    int digitCount = 0;
                    int digLength = number.DigitsCount;
                    int digStart = (digPos < digLength) ? digPos : digLength;
                    fixed (char* spanPtr = &MemoryMarshal.GetReference(sb.AppendSpan(bufferSize)))
                    {
                        char* p = spanPtr + bufferSize - 1;
                        for (int i = digPos - 1; i >= 0; i--)
                        {
                            *p-- = (i < digStart) ? (char)dig[i] : '0';

                            if (groupSize > 0)
                            {
                                digitCount++;
                                if ((digitCount == groupSize) && (i != 0))
                                {
                                    for (int j = sGroup.Length - 1; j >= 0; j--)
                                        *p-- = sGroup[j];

                                    if (groupSizeIndex < groupDigits.Length - 1)
                                    {
                                        groupSizeIndex++;
                                        groupSize = groupDigits[groupSizeIndex];
                                    }
                                    digitCount = 0;
                                }
                            }
                        }

                        Debug.Assert(p >= spanPtr - 1, "Underflow");
                        dig += digStart;
                    }
                }
                else
                {
                    do
                    {
                        sb.Append(*dig != 0 ? (char)(*dig++) : '0');
                    }
                    while (--digPos > 0);
                }
            }
            else
            {
                sb.Append('0');
            }

            if (nMaxDigits > 0)
            {
                Debug.Assert(sDecimal != null);
                sb.Append(sDecimal);
                if ((digPos < 0) && (nMaxDigits > 0))
                {
                    int zeroes = Math.Min(-digPos, nMaxDigits);
                    sb.Append('0', zeroes);
                    digPos += zeroes;
                    nMaxDigits -= zeroes;
                }

                while (nMaxDigits > 0)
                {
                    sb.Append((*dig != 0) ? (char)(*dig++) : '0');
                    nMaxDigits--;
                }
            }
        }

        private static unsafe void FormatGeneral(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info, char expChar, bool bSuppressScientific)
        {
            int digPos = number.Scale;
            bool scientific = false;

            if (!bSuppressScientific)
            {
                // Don't switch to scientific notation
                if (digPos > nMaxDigits || digPos < -3)
                {
                    digPos = 1;
                    scientific = true;
                }
            }

            byte* dig = number.GetDigitsPointer();

            if (digPos > 0)
            {
                do
                {
                    sb.Append((*dig != 0) ? (char)(*dig++) : '0');
                } while (--digPos > 0);
            }
            else
            {
                sb.Append('0');
            }

            if (*dig != 0 || digPos < 0)
            {
                sb.Append(info.NumberDecimalSeparator);

                while (digPos < 0)
                {
                    sb.Append('0');
                    digPos++;
                }

                while (*dig != 0)
                    sb.Append((char)(*dig++));
            }

            if (scientific)
                FormatExponent(ref sb, info, number.Scale - 1, expChar, 2, true);
        }

        private static void FormatNumber(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info)
        {
            const string PosNumberFormat = "#";
            string fmt = number.IsNegative ?
                s_negNumberFormats[info.NumberNegativePattern] :
                PosNumberFormat;

            foreach (char ch in fmt)
            {
                switch (ch)
                {
                    case '#':
                        FormatFixed(ref sb, ref number, nMaxDigits, info.NumberGroupSizes, info.NumberDecimalSeparator, info.NumberGroupSeparator);
                        break;
                    case '-':
                        sb.Append(info.NegativeSign);
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
        }

        private static void FormatPercent(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info)
        {
            string fmt = number.IsNegative ?
                s_negPercentFormats[info.PercentNegativePattern] :
                s_posPercentFormats[info.PercentPositivePattern];

            foreach (char ch in fmt)
            {
                switch (ch)
                {
                    case '#':
                        FormatFixed(ref sb, ref number, nMaxDigits, info.PercentGroupSizes, info.PercentDecimalSeparator, info.PercentGroupSeparator);
                        break;
                    case '-':
                        sb.Append(info.NegativeSign);
                        break;
                    case '%':
                        sb.Append(info.PercentSymbol);
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
        }

        private static unsafe void FormatScientific(ref ValueStringBuilder sb, ref NumberBuffer number, int nMaxDigits, NumberFormatInfo info, char expChar)
        {
            byte* dig = number.GetDigitsPointer();

            sb.Append((*dig != 0) ? (char)*dig++ : '0');

            if (nMaxDigits != 1) // For E0 we would like to suppress the decimal point
                sb.Append(info.NumberDecimalSeparator);

            while (--nMaxDigits > 0)
                sb.Append((*dig != 0) ? (char)*dig++ : '0');

            int e = number.Digits[0] == 0 ? 0 : number.Scale - 1;
            FormatExponent(ref sb, info, e, expChar, 3, true);
        }

        private static unsafe void NumberToString(ref ValueStringBuilder sb, ref NumberBuffer number, char format, int nMaxDigits, NumberFormatInfo info)
        {
            switch (format)
            {
                case 'C':
                case 'c':
                    if (nMaxDigits < 0)
                        nMaxDigits = info.CurrencyDecimalDigits;

                    RoundNumber(ref number, number.Scale + nMaxDigits); // Don't change this line to use digPos since digCount could have its sign changed.

                    FormatCurrency(ref sb, ref number, nMaxDigits, info);

                    break;
                case 'F':
                case 'f':
                    if (nMaxDigits < 0)
                        nMaxDigits = info.NumberDecimalDigits;

                    RoundNumber(ref number, number.Scale + nMaxDigits);

                    if (number.IsNegative)
                        sb.Append(info.NegativeSign);

                    FormatFixed(ref sb, ref number, nMaxDigits, null, info.NumberDecimalSeparator, null);

                    break;
                case 'N':
                case 'n':
                    if (nMaxDigits < 0)
                        nMaxDigits = info.NumberDecimalDigits; // Since we are using digits in our calculation

                    RoundNumber(ref number, number.Scale + nMaxDigits);

                    FormatNumber(ref sb, ref number, nMaxDigits, info);

                    break;
                case 'E':
                case 'e':
                    const int DefaultPrecisionExponentialFormat = 6;
                    if (nMaxDigits < 0)
                        nMaxDigits = DefaultPrecisionExponentialFormat;
                    nMaxDigits++;

                    RoundNumber(ref number, nMaxDigits);

                    if (number.IsNegative)
                        sb.Append(info.NegativeSign);

                    FormatScientific(ref sb, ref number, nMaxDigits, info, format);

                    break;
                case 'G':
                case 'g':
                    bool noRounding = false;
                    if (nMaxDigits < 1)
                    {
                        if ((number.Kind == NumberBufferKind.Decimal) && (nMaxDigits == -1))
                        {
                            noRounding = true;  // Turn off rounding for ECMA compliance to output trailing 0's after decimal as significant

                            if (number.Digits[0] == 0) // -0 should be formatted as 0 for decimal. This is normally handled by RoundNumber (which we are skipping)
                                goto SkipSign;

                            goto SkipRounding;
                        }
                        else
                        {
                            // This ensures that the PAL code pads out to the correct place even when we use the default precision
                            nMaxDigits = number.DigitsCount;
                        }
                    }
                    RoundNumber(ref number, nMaxDigits);

                SkipRounding:
                    if (number.IsNegative)
                        sb.Append(info.NegativeSign);

                    SkipSign:
                    FormatGeneral(ref sb, ref number, nMaxDigits, info, (char)(format - ('G' - 'E')), noRounding);

                    break;
                case 'P':
                case 'p':
                    if (nMaxDigits < 0)
                        nMaxDigits = info.PercentDecimalDigits;
                    number.Scale += 2;

                    RoundNumber(ref number, number.Scale + nMaxDigits);

                    FormatPercent(ref sb, ref number, nMaxDigits, info);

                    break;
                default:
                    throw new FormatException(Constants.BadFormatSpecifier);
            }
        }

        private static unsafe void NumberToStringFormat(ref ValueStringBuilder sb, ref NumberBuffer number, ReadOnlySpan<char> format, NumberFormatInfo info)
        {
            int digitCount;
            int decimalPos;
            int firstDigit;
            int lastDigit;
            int digPos;
            bool scientific;
            int thousandPos;
            int thousandCount = 0;
            bool thousandSeps;
            int scaleAdjust;
            int adjust;

            int section;
            int src;
            byte* dig = number.GetDigitsPointer();
            char ch;

            section = FindSection(format, dig[0] == 0 ? 2 : number.IsNegative ? 1 : 0);

            while (true)
            {
                digitCount = 0;
                decimalPos = -1;
                firstDigit = 0x7FFFFFFF;
                lastDigit = 0;
                scientific = false;
                thousandPos = -1;
                thousandSeps = false;
                scaleAdjust = 0;
                src = section;

                fixed (char* pFormat = &MemoryMarshal.GetReference(format))
                {
                    while (src < format.Length && (ch = pFormat[src++]) != 0 && ch != ';')
                    {
                        switch (ch)
                        {
                            case '#':
                                digitCount++;
                                break;
                            case '0':
                                if (firstDigit == 0x7FFFFFFF)
                                    firstDigit = digitCount;
                                digitCount++;
                                lastDigit = digitCount;
                                break;
                            case '.':
                                if (decimalPos < 0)
                                    decimalPos = digitCount;
                                break;
                            case ',':
                                if (digitCount > 0 && decimalPos < 0)
                                {
                                    if (thousandPos >= 0)
                                    {
                                        if (thousandPos == digitCount)
                                        {
                                            thousandCount++;
                                            break;
                                        }
                                        thousandSeps = true;
                                    }
                                    thousandPos = digitCount;
                                    thousandCount = 1;
                                }
                                break;
                            case '%':
                                scaleAdjust += 2;
                                break;
                            case '\x2030':
                                scaleAdjust += 3;
                                break;
                            case '\'':
                            case '"':
                                while (src < format.Length && pFormat[src] != 0 && pFormat[src++] != ch)
                                    ;
                                break;
                            case '\\':
                                if (src < format.Length && pFormat[src] != 0)
                                    src++;
                                break;
                            case 'E':
                            case 'e':
                                if ((src < format.Length && pFormat[src] == '0') ||
                                    (src + 1 < format.Length && (pFormat[src] == '+' || pFormat[src] == '-') && pFormat[src + 1] == '0'))
                                {
                                    while (++src < format.Length && pFormat[src] == '0')
                                        ;
                                    scientific = true;
                                }
                                break;
                        }
                    }
                }

                if (decimalPos < 0)
                    decimalPos = digitCount;

                if (thousandPos >= 0)
                {
                    if (thousandPos == decimalPos)
                        scaleAdjust -= thousandCount * 3;
                    else
                        thousandSeps = true;
                }

                if (dig[0] != 0)
                {
                    number.Scale += scaleAdjust;
                    int pos = scientific ? digitCount : number.Scale + digitCount - decimalPos;
                    RoundNumber(ref number, pos);
                    if (dig[0] == 0)
                    {
                        src = FindSection(format, 2);
                        if (src != section)
                        {
                            section = src;
                            continue;
                        }
                    }
                }
                else
                {
                    number.IsNegative = false;
                    number.Scale = 0; // Decimals with scale ('0.00') should be rounded.
                }

                break;
            }

            firstDigit = firstDigit < decimalPos ? decimalPos - firstDigit : 0;
            lastDigit = lastDigit > decimalPos ? decimalPos - lastDigit : 0;
            if (scientific)
            {
                digPos = decimalPos;
                adjust = 0;
            }
            else
            {
                digPos = number.Scale > decimalPos ? number.Scale : decimalPos;
                adjust = number.Scale - decimalPos;
            }
            src = section;

            // Adjust can be negative, so we make this an int instead of an unsigned int.
            // Adjust represents the number of characters over the formatting e.g. format string is "0000" and you are trying to
            // format 100000 (6 digits). Means adjust will be 2. On the other hand if you are trying to format 10 adjust will be
            // -2 and we'll need to fixup these digits with 0 padding if we have 0 formatting as in this example.
            Span<int> thousandsSepPos = stackalloc int[4];
            int thousandsSepCtr = -1;

            if (thousandSeps)
            {
                // We need to precompute this outside the number formatting loop
                if (info.NumberGroupSeparator.Length > 0)
                {
                    // We need this array to figure out where to insert the thousands separator. We would have to traverse the string
                    // backwards. PIC formatting always traverses forwards. These indices are precomputed to tell us where to insert
                    // the thousands separator so we can get away with traversing forwards. Note we only have to compute up to digPos.
                    // The max is not bound since you can have formatting strings of the form "000,000..", and this
                    // should handle that case too.

                    int[] groupDigits = info.NumberGroupSizes;

                    int groupSizeIndex = 0;     // Index into the groupDigits array.
                    int groupTotalSizeCount = 0;
                    int groupSizeLen = groupDigits.Length;    // The length of groupDigits array.
                    if (groupSizeLen != 0)
                        groupTotalSizeCount = groupDigits[groupSizeIndex];   // The current running total of group size.
                    int groupSize = groupTotalSizeCount;

                    int totalDigits = digPos + ((adjust < 0) ? adjust : 0); // Actual number of digits in o/p
                    int numDigits = (firstDigit > totalDigits) ? firstDigit : totalDigits;
                    while (numDigits > groupTotalSizeCount)
                    {
                        if (groupSize == 0)
                            break;
                        ++thousandsSepCtr;
                        if (thousandsSepCtr >= thousandsSepPos.Length)
                        {
                            var newThousandsSepPos = new int[thousandsSepPos.Length * 2];
                            thousandsSepPos.CopyTo(newThousandsSepPos);
                            thousandsSepPos = newThousandsSepPos;
                        }

                        thousandsSepPos[thousandsSepCtr] = groupTotalSizeCount;
                        if (groupSizeIndex < groupSizeLen - 1)
                        {
                            groupSizeIndex++;
                            groupSize = groupDigits[groupSizeIndex];
                        }
                        groupTotalSizeCount += groupSize;
                    }
                }
            }

            if (number.IsNegative && (section == 0) && (number.Scale != 0))
                sb.Append(info.NegativeSign);

            bool decimalWritten = false;

            fixed (char* pFormat = &MemoryMarshal.GetReference(format))
            {
                byte* cur = dig;

                while (src < format.Length && (ch = pFormat[src++]) != 0 && ch != ';')
                {
                    if (adjust > 0)
                    {
                        switch (ch)
                        {
                            case '#':
                            case '0':
                            case '.':
                                while (adjust > 0)
                                {
                                    // digPos will be one greater than thousandsSepPos[thousandsSepCtr] since we are at
                                    // the character after which the groupSeparator needs to be appended.
                                    sb.Append(*cur != 0 ? (char)(*cur++) : '0');
                                    if (thousandSeps && digPos > 1 && thousandsSepCtr >= 0)
                                    {
                                        if (digPos == thousandsSepPos[thousandsSepCtr] + 1)
                                        {
                                            sb.Append(info.NumberGroupSeparator);
                                            thousandsSepCtr--;
                                        }
                                    }
                                    digPos--;
                                    adjust--;
                                }
                                break;
                        }
                    }

                    switch (ch)
                    {
                        case '#':
                        case '0':
                            {
                                if (adjust < 0)
                                {
                                    adjust++;
                                    ch = digPos <= firstDigit ? '0' : '\0';
                                }
                                else
                                {
                                    ch = *cur != 0 ? (char)(*cur++) : digPos > lastDigit ? '0' : '\0';
                                }
                                if (ch != 0)
                                {
                                    sb.Append(ch);
                                    if (thousandSeps && digPos > 1 && thousandsSepCtr >= 0)
                                    {
                                        if (digPos == thousandsSepPos[thousandsSepCtr] + 1)
                                        {
                                            sb.Append(info.NumberGroupSeparator);
                                            thousandsSepCtr--;
                                        }
                                    }
                                }

                                digPos--;
                                break;
                            }
                        case '.':
                            {
                                if (digPos != 0 || decimalWritten)
                                {
                                    // For compatibility, don't echo repeated decimals
                                    break;
                                }
                                // If the format has trailing zeros or the format has a decimal and digits remain
                                if (lastDigit < 0 || (decimalPos < digitCount && *cur != 0))
                                {
                                    sb.Append(info.NumberDecimalSeparator);
                                    decimalWritten = true;
                                }
                                break;
                            }
                        case '\x2030':
                            sb.Append(info.PerMilleSymbol);
                            break;
                        case '%':
                            sb.Append(info.PercentSymbol);
                            break;
                        case ',':
                            break;
                        case '\'':
                        case '"':
                            while (src < format.Length && pFormat[src] != 0 && pFormat[src] != ch)
                                sb.Append(pFormat[src++]);
                            if (src < format.Length && pFormat[src] != 0)
                                src++;
                            break;
                        case '\\':
                            if (src < format.Length && pFormat[src] != 0)
                                sb.Append(pFormat[src++]);
                            break;
                        case 'E':
                        case 'e':
                            {
                                bool positiveSign = false;
                                int i = 0;
                                if (scientific)
                                {
                                    if (src < format.Length && pFormat[src] == '0')
                                    {
                                        // Handles E0, which should format the same as E-0
                                        i++;
                                    }
                                    else if (src + 1 < format.Length && pFormat[src] == '+' && pFormat[src + 1] == '0')
                                    {
                                        // Handles E+0
                                        positiveSign = true;
                                    }
                                    else if (src + 1 < format.Length && pFormat[src] == '-' && pFormat[src + 1] == '0')
                                    {
                                        // Handles E-0
                                        // Do nothing, this is just a place holder s.t. we don't break out of the loop.
                                    }
                                    else
                                    {
                                        sb.Append(ch);
                                        break;
                                    }

                                    while (++src < format.Length && pFormat[src] == '0')
                                        i++;
                                    if (i > 10)
                                        i = 10;

                                    int exp = dig[0] == 0 ? 0 : number.Scale - decimalPos;
                                    FormatExponent(ref sb, info, exp, ch, i, positiveSign);
                                    scientific = false;
                                }
                                else
                                {
                                    sb.Append(ch); // Copy E or e to output
                                    if (src < format.Length)
                                    {
                                        if (pFormat[src] == '+' || pFormat[src] == '-')
                                            sb.Append(pFormat[src++]);
                                        while (src < format.Length && pFormat[src] == '0')
                                            sb.Append(pFormat[src++]);
                                    }
                                }
                                break;
                            }
                        default:
                            sb.Append(ch);
                            break;
                    }
                }
            }

            if (number.IsNegative && (section == 0) && (number.Scale == 0) && (sb.Length > 0))
                sb.Insert(0, info.NegativeSign);
        }

        private static unsafe char ParseFormatSpecifier(ReadOnlySpan<char> format, out int digits)
        {
            char c = default;
            if (format.Length > 0)
            {
                // If the format begins with a symbol, see if it's a standard format
                // with or without a specified number of digits.
                c = format[0];
                if ((uint)(c - 'A') <= 'Z' - 'A' ||
                    (uint)(c - 'a') <= 'z' - 'a')
                {
                    // Fast path for sole symbol, e.g. "D"
                    if (format.Length == 1)
                    {
                        digits = -1;
                        return c;
                    }

                    if (format.Length == 2)
                    {
                        // Fast path for symbol and single digit, e.g. "X4"
                        int d = format[1] - '0';
                        if ((uint)d < 10)
                        {
                            digits = d;
                            return c;
                        }
                    }
                    else if (format.Length == 3)
                    {
                        // Fast path for symbol and double digit, e.g. "F12"
                        int d1 = format[1] - '0', d2 = format[2] - '0';
                        if ((uint)d1 < 10 && (uint)d2 < 10)
                        {
                            digits = d1 * 10 + d2;
                            return c;
                        }
                    }

                    // Fallback for symbol and any length digits.  The digits value must be >= 0 && <= 99,
                    // but it can begin with any number of 0s, and thus we may need to check more than two
                    // digits.  Further, for compat, we need to stop when we hit a null char.
                    int n = 0;
                    int i = 1;
                    while (i < format.Length && (((uint)format[i] - '0') < 10) && n < 10)
                        n = (n * 10) + format[i++] - '0';

                    // If we're at the end of the digits rather than having stopped because we hit something
                    // other than a digit or overflowed, return the standard format info.
                    if (i == format.Length || format[i] == '\0')
                    {
                        digits = n;
                        return c;
                    }
                }
            }

            // Default empty format to be "G"; custom format is signified with '\0'.
            digits = -1;
            return format.Length == 0 || c == '\0' ? // For compat, treat '\0' as the end of the specifier, even if the specifier extends beyond it.
                'G' :
                '\0';
        }

        private static unsafe void RoundNumber(ref NumberBuffer number, int pos)
        {
            byte* dig = number.GetDigitsPointer();

            int i = 0;
            while (i < pos && dig[i] != '\0')
                i++;

            if ((i == pos) && dig[i] >= '5')
            {
                while (i > 0 && dig[i - 1] == '9')
                    i--;

                if (i > 0)
                {
                    dig[i - 1]++;
                }
                else
                {
                    number.Scale++;
                    dig[0] = (byte)('1');
                    i = 1;
                }
            }
            else
            {
                while (i > 0 && dig[i - 1] == '0')
                    i--;
            }

            if (i == 0)
            {
                number.IsNegative = false;
                number.Scale = 0; // Decimals with scale ('0.00') should be rounded.
            }

            dig[i] = (byte)('\0');
            number.DigitsCount = i;
        }
    }
}