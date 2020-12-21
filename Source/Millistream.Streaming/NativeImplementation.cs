using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming
{
    internal static class NativeImplementation
    {
        private static INativeImplementation _nativeImplementation;

        internal static INativeImplementation Get()
        {
            if (_nativeImplementation != null)
                return _nativeImplementation;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return _nativeImplementation = new NativeWindowsImplementation();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return _nativeImplementation = new NativeLinuxImplementation();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return _nativeImplementation = new NativeMacOsImplementation();

            throw new PlatformNotSupportedException();
        }
    }
}