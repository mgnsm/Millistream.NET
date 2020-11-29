using System;

namespace Millistream.Streaming.DataTypes.Parsing
{
    internal static class Utf8Parser
    {
        internal static bool TryParse(ReadOnlySpan<byte> source, out uint value)
        {
            if (source.Length < 1)
                goto FalseExit;

            int index = 0;
            int num = source[index];
            int answer = 0;

            // Throw away any leading spaces
            const int Space = 32;
            if (num == Space)
            {
                do
                {
                    index++;
                    if (index >= source.Length)
                        goto FalseExit;
                    num = source[index];
                } while (num == Space);
            }

            // Throw away any trailing spaces
            int length = source.Length;
            if (source[length - 1] == Space)
            {
                do
                {
                    length--;
                } while (source[length - 1] == Space && length > 0);
            }

            num -= '0';
            if (ParserHelpers.IsDigit(num))
            {
                if (num == '0')
                {
                    do
                    {
                        index++;
                        if (index >= length)
                            goto Done;
                        num = source[index] - '0';
                    } while (num == '0');
                    if (!ParserHelpers.IsDigit(num))
                        goto FalseExit;
                }

                answer = num;
                index++;

                if (index >= length)
                    goto Done;
                num = source[index] - '0';
                if (!ParserHelpers.IsDigit(num))
                    goto FalseExit;
                index++;
                answer = 10 * answer + num;

                if (index >= length)
                    goto Done;
                num = source[index] - '0';
                if (!ParserHelpers.IsDigit(num))
                    goto FalseExit;
                index++;
                answer = 10 * answer + num;

                if (index >= length)
                    goto Done;
                num = source[index] - '0';
                if (!ParserHelpers.IsDigit(num))
                    goto FalseExit;
                index++;
                answer = 10 * answer + num;

                if (index >= length)
                    goto Done;
                num = source[index] - '0';
                if (!ParserHelpers.IsDigit(num))
                    goto FalseExit;
                index++;
                answer = 10 * answer + num;

                if (index >= length)
                    goto Done;
                num = source[index] - '0';
                if (!ParserHelpers.IsDigit(num))
                    goto FalseExit;
                index++;
                answer = 10 * answer + num;

                if (index >= length)
                    goto Done;
                num = source[index] - '0';
                if (!ParserHelpers.IsDigit(num))
                    goto FalseExit;
                index++;
                answer = 10 * answer + num;

                if (index >= length)
                    goto Done;
                num = source[index] - '0';
                if (!ParserHelpers.IsDigit(num))
                    goto FalseExit;
                index++;
                answer = 10 * answer + num;

                if (index >= length)
                    goto Done;
                num = source[index] - '0';
                if (!ParserHelpers.IsDigit(num))
                    goto FalseExit;
                index++;
                answer = 10 * answer + num;

                // Potential overflow
                if (index >= length)
                    goto Done;
                num = source[index] - '0';
                if (!ParserHelpers.IsDigit(num))
                    goto FalseExit;
                index++;
                if (((uint)answer) > uint.MaxValue / 10 || (((uint)answer) == uint.MaxValue / 10 && num > '5'))
                    goto FalseExit; // Overflow
                answer = answer * 10 + num;

                if (index >= length)
                    goto Done;
                if (!ParserHelpers.IsDigit(source[index]))
                    goto FalseExit;

                // Guaranteed overflow
                goto FalseExit;
            }

        FalseExit:
            value = default;
            return false;

        Done:
            value = (uint)answer;
            return true;
        }
    }
}
