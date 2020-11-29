using System.Runtime.CompilerServices;

namespace Millistream.Streaming.DataTypes.Parsing
{
    internal static class ParserHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsDigit(int digit) => digit > -1 && digit < 10;
    }
}