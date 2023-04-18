using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming.Interop
{
    internal sealed class NativeUnixLibrary : NativeLibrary
    {
        private const string DllName = "libdl";
        private const int RTLD_NOW = 0x002;

        [DllImport(DllName, ExactSpelling = true)]
        private static extern IntPtr dlopen(string filename, int flags);

        [DllImport(DllName, ExactSpelling = true)]
        private static extern IntPtr dlsym(IntPtr handle, string symbol);

        protected override IntPtr DoGetExport(IntPtr handle, string name) =>
            dlsym(handle, name);

        protected override IntPtr DoLoad(string libraryPath) =>
            dlopen(libraryPath, RTLD_NOW);
    }
}
