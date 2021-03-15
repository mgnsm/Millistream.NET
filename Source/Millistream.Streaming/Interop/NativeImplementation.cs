using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming.Interop
{
    unsafe internal sealed class NativeImplementation : INativeImplementation
    {
        private static readonly NativeImplementation s_instance = new NativeImplementation();
        private static readonly delegate* unmanaged[Cdecl]<IntPtr> s_mdf_create;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, void> s_mdf_destroy;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, int, int> s_mdf_consume;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, ref int, ref int, ref ulong, int> s_mdf_get_next_message;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, ref uint, ref IntPtr, int> s_mdf_get_next_field;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, int, ref IntPtr, int> s_mdf_get_property;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, int, ref int, int> s_mdf_get_int_property;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, int, ref ulong, int> s_mdf_get_ulong_property;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, int, ref long, int> s_mdf_get_long_property;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, int> s_mdf_set_property;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int> s_mdf_connect;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, void> s_mdf_disconnect;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr> s_mdf_message_create;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, void> s_mdf_message_destroy;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, int, int> s_mdf_message_set_compression_level;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, void> s_mdf_message_reset;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, int> s_mdf_message_del;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, ulong, int, int> s_mdf_message_add;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> s_mdf_message_add_numeric;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, long, int, int> s_mdf_message_add_int;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, ulong, int, int> s_mdf_message_add_uint;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> s_mdf_message_add_string;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int, int> s_mdf_message_add_string2;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> s_mdf_message_add_date;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int> s_mdf_message_add_date2;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> s_mdf_message_add_time;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int> s_mdf_message_add_time2;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int> s_mdf_message_add_time3;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int> s_mdf_message_add_list;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int> s_mdf_message_send;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, int> s_mdf_message_get_num;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, int> s_mdf_message_get_num_active;      
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ulong, ulong, int> s_mdf_message_move;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, ref IntPtr, int> s_mdf_message_serialize;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int> s_mdf_message_deserialize;
        private static readonly delegate* unmanaged[Cdecl]<IntPtr, int, int> s_mdf_message_set_utf8_validation;

        static NativeImplementation()
        {
            NativeLibrary nativeLibrary = default;
            IntPtr lib = IntPtr.Zero;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                nativeLibrary = new NativeUnixLibrary();
                lib = nativeLibrary.Load("libmdf.so.0");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                nativeLibrary = new NativeWindowsLibrary();
                lib = nativeLibrary.Load("libmdf-0.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                nativeLibrary = new NativeUnixLibrary();
                lib = nativeLibrary.Load("libmdf.0.dylib");
            }
            else
                throw new PlatformNotSupportedException();


            s_mdf_create = (delegate* unmanaged[Cdecl]<IntPtr>)nativeLibrary.GetExport(lib, nameof(mdf_create));
            s_mdf_destroy = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_destroy));
            s_mdf_consume = (delegate* unmanaged[Cdecl]<IntPtr, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_consume));
            s_mdf_get_next_message = (delegate* unmanaged[Cdecl]<IntPtr, ref int, ref int, ref ulong, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_next_message));
            s_mdf_get_next_field = (delegate* unmanaged[Cdecl]<IntPtr, ref uint, ref IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_next_field));
            s_mdf_get_property = (delegate* unmanaged[Cdecl]<IntPtr, int, ref IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_property));
            s_mdf_get_int_property = (delegate* unmanaged[Cdecl]<IntPtr, int, ref int, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_property));
            s_mdf_get_ulong_property = (delegate* unmanaged[Cdecl]<IntPtr, int, ref ulong, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_property));
            s_mdf_get_long_property = (delegate* unmanaged[Cdecl]<IntPtr, int, ref long, int>)nativeLibrary.GetExport(lib, nameof(mdf_get_property));
            s_mdf_set_property = (delegate* unmanaged[Cdecl]<IntPtr, int, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_set_property));
            s_mdf_connect = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_connect));
            s_mdf_disconnect = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_disconnect));
            s_mdf_message_create = (delegate* unmanaged[Cdecl]<IntPtr>)nativeLibrary.GetExport(lib, nameof(mdf_message_create));
            s_mdf_message_destroy = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_message_destroy));
            s_mdf_message_reset = (delegate* unmanaged[Cdecl]<IntPtr, void>)nativeLibrary.GetExport(lib, nameof(mdf_message_reset));
            s_mdf_message_del = (delegate* unmanaged[Cdecl]<IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_del));
            s_mdf_message_add = (delegate* unmanaged[Cdecl]<IntPtr, ulong, int, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add));
            s_mdf_message_add_numeric = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_numeric));
            s_mdf_message_add_string = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_string));
            s_mdf_message_add_date = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_date));
            s_mdf_message_add_time = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_time));
            s_mdf_message_add_list = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_add_list));
            s_mdf_message_send = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_send));
            s_mdf_message_get_num = (delegate* unmanaged[Cdecl]<IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_get_num));
            s_mdf_message_get_num_active = (delegate* unmanaged[Cdecl]<IntPtr, int>)nativeLibrary.GetExport(lib, nameof(mdf_message_get_num_active));
            
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_set_compression_level), out IntPtr address))
                s_mdf_message_set_compression_level = (delegate* unmanaged[Cdecl]<IntPtr, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_int), out address))
                s_mdf_message_add_int = (delegate* unmanaged[Cdecl]<IntPtr, uint, long, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_uint), out address))
                s_mdf_message_add_uint = (delegate* unmanaged[Cdecl]<IntPtr, uint, ulong, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_string2), out address))
                s_mdf_message_add_string2 = (delegate* unmanaged[Cdecl]<IntPtr, uint, IntPtr, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_date2), out address))
                s_mdf_message_add_date2 = (delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_time2), out address))
                s_mdf_message_add_time2 = (delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_add_time3), out address))
                s_mdf_message_add_time3 = (delegate* unmanaged[Cdecl]<IntPtr, uint, int, int, int, int, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_move), out address))
                s_mdf_message_move = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ulong, ulong, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_serialize), out address))
                s_mdf_message_serialize = (delegate* unmanaged[Cdecl]<IntPtr, ref IntPtr, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_deserialize), out address))
                s_mdf_message_deserialize = (delegate* unmanaged[Cdecl]<IntPtr, IntPtr, int>)address;
            if (nativeLibrary.TryGetExport(lib, nameof(mdf_message_set_utf8_validation), out address))
                s_mdf_message_set_utf8_validation = (delegate* unmanaged[Cdecl]<IntPtr, int, int>)address;
        }

        private NativeImplementation() { }

        public static NativeImplementation Instance => s_instance;

        public IntPtr mdf_create() => s_mdf_create();
        public void mdf_destroy(IntPtr handle) => s_mdf_destroy(handle);
        public int mdf_consume(IntPtr handle, int timeout) => s_mdf_consume(handle, timeout);
        public int mdf_get_next_message(IntPtr handle, ref int mref, ref int mclass, ref ulong insref) => s_mdf_get_next_message(handle, ref mref, ref mclass, ref insref);
        public int mdf_get_next_field(IntPtr handle, ref uint tag, ref IntPtr value) => s_mdf_get_next_field(handle, ref tag, ref value);
        public int mdf_get_property(IntPtr handle, MDF_OPTION option, ref IntPtr value) => s_mdf_get_property(handle, (int)option, ref value);
        public int mdf_get_property(IntPtr handle, MDF_OPTION option, ref int value) => s_mdf_get_int_property(handle, (int)option, ref value);
        public int mdf_get_property(IntPtr handle, MDF_OPTION option, ref ulong value) => s_mdf_get_ulong_property(handle, (int)option, ref value);
        public int mdf_get_property(IntPtr handle, MDF_OPTION option, ref long value) => s_mdf_get_long_property(handle, (int)option, ref value);
        public int mdf_set_property(IntPtr handle, MDF_OPTION option, IntPtr value) => s_mdf_set_property(handle, (int)option, value);
        public int mdf_connect(IntPtr handle, IntPtr server) => s_mdf_connect(handle, server);
        public void mdf_disconnect(IntPtr handle) => s_mdf_disconnect(handle);
        public IntPtr mdf_message_create() => s_mdf_message_create();
        public void mdf_message_destroy(IntPtr message) => s_mdf_message_destroy(message);        
        public void mdf_message_reset(IntPtr message) => s_mdf_message_reset(message);
        public int mdf_message_del(IntPtr message) => s_mdf_message_del(message);
        public int mdf_message_add(IntPtr message, ulong instrument_reference, int message_reference) => s_mdf_message_add(message, instrument_reference, message_reference);
        public int mdf_message_add_numeric(IntPtr message, uint tag, IntPtr value) => s_mdf_message_add_numeric(message, tag, value);
        public int mdf_message_add_string(IntPtr message, uint tag, IntPtr value) => s_mdf_message_add_string(message, tag, value);
        public int mdf_message_add_date(IntPtr message, uint tag, IntPtr value) => s_mdf_message_add_date(message, tag, value);
        public int mdf_message_add_time(IntPtr message, uint tag, IntPtr value) => s_mdf_message_add_time(message, tag, value);
        public int mdf_message_add_list(IntPtr message, uint tag, IntPtr value) => s_mdf_message_add_list(message, tag, value);
        public int mdf_message_send(IntPtr handle, IntPtr message) => s_mdf_message_send(handle, message);
        public int mdf_message_get_num(IntPtr message) => s_mdf_message_get_num(message);
        public int mdf_message_get_num_active(IntPtr message) => s_mdf_message_get_num_active(message);

        public int mdf_message_set_compression_level(IntPtr message, int level) =>
            s_mdf_message_set_compression_level == null ? throw new InvalidOperationException() :
            s_mdf_message_set_compression_level(message, level);

        public int mdf_message_add_int(IntPtr message, uint tag, long value, int decimals) =>
            s_mdf_message_add_int == null ? throw new InvalidOperationException() :
            s_mdf_message_add_int(message, tag, value, decimals);

        public int mdf_message_add_uint(IntPtr message, uint tag, ulong value, int decimals) =>
            s_mdf_message_add_uint == null ? throw new InvalidOperationException() :
            s_mdf_message_add_uint(message, tag, value, decimals);

        public int mdf_message_add_string2(IntPtr message, uint tag, IntPtr value, int len) =>
            s_mdf_message_add_string2 == null ? throw new InvalidOperationException() :
            s_mdf_message_add_string2(message, tag, value, len);

        public int mdf_message_add_date2(IntPtr message, uint tag, int year, int mon, int day) =>
            s_mdf_message_add_date2 == null ? throw new InvalidOperationException() :
            s_mdf_message_add_date2(message, tag, year, mon, day);

        public int mdf_message_add_time2(IntPtr message, uint tag, int hour, int min, int sec, int msec) =>
            s_mdf_message_add_time2 == null ? throw new InvalidOperationException() :
            s_mdf_message_add_time2(message, tag, hour, min, sec, msec);
        
        public int mdf_message_add_time3(IntPtr message, uint tag, int hour, int min, int sec, int nsec) =>
            s_mdf_message_add_time3 == null ? throw new InvalidOperationException() :
            s_mdf_message_add_time3(message, tag, hour, min, sec, nsec);

        public int mdf_message_move(IntPtr src, IntPtr dst, ulong insref_src, ulong insref_dst) =>
            s_mdf_message_move == null ? throw new InvalidOperationException() :
            s_mdf_message_move(src, dst, insref_src, insref_dst);
        
        public int mdf_message_serialize(IntPtr message, ref IntPtr result) =>
            s_mdf_message_serialize == null ? throw new InvalidOperationException() :
            s_mdf_message_serialize(message, ref result);
        
        public int mdf_message_deserialize(IntPtr message, IntPtr data) =>
            s_mdf_message_deserialize == null ? throw new InvalidOperationException() :
            s_mdf_message_deserialize(message, data);

        public int mdf_message_set_utf8_validation(IntPtr message, int enable) =>
            s_mdf_message_set_utf8_validation == null ? throw new InvalidOperationException() :
            s_mdf_message_set_utf8_validation(message, enable);
    }
}