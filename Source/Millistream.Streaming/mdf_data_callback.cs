using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void mdf_data_callback(IntPtr userdata, IntPtr handle);
}