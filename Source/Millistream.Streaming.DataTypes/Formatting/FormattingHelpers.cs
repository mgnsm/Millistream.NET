using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming.DataTypes.Formatting
{
    internal static class FormattingHelpers
    {
        internal static int CountDigits(BigInteger bigInteger)
        {
            if (bigInteger < BigInteger.Zero)
                bigInteger = BigInteger.Abs(bigInteger);

            if (bigInteger <= uint.MaxValue)
                return CountDigits((uint)bigInteger);
            else if (bigInteger <= ulong.MaxValue)
                return CountDigits((ulong)bigInteger);

            int precision = 18;
            do
            {
                bigInteger /= 10;
                if (precision == int.MaxValue)
                    throw new InvalidOperationException();
                precision++;
            }
            while (bigInteger >= 1000000000000000000);

            return precision;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CountDigits(ulong value)
        {
            int digits = 1;
            uint part;
            if (value >= 10000000)
            {
                if (value >= 100000000000000)
                {
                    part = (uint)(value / 100000000000000);
                    digits += 14;
                }
                else
                {
                    part = (uint)(value / 10000000);
                    digits += 7;
                }
            }
            else
            {
                part = (uint)value;
            }

            if (part < 10)
            {
                // no-op
            }
            else if (part < 100)
                digits++;
            else if (part < 1000)
                digits += 2;
            else if (part < 10000)
                digits += 3;
            else if (part < 100000)
                digits += 4;
            else if (part < 1000000)
                digits += 5;
            else
                digits += 6;

            return digits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CountDigits(uint value)
        {
            int digits = 1;
            if (value >= 100000)
            {
                value /= 100000;
                digits += 5;
            }

            if (value < 10)
            {
                // no-op
            }
            else if (value < 100)
                digits++;
            else if (value < 1000)
                digits += 2;
            else if (value < 10000)
                digits += 3;
            else
                digits += 4;

            return digits;
        }
    }
}