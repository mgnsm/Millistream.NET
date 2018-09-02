using System;

namespace Millistream.Streaming
{
    internal interface INativeImplementation
    {
        IntPtr mdf_create();
        void mdf_destroy(IntPtr handle);
        int mdf_get_next_message(IntPtr handle, ref int message, ref int message_class, ref uint instrument);
        int mdf_get_next_field(IntPtr handle, ref uint tag, ref IntPtr value);
        int mdf_get_property(IntPtr handle, MDF_OPTION option, ref IntPtr value);
        int mdf_set_property(IntPtr handle, MDF_OPTION option, IntPtr value);
        int mdf_connect(IntPtr handle, string server);
        void mdf_disconnect(IntPtr handle);
        int mdf_consume(IntPtr handle, int timout);
        IntPtr mdf_message_create();
        int mdf_message_add(IntPtr message, ulong instrument_reference, int message_reference);
        int mdf_message_del(IntPtr message);
        void mdf_message_reset(IntPtr message);
        void mdf_message_destroy(IntPtr message);
        int mdf_message_add_list(IntPtr message, uint tag, string value);
        int mdf_message_add_numeric(IntPtr message, uint tag, string value);
        int mdf_message_add_uint(IntPtr message, uint tag, ulong value, int decimals);
        int mdf_message_add_int(IntPtr message, uint tag, long value, int decimals);
        int mdf_message_add_string(IntPtr message, uint tag, string value);
        int mdf_message_add_date(IntPtr message, uint tag, string value);
        int mdf_message_add_time(IntPtr message, uint tag, string value);
        int mdf_message_add_time2(IntPtr message, uint tag, int hour, int min, int sec, int msec);
        int mdf_message_get_num(IntPtr message);
        int mdf_message_get_num_active(IntPtr message);
        int mdf_message_send(IntPtr handle, IntPtr message);
    }
}
