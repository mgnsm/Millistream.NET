using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class MarketDataFeed<TCallbackUserData, TStatusCallbackUserData>
    {
        /// <summary>
        /// Consumes data sent from the server. If there currently is no data the function waits for <paramref name="timeout"/> number of seconds, if <paramref name="timeout"/> is zero (0) the function will return immediately. If <paramref name="timeout"/> is negative then the wait period is treated as number of microseconds instead of number of seconds (i.e. -1000 will wait a maximum of 1000µs).
        /// </summary>
        /// <param name="timeout">The wait period in seconds if positive. If negative, the value is treated as the number of microseconds to wait instead of the number of seconds.</param>
        /// <returns>1 if data has been consumed that needs to be handled by <see cref="GetNextMessage(out ushort, out ulong)" /> and no callback function has been registered. The function returns 0 on timeout or if a callback function is registered and there was data. On errors, -1 will be returned (and the connection will be dropped).</returns>
        /// <remarks>The corresponding native function is mdf_consume.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int Consume(int timeout) => _nativeImplementation.mdf_consume(_feedHandle, timeout);
    }
}