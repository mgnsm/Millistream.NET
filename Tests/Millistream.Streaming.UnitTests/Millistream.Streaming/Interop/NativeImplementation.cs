using System;

namespace Millistream.Streaming.Interop
{
    internal class NativeImplementation
    {
        private static NativeImplementation s_defaultImplementation;
        internal static NativeImplementation Default => s_defaultImplementation ??= new(null);
        internal static INativeImplementation Implementation { get; set; }
        internal static int InstanceCount { get; private set; }

        internal NativeImplementation(string _) => InstanceCount++;

#pragma warning disable IDE1006
#pragma warning disable IDE0079
#pragma warning disable CA1822
        internal IntPtr mdf_create() => Implementation.mdf_create();
        internal void mdf_destroy(IntPtr handle) => Implementation.mdf_destroy(handle);
        internal int mdf_consume(IntPtr handle, int timeout) => Implementation.mdf_consume(handle, timeout);
        internal int mdf_get_next_message(IntPtr handle, ref int mref, ref int mclass, ref ulong insref) => Implementation.mdf_get_next_message(handle, ref mref, ref mclass, ref insref);
        internal int mdf_get_next_field(IntPtr handle, ref uint tag, ref IntPtr value) => Implementation.mdf_get_next_field(handle, ref tag, ref value);
        internal int mdf_get_property(IntPtr handle, MDF_OPTION option, ref IntPtr value) => Implementation.mdf_get_property(handle, (int)option, ref value);
        internal int mdf_get_int_property(IntPtr handle, MDF_OPTION option, ref int value) => Implementation.mdf_get_property(handle, (int)option, ref value);
        internal int mdf_get_ulong_property(IntPtr handle, MDF_OPTION option, ref ulong value) => Implementation.mdf_get_property(handle, (int)option, ref value);
        internal int mdf_get_long_property(IntPtr handle, MDF_OPTION option, ref long value) => Implementation.mdf_get_property(handle, (int)option, ref value);
        internal int mdf_set_property(IntPtr handle, MDF_OPTION option, IntPtr value) => Implementation.mdf_set_property(handle, (int)option, value);
        internal int mdf_connect(IntPtr handle, IntPtr server) => Implementation.mdf_connect(handle, server);
        internal void mdf_disconnect(IntPtr handle) => Implementation.mdf_disconnect(handle);
        internal IntPtr mdf_message_create() => Implementation.mdf_message_create();
        internal void mdf_message_destroy(IntPtr message) => Implementation.mdf_message_destroy(message);
        internal int mdf_message_set_compression_level(IntPtr message, int level) => Implementation.mdf_message_set_compression_level(message, level);
        internal void mdf_message_reset(IntPtr message) => Implementation.mdf_message_reset(message);
        internal int mdf_message_del(IntPtr message) => Implementation.mdf_message_del(message);
        internal int mdf_message_add(IntPtr message, ulong insref, int mref) => Implementation.mdf_message_add(message, insref, mref);
        internal int mdf_message_add_numeric(IntPtr message, uint tag, IntPtr value) => Implementation.mdf_message_add_numeric(message, tag, value);
        internal int mdf_message_add_int(IntPtr message, uint tag, long value, int decimals) => Implementation.mdf_message_add_int(message, tag, value, decimals);
        internal int mdf_message_add_uint(IntPtr message, uint tag, ulong value, int decimals) => Implementation.mdf_message_add_uint(message, tag, value, decimals);
        internal int mdf_message_add_string(IntPtr message, uint tag, IntPtr value) => Implementation.mdf_message_add_string(message, tag, value);
        internal int mdf_message_add_string2(IntPtr message, uint tag, IntPtr value, int len) => Implementation.mdf_message_add_string2(message, tag, value, len);
        internal int mdf_message_add_date(IntPtr message, uint tag, IntPtr value) => Implementation.mdf_message_add_date(message, tag, value);
        internal int mdf_message_add_date2(IntPtr message, uint tag, int year, int mon, int day) => Implementation.mdf_message_add_date2(message, tag, year, mon, day);
        internal int mdf_message_add_time(IntPtr message, uint tag, IntPtr value) => Implementation.mdf_message_add_time(message, tag, value);
        internal int mdf_message_add_time2(IntPtr message, uint tag, int hour, int min, int sec, int msec) => Implementation.mdf_message_add_time2(message, tag, hour, min, sec, msec);
        internal int mdf_message_add_time3(IntPtr message, uint tag, int hour, int min, int sec, int nsec) => Implementation.mdf_message_add_time3(message, tag, hour, min, sec, nsec);
        internal int mdf_message_add_list(IntPtr message, uint tag, IntPtr value) => Implementation.mdf_message_add_list(message, tag, value);
        internal int mdf_message_send(IntPtr handle, IntPtr message) => Implementation.mdf_message_send(handle, message);
        internal int mdf_message_get_num(IntPtr message) => Implementation.mdf_message_get_num(message);
        internal int mdf_message_get_num_active(IntPtr message) => Implementation.mdf_message_get_num_active(message);
        internal int mdf_message_move(IntPtr src, IntPtr dst, ulong insref_src, ulong insref_dst) => Implementation.mdf_message_move(src, dst, insref_src, insref_dst);
        internal int mdf_message_serialize(IntPtr message, ref IntPtr result) => Implementation.mdf_message_serialize(message, ref result);
        internal int mdf_message_deserialize(IntPtr message, IntPtr data) => Implementation.mdf_message_deserialize(message, data);
        internal int mdf_message_set_utf8_validation(IntPtr message, int enable) => Implementation.mdf_message_set_utf8_validation(message, enable);
#pragma warning restore CA1822
#pragma warning restore IDE0079
#pragma warning restore IDE1006
    }
}