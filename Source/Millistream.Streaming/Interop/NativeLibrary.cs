using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming.Interop
{
    internal abstract class NativeLibrary
    {
        private static NativeLibrary _nativeLibrary;

        public IntPtr Load(string libraryPath)
        {
            if (string.IsNullOrEmpty(libraryPath))
                throw new ArgumentNullException(nameof(libraryPath));

            IntPtr p = DoLoad(libraryPath);
            if (p == IntPtr.Zero)
                throw new DllNotFoundException();

            return p;
        }

        public IntPtr GetExport(IntPtr handle, string name)
        {
            if (!TryGetExport(handle, name, out IntPtr address))
                throw new DllNotFoundException();

            return address;
        }

        public bool TryGetExport(IntPtr handle, string name, out IntPtr address)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            address = handle != IntPtr.Zero ? DoGetExport(handle, name) : IntPtr.Zero;
            return address != IntPtr.Zero;
        }

        public static NativeLibrary GetDefault()
        {
            if (_nativeLibrary != null)
                return _nativeLibrary;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return _nativeLibrary = new NativeUnixLibrary();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return _nativeLibrary = new NativeWindowsLibrary();

            throw new PlatformNotSupportedException();
        }

        protected abstract IntPtr DoLoad(string libraryPath);
        protected abstract IntPtr DoGetExport(IntPtr handle, string name);
    }
}