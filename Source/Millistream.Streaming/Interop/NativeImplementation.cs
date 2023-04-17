using System;
using System.Runtime.InteropServices;
#if NETCOREAPP3_0_OR_GREATER
using nativeLibrary = System.Runtime.InteropServices.NativeLibrary;
#endif
namespace Millistream.Streaming.Interop
{
    unsafe internal sealed class NativeImplementation
    {
        private static NativeImplementation s_defaultImplementation;

        internal readonly delegate* unmanaged[Cdecl]<IntPtr> mdf_create;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, void> mdf_destroy;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, int, int> mdf_consume;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, ref int, ref int, ref ulong, int> mdf_get_next_message;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, ref ushort, ref ulong, int> mdf_get_next_message2;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, ref uint, ref IntPtr, int> mdf_get_next_field;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, ref IntPtr, int> mdf_get_property;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, ref int, int> mdf_get_int_property;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, ref ulong, int> mdf_get_ulong_property;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, ref long, int> mdf_get_long_property;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, IntPtr, int> mdf_set_property;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, byte> mdf_get_delay;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, ulong> mdf_get_mclass;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, string, int> mdf_connect;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, void> mdf_disconnect;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr> mdf_message_create;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, void> mdf_message_destroy;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, void> mdf_message_reset;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, int> mdf_message_del;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, ulong, int, int> mdf_message_add;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> mdf_message_add_numeric;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, string, int> mdf_message_add_numeric_str;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, long, int, int> mdf_message_add_int;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, ulong, int, int> mdf_message_add_uint;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> mdf_message_add_string;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int, int> mdf_message_add_string2;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> mdf_message_add_date;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, string, int> mdf_message_add_date_str;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int> mdf_message_add_date2;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> mdf_message_add_time;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, string, int> mdf_message_add_time_str;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int> mdf_message_add_time2;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int> mdf_message_add_time3;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> mdf_message_add_list;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, uint, string, int> mdf_message_add_list_str;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int> mdf_message_send;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, int> mdf_message_get_num;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, int> mdf_message_get_num_active;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ulong, ulong, int> mdf_message_move;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, ref IntPtr, int> mdf_message_serialize;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int> mdf_message_deserialize;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, string, int> mdf_message_deserialize_str;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, MDF_MSG_OPTION, int, int> mdf_message_set_property;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, int> mdf_message_get_num_fields;
        // Removed in libmdf-1.0.26:
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, int, int> mdf_message_set_compression_level;
        internal readonly delegate* unmanaged[Cdecl]<IntPtr, int, int> mdf_message_set_utf8_validation;

        internal NativeImplementation(string libraryPath)
        {
#if !NETCOREAPP3_0_OR_GREATER
            NativeLibrary nativeLibrary;
#endif
            IntPtr lib;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
#if !NETCOREAPP3_0_OR_GREATER
                nativeLibrary = new NativeUnixLibrary();
#endif
                lib = nativeLibrary.Load(libraryPath ?? "libmdf.so.0");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
#if !NETCOREAPP3_0_OR_GREATER
                nativeLibrary = new NativeWindowsLibrary();
#endif
                lib = nativeLibrary.Load(libraryPath ?? "libmdf-0.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
#if !NETCOREAPP3_0_OR_GREATER
                nativeLibrary = new NativeUnixLibrary();
#endif
                lib = nativeLibrary.Load(libraryPath ?? "libmdf.0.dylib");
            }
            else
                throw new PlatformNotSupportedException();


            mdf_create = (delegate* unmanaged[Cdecl]<IntPtr>)nativeLibrary.GetExport(lib, nameof(mdf_create));
            mdf_destroy = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_destroy));
            mdf_consume = (delegate* unmanaged[Cdecl]<IntPtr, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_consume));
            mdf_get_next_message = (delegate* unmanaged[Cdecl]<IntPtr, ref int, ref int, ref ulong, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_next_message));
            mdf_get_next_field = (delegate* unmanaged[Cdecl]<IntPtr, ref uint, ref IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_next_field));
            mdf_get_property = (delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, ref IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_property));
            mdf_get_int_property = (delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, ref int, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_property));
            mdf_get_ulong_property = (delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, ref ulong, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_property));
            mdf_get_long_property = (delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, ref long, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_property));
            mdf_set_property = (delegate* unmanaged[Cdecl]<IntPtr, MDF_OPTION, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_set_property));
            mdf_connect = (delegate* unmanaged[Cdecl]<IntPtr, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_connect));
            mdf_disconnect = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_disconnect));
            mdf_message_create = (delegate* unmanaged[Cdecl]<IntPtr>)nativeLibrary.GetExport(lib, nameof(mdf_message_create));
            mdf_message_destroy = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_message_destroy));
            mdf_message_reset = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_message_reset));
            mdf_message_del = (delegate* unmanaged[Cdecl]<IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_del));
            mdf_message_add = (delegate* unmanaged[Cdecl]<IntPtr, ulong, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add));
            mdf_message_add_numeric = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_numeric));
            mdf_message_add_numeric_str = (delegate* unmanaged[Cdecl]<IntPtr, uint, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_numeric));
            mdf_message_add_string = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_string));
            mdf_message_add_date = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_date));
            mdf_message_add_date_str = (delegate* unmanaged[Cdecl]<IntPtr, uint, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_date));
            mdf_message_add_time = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_time));
            mdf_message_add_time_str = (delegate* unmanaged[Cdecl]<IntPtr, uint, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_time));
            mdf_message_add_list = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_list));
            mdf_message_add_list_str = (delegate* unmanaged[Cdecl]<IntPtr, uint, string, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_list));
            mdf_message_send = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_send));
            mdf_message_get_num = (delegate* unmanaged[Cdecl]<IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_get_num));
            mdf_message_get_num_active = (delegate* unmanaged[Cdecl]<IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_get_num_active));

            if (nativeLibrary.TryGetExport(lib, nameof(mdf_get_next_message2), out IntPtr address))
                mdf_get_next_message2 = (delegate* unmanaged[Cdecl]<IntPtr, ref ushort, ref ulong, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_get_delay), out address))
                mdf_get_delay = (delegate* unmanaged[Cdecl]<IntPtr, byte>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_get_mclass), out address))
                mdf_get_mclass = (delegate* unmanaged[Cdecl]<IntPtr, ulong>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_int), out address))
                mdf_message_add_int = (delegate* unmanaged[Cdecl]<IntPtr, uint, long, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_uint), out address))
                mdf_message_add_uint = (delegate* unmanaged[Cdecl]<IntPtr, uint, ulong, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_string2), out address))
                mdf_message_add_string2 = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_date2), out address))
                mdf_message_add_date2 = (delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_time2), out address))
                mdf_message_add_time2 = (delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_time3), out address))
                mdf_message_add_time3 = (delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_move), out address))
                mdf_message_move = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ulong, ulong, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_serialize), out address))
                mdf_message_serialize = (delegate* unmanaged[Cdecl]<IntPtr, ref IntPtr, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_deserialize), out address))
            {
                mdf_message_deserialize = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int>)address;
                mdf_message_deserialize_str = (delegate* unmanaged[Cdecl]<IntPtr, string, int>)address;
            }
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_set_property), out address))
                mdf_message_set_property = (delegate* unmanaged[Cdecl]<IntPtr, MDF_MSG_OPTION, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_get_num_fields), out address))
                mdf_message_get_num_fields = (delegate* unmanaged[Cdecl]<IntPtr, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_set_compression_level), out address))
                mdf_message_set_compression_level = (delegate* unmanaged[Cdecl]<IntPtr, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_set_utf8_validation), out address))
                mdf_message_set_utf8_validation = (delegate* unmanaged[Cdecl]<IntPtr, int, int>)address;
        }

        internal static NativeImplementation Default => s_defaultImplementation ??= new(null);
    }
}