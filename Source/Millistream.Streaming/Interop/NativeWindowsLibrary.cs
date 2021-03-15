using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming.Interop
{
    internal sealed class NativeWindowsLibrary : NativeLibrary
    {
        private const string DllName = "kernel32";

        [DllImport(DllName, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(DllName, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        protected override IntPtr DoGetExport(IntPtr handle, string name) =>
            GetProcAddress(handle, name);

        protected override IntPtr DoLoad(string libraryPath) =>
            LoadLibrary(libraryPath);
    }
}