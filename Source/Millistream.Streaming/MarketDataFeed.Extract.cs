using System;

namespace Millistream.Streaming
{
    public partial class MarketDataFeed<TCallbackUserData, TStatusCallbackUserData>
    {
        /// <summary>
        /// Extracts the current message from the handle.
        /// </summary>
        /// <param name="mref">The message reference of the extracted message. This should match a <see cref="MessageReferences"/> value.</param>
        /// <param name="insref">The unique instrument reference of the extracted message.</param>
        /// <param name="len">The length of the extracted message in bytes.</param>
        /// <returns>An unmanaged pointer to the extracted message if there was a message to return or <see langword="default(IntPtr)"/> if there are no more messages in the stream.</returns>
        /// <remarks>The caller must make a copy of the returned data since the pointer is to the internal state of the handle. The corresponding native function is mdf_extract.</remarks>
        public unsafe IntPtr Extract(out ushort mref, out ulong insref, out uint len)
        {
            mref = default;
            insref = default;
            len = default;
            return _nativeImplementation.mdf_extract == default ? default 
                : _nativeImplementation.mdf_extract(_feedHandle, ref mref, ref insref, ref len);
        }
    }
}