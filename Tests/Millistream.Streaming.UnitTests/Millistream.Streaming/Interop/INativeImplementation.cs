using System;

namespace Millistream.Streaming.Interop
{
    public interface INativeImplementation
    {
#pragma warning disable IDE1006
        IntPtr mdf_create();
        void mdf_destroy(IntPtr handle);
        int mdf_consume(IntPtr handle, int timeout);
        int mdf_get_next_message(IntPtr handle, ref int mref, ref int mclass, ref ulong insref);
        int mdf_get_next_field(IntPtr handle, ref uint tag, ref IntPtr value);
        int mdf_get_property(IntPtr handle, int option, ref IntPtr value);
        int mdf_get_property(IntPtr handle, int option, ref int value);
        int mdf_get_property(IntPtr handle, int option, ref ulong value);
        int mdf_get_property(IntPtr handle, int option, ref long value);
        int mdf_set_property(IntPtr handle, int option, IntPtr value);
        int mdf_connect(IntPtr handle, IntPtr server);
        void mdf_disconnect(IntPtr handle);
        IntPtr mdf_message_create();
        void mdf_message_destroy(IntPtr message);
        void mdf_message_reset(IntPtr message);
        int mdf_message_del(IntPtr message);
        int mdf_message_add(IntPtr message, ulong instrument_reference, int message_reference);
        int mdf_message_add_numeric(IntPtr message, uint tag, IntPtr value);
        int mdf_message_add_int(IntPtr message, uint tag, long value, int decimals);
        int mdf_message_add_uint(IntPtr message, uint tag, ulong value, int decimals);
        int mdf_message_add_string(IntPtr message, uint tag, IntPtr value);
        int mdf_message_add_string2(IntPtr message, uint tag, IntPtr value, int len);
        int mdf_message_add_date(IntPtr message, uint tag, IntPtr value);
        int mdf_message_add_date2(IntPtr message, uint tag, int year, int mon, int day);
        int mdf_message_add_time(IntPtr message, uint tag, IntPtr value);
        int mdf_message_add_time2(IntPtr message, uint tag, int hour, int min, int sec, int msec);
        int mdf_message_add_time3(IntPtr message, uint tag, int hour, int min, int sec, int nsec);
        int mdf_message_add_list(IntPtr message, uint tag, IntPtr value);
        int mdf_message_send(IntPtr handle, IntPtr message);
        int mdf_message_get_num(IntPtr message);
        int mdf_message_get_num_active(IntPtr message);
        int mdf_message_move(IntPtr src, IntPtr dst, ulong insref_src, ulong insref_dst);
        int mdf_message_serialize(IntPtr message, ref IntPtr result);
        int mdf_message_deserialize(IntPtr message, IntPtr data);
        int mdf_message_set_property(IntPtr message, int option, int value);
        int mdf_message_get_num_fields(IntPtr message);
#pragma warning restore IDE1006
    }
}
