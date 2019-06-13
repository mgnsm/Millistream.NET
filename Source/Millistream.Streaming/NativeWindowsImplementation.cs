﻿using System;
using System.Runtime.InteropServices;

namespace Millistream.Streaming
{
    internal class NativeWindowsImplementation : INativeImplementation
    {
        private static class NativeWindowsMethods
        {
            private const string DllName = "libmdf-0.dll";

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr mdf_create();

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern void mdf_destroy(IntPtr handle);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_get_next_message(IntPtr handle, ref int message, ref int message_class, ref uint instrument);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_get_next_field(IntPtr handle, ref uint tag, ref IntPtr value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_get_property(IntPtr handle, MDF_OPTION option, ref IntPtr value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_set_property(IntPtr handle, MDF_OPTION option, IntPtr value);

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
            public static extern int mdf_message_add_time(IntPtr message, uint tag, string value);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_add_time2(IntPtr message, uint tag, int hour, int min, int sec, int msec);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_get_num(IntPtr message);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_get_num_active(IntPtr message);

            [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mdf_message_send(IntPtr handle, IntPtr message);
        }

        public int mdf_connect(IntPtr handle, string server) => NativeWindowsMethods.mdf_connect(handle, server);
        public int mdf_consume(IntPtr handle, int timeout) => NativeWindowsMethods.mdf_consume(handle, timeout);
        public IntPtr mdf_create() => NativeWindowsMethods.mdf_create();
        public void mdf_destroy(IntPtr handle) => NativeWindowsMethods.mdf_destroy(handle);
        public void mdf_disconnect(IntPtr handle) => NativeWindowsMethods.mdf_disconnect(handle);
        public int mdf_get_next_field(IntPtr handle, ref uint tag, ref IntPtr value) => NativeWindowsMethods.mdf_get_next_field(handle, ref tag, ref value);
        public int mdf_get_next_message(IntPtr handle, ref int message, ref int message_class, ref uint instrument) => NativeWindowsMethods.mdf_get_next_message(handle, ref message, ref message_class, ref instrument);
        public int mdf_get_property(IntPtr handle, MDF_OPTION option, ref IntPtr value) => NativeWindowsMethods.mdf_get_property(handle, option, ref value);
        public int mdf_message_add(IntPtr message, ulong instrument_reference, int message_reference) => NativeWindowsMethods.mdf_message_add(message, instrument_reference, message_reference);
        public int mdf_message_add_date(IntPtr message, uint tag, string value) => NativeWindowsMethods.mdf_message_add_date(message, tag, value);
        public int mdf_message_add_int(IntPtr message, uint tag, long value, int decimals) => NativeWindowsMethods.mdf_message_add_int(message, tag, value, decimals);
        public int mdf_message_add_list(IntPtr message, uint tag, string value) => NativeWindowsMethods.mdf_message_add_list(message, tag, value);
        public int mdf_message_add_numeric(IntPtr message, uint tag, string value) => NativeWindowsMethods.mdf_message_add_numeric(message, tag, value);
        public int mdf_message_add_string(IntPtr message, uint tag, string value) => NativeWindowsMethods.mdf_message_add_string(message, tag, value);
        public int mdf_message_add_time(IntPtr message, uint tag, string value) => NativeWindowsMethods.mdf_message_add_time(message, tag, value);
        public int mdf_message_add_time2(IntPtr message, uint tag, int hour, int min, int sec, int msec) => NativeWindowsMethods.mdf_message_add_time2(message, tag, hour, min, sec, msec);
        public int mdf_message_add_uint(IntPtr message, uint tag, ulong value, int decimals) => NativeWindowsMethods.mdf_message_add_uint(message, tag, value, decimals);
        public IntPtr mdf_message_create() => NativeWindowsMethods.mdf_message_create();
        public int mdf_message_del(IntPtr message) => NativeWindowsMethods.mdf_message_del(message);
        public void mdf_message_destroy(IntPtr message) => NativeWindowsMethods.mdf_message_destroy(message);
        public int mdf_message_get_num(IntPtr message) => NativeWindowsMethods.mdf_message_get_num(message);
        public int mdf_message_get_num_active(IntPtr message) => NativeWindowsMethods.mdf_message_get_num_active(message);
        public void mdf_message_reset(IntPtr message) => NativeWindowsMethods.mdf_message_reset(message);
        public int mdf_message_send(IntPtr handle, IntPtr message) => NativeWindowsMethods.mdf_message_send(handle, message);
        public int mdf_set_property(IntPtr handle, MDF_OPTION option, IntPtr value) => NativeWindowsMethods.mdf_set_property(handle, option, value);
    }
}