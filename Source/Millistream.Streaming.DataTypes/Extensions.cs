using System;
namespace Millistream.Streaming.DataTypes
{
    internal static class Extensions
    {
        internal static int IndexOf(this ReadOnlySpan<char> span, char value, char escape)
        {
            for (int i = 0; i < span.Length; i++)
                if (span[i] == value 
                    && (i == 0 || span[i - 1] != escape || (i > 1 && span[i - 2] == escape)))
                    return i;
            return -1;
        }
    }
}