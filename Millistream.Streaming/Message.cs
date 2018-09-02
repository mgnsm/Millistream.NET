using System;
using System.Globalization;
using System.Linq;

namespace Millistream.Streaming
{
    internal sealed class Message : IMessage
    {
        private const string DateFormat = "yyyy-MM-dd";
        private const string ListSeparator = " ";
        private readonly INativeImplementation _nativeImplementation;
        private readonly IntPtr _messageHandle;

        internal Message(INativeImplementation nativeImplementation, IntPtr messageHandle)
        {
            _nativeImplementation = nativeImplementation ?? throw new ArgumentNullException(nameof(nativeImplementation));
            _messageHandle = messageHandle;
        }

        public bool Add(ulong instrumentReference, MessageReference messageReference) =>
            _nativeImplementation.mdf_message_add(_messageHandle, instrumentReference, (int)messageReference) == 1;

        public bool AddDate(Field tag, DateTime value) =>
            _nativeImplementation.mdf_message_add_date(_messageHandle, (uint)tag, value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)) == 1;

        public bool AddInstrumentReferences(Field tag, ulong[] instrumentReferences)
        {
            if (instrumentReferences == null || instrumentReferences.Length == 0)
                return false;
            if (instrumentReferences.Length > 100_000_00)
                throw new ArgumentException("There is a current soft limit of 100.000.000 instrument references per list.", nameof(instrumentReferences));

            return _nativeImplementation.mdf_message_add_list(_messageHandle, (uint)tag, string.Join(ListSeparator, instrumentReferences)) == 1;
        }

        public bool AddRequestClasses(RequestClass[] requestClasses) =>
            (requestClasses == null || requestClasses.Length == 0) ? false : _nativeImplementation.mdf_message_add_list(_messageHandle, (uint)Field.MDF_F_REQUESTCLASS, string.Join(ListSeparator, requestClasses.Select(x => ((uint)x).ToString()))) == 1;

        public bool AddInt64(Field tag, long value, sbyte decimals)
        {
            if (decimals > 19)
                throw new ArgumentException($"{nameof(decimals)} cannot be greater than 19.", nameof(decimals));
            return _nativeImplementation.mdf_message_add_int(_messageHandle, (uint)tag, value, decimals) == 1;
        }

        public bool AddString(Field tag, string value) =>
            _nativeImplementation.mdf_message_add_string(_messageHandle, (uint)tag, value) == 1;

        public bool AddTime(Field tag, TimeSpan value) =>
            _nativeImplementation.mdf_message_add_time2(_messageHandle, (uint)tag, value.Hours, value.Minutes, value.Seconds, value.Milliseconds) == 1;

        public bool AddUInt32(Field tag, uint value) =>
            _nativeImplementation.mdf_message_add_numeric(_messageHandle, (uint)tag, value.ToString()) == 1;

        public bool AddUInt64(Field tag, ulong value, sbyte decimals)
        {
            if (decimals > 19)
                throw new ArgumentException($"{nameof(decimals)} cannot be greater than 19.", nameof(decimals));
            return _nativeImplementation.mdf_message_add_uint(_messageHandle, (uint)tag, value, decimals) == 1;
        }
    }
}