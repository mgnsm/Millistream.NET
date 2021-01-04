using System;

namespace Millistream.Streaming.UnitTests
{
    internal delegate void GetUInt64PropertyCallback(IntPtr handle, MDF_OPTION option, ref ulong value);
}