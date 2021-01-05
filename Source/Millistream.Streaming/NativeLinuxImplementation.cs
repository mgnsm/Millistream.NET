using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming
{
    internal class NativeLinuxImplementation : INativeImplementation
    {
        private static class NativeLinuxMethods
        {
            private const string DllName = "libmdf.so.0";

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr mdf_create();

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void mdf_destroy(IntPtr handle);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_get_next_message(IntPtr handle, ref int message, ref int message_class, ref ulong instrument);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_get_next_field(IntPtr handle, ref uint tag, ref IntPtr value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_get_property(IntPtr handle, MDF_OPTION option, ref IntPtr value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_get_property(IntPtr handle, MDF_OPTION option, ref int value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_get_property(IntPtr handle, MDF_OPTION option, ref ulong value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_get_property(IntPtr handle, MDF_OPTION option, ref long value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_set_property(IntPtr handle, MDF_OPTION option, IntPtr value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_set_property(IntPtr handle, MDF_OPTION option, string value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_connect(IntPtr handle, string server);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void mdf_disconnect(IntPtr handle);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_consume(IntPtr handle, int timeout);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr mdf_message_create();

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add(IntPtr message, ulong instrument_reference, int message_reference);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_del(IntPtr message);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void mdf_message_reset(IntPtr message);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void mdf_message_destroy(IntPtr message);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add_list(IntPtr message, uint tag, string value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add_numeric(IntPtr message, uint tag, string value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add_uint(IntPtr message, uint tag, ulong value, int decimals);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add_int(IntPtr message, uint tag, long value, int decimals);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add_string(IntPtr message, uint tag, string value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add_date(IntPtr message, uint tag, string value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add_date2(IntPtr message, uint tag, int year, int mon, int day);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add_time(IntPtr message, uint tag, string value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add_time2(IntPtr message, uint tag, int hour, int min, int sec, int msec);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add_time3(IntPtr message, uint tag, int hour, int min, int sec, int nsec);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_get_num(IntPtr message);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_get_num_active(IntPtr message);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_send(IntPtr handle, IntPtr message);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_set_compression_level(IntPtr message, int level);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_move(IntPtr src, IntPtr dst, ulong insref_src, ulong insref_dst);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_serialize(IntPtr message, ref IntPtr result);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_deserialize(IntPtr message, string data);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_set_utf8_validation(IntPtr message, int enable);
        }

        public int mdf_connect(IntPtr handle, string server) => NativeLinuxMethods.mdf_connect(handle, server);
        public int mdf_consume(IntPtr handle, int timeout) => NativeLinuxMethods.mdf_consume(handle, timeout);
        public IntPtr mdf_create() => NativeLinuxMethods.mdf_create();
        public void mdf_destroy(IntPtr handle) => NativeLinuxMethods.mdf_destroy(handle);
        public void mdf_disconnect(IntPtr handle) => NativeLinuxMethods.mdf_disconnect(handle);
        public int mdf_get_next_field(IntPtr handle, ref uint tag, ref IntPtr value) => NativeLinuxMethods.mdf_get_next_field(handle, ref tag, ref value);
        public int mdf_get_next_message(IntPtr handle, ref int message, ref int message_class, ref ulong instrument) => NativeLinuxMethods.mdf_get_next_message(handle, ref message, ref message_class, ref instrument);
        public int mdf_get_property(IntPtr handle, MDF_OPTION option, ref IntPtr value) => NativeLinuxMethods.mdf_get_property(handle, option, ref value);
        public int mdf_get_property(IntPtr handle, MDF_OPTION option, ref int value) => NativeLinuxMethods.mdf_get_property(handle, option, ref value);
        public int mdf_get_property(IntPtr handle, MDF_OPTION option, ref ulong value) => NativeLinuxMethods.mdf_get_property(handle, option, ref value);
        public int mdf_get_property(IntPtr handle, MDF_OPTION option, ref long value) => NativeLinuxMethods.mdf_get_property(handle, option, ref value);
        public int mdf_message_add(IntPtr message, ulong instrument_reference, int message_reference) => NativeLinuxMethods.mdf_message_add(message, instrument_reference, message_reference);
        public int mdf_message_add_date(IntPtr message, uint tag, string value) => NativeLinuxMethods.mdf_message_add_date(message, tag, value);
        public int mdf_message_add_date2(IntPtr message, uint tag, int year, int mon, int day) => NativeLinuxMethods.mdf_message_add_date2(message, tag, year, mon, day);
        public int mdf_message_add_int(IntPtr message, uint tag, long value, int decimals) => NativeLinuxMethods.mdf_message_add_int(message, tag, value, decimals);
        public int mdf_message_add_list(IntPtr message, uint tag, string value) => NativeLinuxMethods.mdf_message_add_list(message, tag, value);
        public int mdf_message_add_numeric(IntPtr message, uint tag, string value) => NativeLinuxMethods.mdf_message_add_numeric(message, tag, value);
        public int mdf_message_add_string(IntPtr message, uint tag, string value) => NativeLinuxMethods.mdf_message_add_string(message, tag, value);
        public int mdf_message_add_time(IntPtr message, uint tag, string value) => NativeLinuxMethods.mdf_message_add_time(message, tag, value);
        public int mdf_message_add_time2(IntPtr message, uint tag, int hour, int min, int sec, int msec) => NativeLinuxMethods.mdf_message_add_time2(message, tag, hour, min, sec, msec);
        public int mdf_message_add_time3(IntPtr message, uint tag, int hour, int min, int sec, int nsec) => NativeLinuxMethods.mdf_message_add_time3(message, tag, hour, min, sec, nsec);
        public int mdf_message_add_uint(IntPtr message, uint tag, ulong value, int decimals) => NativeLinuxMethods.mdf_message_add_uint(message, tag, value, decimals);
        public IntPtr mdf_message_create() => NativeLinuxMethods.mdf_message_create();
        public int mdf_message_del(IntPtr message) => NativeLinuxMethods.mdf_message_del(message);
        public void mdf_message_destroy(IntPtr message) => NativeLinuxMethods.mdf_message_destroy(message);
        public int mdf_message_get_num(IntPtr message) => NativeLinuxMethods.mdf_message_get_num(message);
        public int mdf_message_get_num_active(IntPtr message) => NativeLinuxMethods.mdf_message_get_num_active(message);
        public void mdf_message_reset(IntPtr message) => NativeLinuxMethods.mdf_message_reset(message);
        public int mdf_message_send(IntPtr handle, IntPtr message) => NativeLinuxMethods.mdf_message_send(handle, message);
        public int mdf_set_property(IntPtr handle, MDF_OPTION option, IntPtr value) => NativeLinuxMethods.mdf_set_property(handle, option, value);
        public int mdf_set_property(IntPtr handle, MDF_OPTION option, string value) => NativeLinuxMethods.mdf_set_property(handle, option, value);
        public int mdf_message_set_compression_level(IntPtr message, int level) => NativeLinuxMethods.mdf_message_set_compression_level(message, level);
        public int mdf_message_move(IntPtr src, IntPtr dst, ulong insref_src, ulong insref_dst) => NativeLinuxMethods.mdf_message_move(src, dst, insref_src, insref_dst);
        public int mdf_message_serialize(IntPtr message, ref IntPtr result) => NativeLinuxMethods.mdf_message_serialize(message, ref result);
        public int mdf_message_deserialize(IntPtr message, string data) => NativeLinuxMethods.mdf_message_deserialize(message, data);
        public int mdf_message_set_utf8_validation(IntPtr message, int enable) => NativeLinuxMethods.mdf_message_set_utf8_validation(message, enable);
    }
}