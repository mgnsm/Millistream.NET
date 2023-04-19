using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class MarketDataFeed<TCallbackUserData, TStatusCallbackUserData>
    {
        /// <summary>
        /// Disconnect a connected API handle. Safe to call even if the handle is already disconnected.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_disconnect.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Disconnect() => _nativeImplementation.mdf_disconnect(_feedHandle);
    }
}