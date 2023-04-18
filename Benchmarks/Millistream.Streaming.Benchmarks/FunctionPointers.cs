using Millistream.Streaming.Interop;
using System;
using System.Net;
using System.Runtime.InteropServices;
#if NETCOREAPP
using nativeLibrary = System.Runtime.InteropServices.NativeLibrary;
#endif
namespace Millistream.Streaming.Benchmarks
{
    internal static unsafe class FunctionPointers
    {
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr> mdf_create;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, void> mdf_destroy;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, ref int, ref int, ref ulong, int> mdf_get_next_message;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, ref uint, ref IntPtr, int> mdf_get_next_field;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, IntPtr, int> mdf_set_property;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, string, int> mdf_connect;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, void> mdf_disconnect;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, int, int> mdf_consume;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr> mdf_message_create;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, ulong, int, int> mdf_message_add;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, int> mdf_message_del;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, void> mdf_message_reset;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, void> mdf_message_destroy;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> mdf_message_add_list;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, string, int> mdf_message_add_list_str;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> mdf_message_add_numeric;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, string, int> mdf_message_add_numeric_str;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, ulong, int, int> mdf_message_add_uint;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, long, int, int> mdf_message_add_int;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> mdf_message_add_string;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, string, int> mdf_message_add_string_str;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int, int> mdf_message_add_string2;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, string, int, int> mdf_message_add_string2_str;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> mdf_message_add_date;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, string, int> mdf_message_add_date_str;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int> mdf_message_add_date2;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> mdf_message_add_time;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, string, int> mdf_message_add_time_str;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int> mdf_message_add_time2;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int> mdf_message_add_time3;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, int> mdf_message_get_num;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, int> mdf_message_get_num_active;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, int> mdf_message_get_num_fields;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int> mdf_message_send; 
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, MDF_MSG_OPTION, int, int> mdf_message_set_property;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ulong, ulong, int> mdf_message_move;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, ref IntPtr, int> mdf_message_serialize;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int> mdf_message_deserialize;
        internal static readonly delegate* unmanaged[Cdecl]<IntPtr, string, int> mdf_message_deserialize_str;

        static FunctionPointers()
        {
#if NETFRAMEWORK
            NativeLibrary nativeLibrary = new NativeWindowsLibrary();
#endif
            IntPtr lib = nativeLibrary.Load(DllImports.DllName);
            mdf_create = (delegate* unmanaged[Cdecl]<IntPtr>)nativeLibrary.GetExport(lib, nameof(mdf_create));
            mdf_destroy = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_destroy));
            mdf_get_next_message = (delegate* unmanaged[Cdecl]<IntPtr, ref int, ref int, ref ulong, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_next_message));
            mdf_get_next_field = (delegate* unmanaged[Cdecl]<IntPtr, ref uint, ref IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_next_field));
            mdf_set_property = (delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_set_property));
            mdf_connect = (delegate* unmanaged[Cdecl]<IntPtr, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_connect));
            mdf_disconnect = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_disconnect));
            mdf_consume = (delegate* unmanaged[Cdecl]<IntPtr, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_consume));
            mdf_message_create = (delegate* unmanaged[Cdecl]<IntPtr>)nativeLibrary.GetExport(lib, nameof(mdf_message_create));
            mdf_message_add = (delegate* unmanaged[Cdecl]<IntPtr, ulong, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add));
            mdf_message_del = (delegate* unmanaged[Cdecl]<IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_del));
            mdf_message_reset = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_message_reset));
            mdf_message_destroy = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_message_destroy));
            mdf_message_add_list = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_list));
            mdf_message_add_list_str = (delegate* unmanaged[Cdecl]<IntPtr, uint, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_list));
            mdf_message_add_numeric = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_numeric));
            mdf_message_add_numeric_str = (delegate* unmanaged[Cdecl]<IntPtr, uint, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_numeric));
            mdf_message_add_uint = (delegate* unmanaged[Cdecl]<IntPtr, uint, ulong, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_uint));
            mdf_message_add_int = (delegate* unmanaged[Cdecl]<IntPtr, uint, long, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_uint));
            mdf_message_add_string = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_string));
            mdf_message_add_string_str = (delegate* unmanaged[Cdecl]<IntPtr, uint, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_string));
            mdf_message_add_string2 = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_string2));
            mdf_message_add_string2_str = (delegate* unmanaged[Cdecl]<IntPtr, uint, string, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_string2));
            mdf_message_add_date = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_date));
            mdf_message_add_date_str = (delegate* unmanaged[Cdecl]<IntPtr, uint, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_date));
            mdf_message_add_date2 = (delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_date2));
            mdf_message_add_time = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_time));
            mdf_message_add_time_str = (delegate* unmanaged[Cdecl]<IntPtr, uint, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_time));
            mdf_message_add_time2 = (delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_time2));
            mdf_message_add_time3 = (delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_time3));
            mdf_message_get_num = (delegate* unmanaged[Cdecl]<IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_get_num));
            mdf_message_get_num_active = (delegate* unmanaged[Cdecl]<IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_get_num_active));
            mdf_message_get_num_fields = (delegate* unmanaged[Cdecl]<IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_get_num_fields));
            mdf_message_send = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_send));
            mdf_message_set_property = (delegate* unmanaged[Cdecl]<IntPtr, MDF_MSG_OPTION, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_set_property));
            mdf_message_move = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ulong, ulong, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_move));
            mdf_message_serialize = (delegate* unmanaged[Cdecl]<IntPtr, ref IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_serialize));
            mdf_message_deserialize = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_deserialize));
            mdf_message_deserialize_str = (delegate* unmanaged[Cdecl]<IntPtr, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_deserialize));
        }
    }
}