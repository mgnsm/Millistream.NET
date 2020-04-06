using System;
using System.Text;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    internal static class Extensions
    {
        internal static ReadOnlySpan<byte> GetBytes(this string s) => Encoding.UTF8.GetBytes(s);
    }
}