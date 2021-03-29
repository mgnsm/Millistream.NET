using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void mdf_status_callback(IntPtr userdata, ConnectionStatus status, IntPtr host, IntPtr ip);
}