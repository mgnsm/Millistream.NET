using System;

namespace Millistream.Streaming
{
    public partial class MarketDataFeed<TCallbackUserData, TStatusCallbackUserData>
    {
        /// <summary>
        /// Injects a previously extracted message to the handle.
        /// </summary>
        /// <param name="ptr">An unmanaged pointer to a message previously extracted using the <see cref="Extract(out ushort, out ulong, out uint)"/> method.</param>
        /// <param name="len">The length of the previously extracted message in bytes.</param>
        /// <returns>1 if no callback function has been registered, 0 if a callback function is registered and -1 on errors.</returns>
        /// <remarks>After calling this method, you should call <see cref="GetNextMessage(out int, out int, out ulong)"/> / <see cref="GetNextMessage(out ushort, out ulong)"/> as usual until it returns <see langword="false"/>. The corresponding native function is mdf_inject.</remarks>
        public unsafe int Inject(IntPtr ptr, uint len) =>
            _nativeImplementation.mdf_inject == default ? -1 : _nativeImplementation.mdf_inject(_feedHandle, ptr, len);

        /// <summary>
        /// Injects a previously extracted message to the handle.
        /// </summary>
        /// <param name="data">The extracted message as a memory span of bytes.</param>
        /// <returns>1 if no callback function has been registered, 0 if a callback function is registered and -1 on errors.</returns>
        /// <remarks>After calling this method, you should call <see cref="GetNextMessage(out int, out int, out ulong)"/> / <see cref="GetNextMessage(out ushort, out ulong)"/> as usual until it returns <see langword="false"/>. The corresponding native function is mdf_inject.</remarks>
        public unsafe int Inject(ReadOnlySpan<byte> data)
        {
            if (_nativeImplementation.mdf_inject == default)
                return -1;

            fixed (byte* bytes = data)
                return _nativeImplementation.mdf_inject(_feedHandle, (IntPtr)bytes, (uint)data.Length);
        }
    }
}