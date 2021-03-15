using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming.Interop
{
    internal sealed class NativeUnixLibrary : NativeLibrary
    {
        private const string DllName = "libdl";
        private const int RTLD_NOW = 0x002;

#pragma warning disable IDE1006
        [DllImport(DllName, ExactSpelling = true)]
        public static extern IntPtr dlopen(string filename, int flags);

        [DllImport(DllName, ExactSpelling = true)]
        public static extern IntPtr dlsym(IntPtr handle, string symbol);
#pragma warning restore IDE1006

        protected override IntPtr DoGetExport(IntPtr handle, string name) =>
            dlsym(handle, name);

        protected override IntPtr DoLoad(string libraryPath) => 
            dlopen(libraryPath, RTLD_NOW);
    }
}
