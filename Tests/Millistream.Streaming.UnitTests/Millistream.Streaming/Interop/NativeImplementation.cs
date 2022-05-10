using System;

namespace Millistream.Streaming.Interop
{
    internal unsafe class NativeImplementation
    {
        private static NativeImplementation s_defaultImplementation;

        internal delegate*<IntPtr> mdf_create;
        internal delegate*<IntPtr, void> mdf_destroy;
        internal delegate*<IntPtr, int, int> mdf_consume;
        internal delegate*<IntPtr, ref int, ref int, ref ulong, int> mdf_get_next_message;
        internal delegate*<IntPtr, ref ushort, ref ulong, int> mdf_get_next_message2;
        internal delegate*<IntPtr, ref uint, ref IntPtr, int> mdf_get_next_field;
        internal delegate*<IntPtr, MDF_OPTION, ref IntPtr, int> mdf_get_property;
        internal delegate*<IntPtr, MDF_OPTION, ref int, int> mdf_get_int_property;
        internal delegate*<IntPtr, MDF_OPTION, ref ulong, int> mdf_get_ulong_property;
        internal delegate*<IntPtr, MDF_OPTION, ref long, int> mdf_get_long_property;
        internal delegate*<IntPtr, MDF_OPTION, IntPtr, int> mdf_set_property;
        internal delegate*<IntPtr, byte> mdf_get_delay;
        internal delegate*<IntPtr, ulong> mdf_get_mclass;
        internal delegate*<IntPtr, IntPtr, int> mdf_connect;
        internal delegate*<IntPtr, void> mdf_disconnect;
        internal delegate*<IntPtr> mdf_message_create;
        internal delegate*<IntPtr, void> mdf_message_destroy;
        internal delegate*<IntPtr, void> mdf_message_reset;
        internal delegate*<IntPtr, int> mdf_message_del;
        internal delegate*<IntPtr, ulong, int, int> mdf_message_add;
        internal delegate*<IntPtr, uint, IntPtr, int> mdf_message_add_numeric;
        internal delegate*<IntPtr, uint, long, int, int> mdf_message_add_int;
        internal delegate*<IntPtr, uint, ulong, int, int> mdf_message_add_uint;
        internal delegate*<IntPtr, uint, IntPtr, int> mdf_message_add_string;
        internal delegate*<IntPtr, uint, IntPtr, int, int> mdf_message_add_string2;
        internal delegate*<IntPtr, uint, IntPtr, int> mdf_message_add_date;
        internal delegate*<IntPtr, uint, int, int, int, int> mdf_message_add_date2;
        internal delegate*<IntPtr, uint, IntPtr, int> mdf_message_add_time;
        internal delegate*<IntPtr, uint, int, int, int, int, int> mdf_message_add_time2;
        internal delegate*<IntPtr, uint, int, int, int, int, int> mdf_message_add_time3;
        internal delegate*<IntPtr, uint, IntPtr, int> mdf_message_add_list;
        internal delegate*<IntPtr, IntPtr, int> mdf_message_send;
        internal delegate*<IntPtr, int> mdf_message_get_num;
        internal delegate*<IntPtr, int> mdf_message_get_num_active;
        internal delegate*<IntPtr, IntPtr, ulong, ulong, int> mdf_message_move;
        internal delegate*<IntPtr, ref IntPtr, int> mdf_message_serialize;
        internal delegate*<IntPtr, IntPtr, int> mdf_message_deserialize;
        internal delegate*<IntPtr, MDF_MSG_OPTION, int, int> mdf_message_set_property;
        internal delegate*<IntPtr, int> mdf_message_get_num_fields;
        internal delegate*<IntPtr, int, int> mdf_message_set_compression_level;
        internal delegate*<IntPtr, int, int> mdf_message_set_utf8_validation;

        internal static NativeImplementation Default => s_defaultImplementation ??= new(null);
        internal static INativeImplementation Implementation { get; set; }
        internal static int InstanceCount { get; private set; }

        internal NativeImplementation(string _)
        {
            InstanceCount++;

            mdf_create = &MdfCreate;
            mdf_destroy = &MdfDestroy;
            mdf_consume = &MdfConsume;
            mdf_get_next_message = &MdfGetNextMessage;
            mdf_get_next_message2 = &MdfGetNextMessage2;
            mdf_get_next_field = &MdfGetNextField;
            mdf_get_property = &MdfGetProperty;
            mdf_get_int_property = &MdfGetInt32Property;
            mdf_get_ulong_property = &MdfGetUInt64Property;
            mdf_get_long_property = &MdfGetInt64Property;
            mdf_set_property = &MdfSetProperty;
            mdf_get_delay = &MdfGetDelay;
            mdf_get_mclass = &MdfGetMClass;
            mdf_connect = &MdfConnect;
            mdf_disconnect = &MdfDisconnect;
            mdf_message_create = &MdfMessageCreate;
            mdf_message_destroy = &MdfMessageDestroy;
            mdf_message_reset = &MdfMessageReset;
            mdf_message_del = &MdfMessageDel;
            mdf_message_add = &MdfMessageAdd;
            mdf_message_add_numeric = &MdfMessageAddNumeric;
            mdf_message_add_int = &MdfMessageAddInt32;
            mdf_message_add_uint = &MdfMessageAddUInt64;
            mdf_message_add_string = &MdfMessageAddString;
            mdf_message_add_string2 = &MdfMessageAddString2;
            mdf_message_add_date = &MdfMessageAddDate;
            mdf_message_add_date2 = &MdfMessageAddDate2;
            mdf_message_add_time = &MdfMessageAddTime;
            mdf_message_add_time2 = &MdfMessageAddTime2;
            mdf_message_add_time3 = &MdfMessageAddTime3;
            mdf_message_add_list = &MdfMessageAddList;
            mdf_message_send = &MdfMessageSend;
            mdf_message_get_num = &MdfMessageGetNum;
            mdf_message_get_num_active = &MdfMessageGetNumActive;
            mdf_message_move = &MdfMessageMove;
            mdf_message_serialize = &MdfMessageSerialize;
            mdf_message_deserialize = &MdfMessageDeserialize;
            mdf_message_set_property = &MdfMessageSetProperty;
            mdf_message_get_num_fields = &MdfMessageGetNumFields;
            mdf_message_set_compression_level = &MdfMessageSetCompressionLevel;
            mdf_message_set_utf8_validation = &MdfMessageSetUtf8Validation;
        }

        private static IntPtr MdfCreate() => Implementation.mdf_create();
        private static void MdfDestroy(IntPtr handle) => Implementation.mdf_destroy(handle);
        private static int MdfConsume(IntPtr handle, int timeout) => Implementation.mdf_consume(handle, timeout);
        private static int MdfGetNextMessage(IntPtr handle, ref int mref, ref int mclass, ref ulong insref) => Implementation.mdf_get_next_message(handle, ref mref, ref mclass, ref insref);
        private static int MdfGetNextMessage2(IntPtr handle, ref ushort mref, ref ulong insref) => Implementation.mdf_get_next_message2(handle, ref mref, ref insref);
        private static int MdfGetNextField(IntPtr handle, ref uint tag, ref IntPtr value) => Implementation.mdf_get_next_field(handle, ref tag, ref value);
        private static int MdfGetProperty(IntPtr handle, MDF_OPTION option, ref IntPtr value) => Implementation.mdf_get_property(handle, (int)option, ref value);
        private static int MdfGetInt32Property(IntPtr handle, MDF_OPTION option, ref int value) => Implementation.mdf_get_property(handle, (int)option, ref value);
        private static int MdfGetUInt64Property(IntPtr handle, MDF_OPTION option, ref ulong value) => Implementation.mdf_get_property(handle, (int)option, ref value);
        private static int MdfGetInt64Property(IntPtr handle, MDF_OPTION option, ref long value) => Implementation.mdf_get_property(handle, (int)option, ref value);
        private static int MdfSetProperty(IntPtr handle, MDF_OPTION option, IntPtr value) => Implementation.mdf_set_property(handle, (int)option, value);
        private static byte MdfGetDelay(IntPtr handle) => Implementation.mdf_get_delay(handle);
        private static ulong MdfGetMClass(IntPtr handle) => Implementation.mdf_get_mclass(handle);
        private static int MdfConnect(IntPtr handle, IntPtr server) => Implementation.mdf_connect(handle, server);
        private static void MdfDisconnect(IntPtr handle) => Implementation.mdf_disconnect(handle);
        private static IntPtr MdfMessageCreate() => Implementation.mdf_message_create();
        private static void MdfMessageDestroy(IntPtr message) => Implementation.mdf_message_destroy(message);
        private static void MdfMessageReset(IntPtr message) => Implementation.mdf_message_reset(message);
        private static int MdfMessageDel(IntPtr message) => Implementation.mdf_message_del(message);
        private static int MdfMessageAdd(IntPtr message, ulong insref, int mref) => Implementation.mdf_message_add(message, insref, mref);
        private static int MdfMessageAddNumeric(IntPtr message, uint tag, IntPtr value) => Implementation.mdf_message_add_numeric(message, tag, value);
        private static int MdfMessageAddInt32(IntPtr message, uint tag, long value, int decimals) => Implementation.mdf_message_add_int(message, tag, value, decimals);
        private static int MdfMessageAddUInt64(IntPtr message, uint tag, ulong value, int decimals) => Implementation.mdf_message_add_uint(message, tag, value, decimals);
        private static int MdfMessageAddString(IntPtr message, uint tag, IntPtr value) => Implementation.mdf_message_add_string(message, tag, value);
        private static int MdfMessageAddString2(IntPtr message, uint tag, IntPtr value, int len) => Implementation.mdf_message_add_string2(message, tag, value, len);
        private static int MdfMessageAddDate(IntPtr message, uint tag, IntPtr value) => Implementation.mdf_message_add_date(message, tag, value);
        private static int MdfMessageAddDate2(IntPtr message, uint tag, int year, int mon, int day) => Implementation.mdf_message_add_date2(message, tag, year, mon, day);
        private static int MdfMessageAddTime(IntPtr message, uint tag, IntPtr value) => Implementation.mdf_message_add_time(message, tag, value);
        private static int MdfMessageAddTime2(IntPtr message, uint tag, int hour, int min, int sec, int msec) => Implementation.mdf_message_add_time2(message, tag, hour, min, sec, msec);
        private static int MdfMessageAddTime3(IntPtr message, uint tag, int hour, int min, int sec, int nsec) => Implementation.mdf_message_add_time3(message, tag, hour, min, sec, nsec);
        private static int MdfMessageAddList(IntPtr message, uint tag, IntPtr value) => Implementation.mdf_message_add_list(message, tag, value);
        private static int MdfMessageSend(IntPtr handle, IntPtr message) => Implementation.mdf_message_send(handle, message);
        private static int MdfMessageGetNum(IntPtr message) => Implementation.mdf_message_get_num(message);
        private static int MdfMessageGetNumActive(IntPtr message) => Implementation.mdf_message_get_num_active(message);
        private static int MdfMessageMove(IntPtr src, IntPtr dst, ulong insref_src, ulong insref_dst) => Implementation.mdf_message_move(src, dst, insref_src, insref_dst);
        private static int MdfMessageSerialize(IntPtr message, ref IntPtr result) => Implementation.mdf_message_serialize(message, ref result);
        private static int MdfMessageDeserialize(IntPtr message, IntPtr data) => Implementation.mdf_message_deserialize(message, data);
        private static int MdfMessageSetProperty(IntPtr message, MDF_MSG_OPTION option, int value) => Implementation.mdf_message_set_property(message, (int)option, value);
        private static int MdfMessageGetNumFields(IntPtr message) => Implementation.mdf_message_get_num_fields(message);
        private static int MdfMessageSetCompressionLevel(IntPtr message, int level) => Implementation.mdf_message_set_compression_level(message, level);
        private static int MdfMessageSetUtf8Validation(IntPtr message, int enable) => Implementation.mdf_message_set_utf8_validation(message, enable);
    }
}