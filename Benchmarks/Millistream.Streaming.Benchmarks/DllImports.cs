using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming.Benchmarks
{
    internal static class DllImports
    {
#if Linux
        internal const string DllName = "libmdf.so.0";
#elif OSX
        internal const string DllName = "libmdf.0.dylib";
#else
        internal const string DllName = "libmdf-0.dll";
#endif

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr mdf_message_create();

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add(IntPtr message, ulong instrument_reference, int message_reference);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_del(IntPtr message);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void mdf_message_reset(IntPtr message);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void mdf_message_destroy(IntPtr message);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_list(IntPtr message, uint tag, string value);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_list(IntPtr message, uint tag, IntPtr value);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_numeric(IntPtr message, uint tag, string value);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_numeric(IntPtr message, uint tag, IntPtr value);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_uint(IntPtr message, uint tag, ulong value, int decimals);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_int(IntPtr message, uint tag, long value, int decimals);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_string(IntPtr message, uint tag, string value);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_string(IntPtr message, uint tag, IntPtr value);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_string2(IntPtr message, uint tag, string value, int len);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_string2(IntPtr message, uint tag, IntPtr value, int len);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_date(IntPtr message, uint tag, string value);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_date(IntPtr message, uint tag, IntPtr value);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_date2(IntPtr message, uint tag, int year, int mon, int day);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_time(IntPtr message, uint tag, string value);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_time(IntPtr message, uint tag, IntPtr value);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_time2(IntPtr message, uint tag, int hour, int min, int sec, int msec);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_add_time3(IntPtr message, uint tag, int hour, int min, int sec, int nsec);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_get_num(IntPtr message);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_get_num_active(IntPtr message);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_get_num_fields(IntPtr message);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_set_property(IntPtr message, MDF_MSG_OPTION option, int value);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_move(IntPtr src, IntPtr dst, ulong insref_src, ulong insref_dst);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_serialize(IntPtr message, ref IntPtr result);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_deserialize(IntPtr handle, string data);

        [DllImport(DllName, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int mdf_message_deserialize(IntPtr handle, IntPtr data);
    }
}