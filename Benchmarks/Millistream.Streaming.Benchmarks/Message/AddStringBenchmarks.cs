using BenchmarkDotNet.Attributes;
using System;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class AddStringBenchmarks : MessageBenchmarks
    {
        private static readonly byte[] s_FooBar = new byte[] { 70, 111, 111, 32, 66, 97, 114, 0 };
        private static readonly byte[] s_aaaBBBccc = new byte[] { 97, 97, 97, 66, 66, 66, 99, 99, 99, 0 };
        private static readonly byte[] s_SE0000108656 = new byte[] { 83, 69, 48, 48, 48, 48, 49, 48, 56, 54, 53, 54, 0 };
        private static readonly byte[] s_åäöÅÄÖ = new byte[] { 195, 165, 195, 164, 195, 182, 195, 133, 195, 132, 195, 150, 195, 166, 195, 159, 226, 130, 172, 194, 181, 0 };
        private static readonly byte[] s_12P = new byte[] { 49, 50, 45, 45, 45, 45, 45, 45, 45, 80, 45, 45, 45, 45, 0 };
        private static readonly byte[] s_46XBP3 = new byte[] { 52, 54, 45, 45, 88, 45, 66, 45, 45, 80, 45, 51, 45, 45, 0 };

        [Benchmark(OperationsPerInvoke = 7)]
        public void AddString()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddString(Tag, "Foo Bar");
            _ = _message.AddString(Tag, "aaaBBBccc");
            _ = _message.AddString(Tag, "SE0000108656");
            _ = _message.AddString(Tag, "åäöÅÄÖæß");
            _ = _message.AddString(Tag, "12-------P----");
            _ = _message.AddString(Tag, "46--X-B--P-3--");
            _ = _message.AddString(Tag, default(string));
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 7)]
        public void AddStringUsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = DllImports.mdf_message_add_string(_messageHandle, Tag, "Foo Bar") == 1;
            _ = DllImports.mdf_message_add_string(_messageHandle, Tag, "aaaBBBccc") == 1;
            _ = DllImports.mdf_message_add_string(_messageHandle, Tag, "SE0000108656") == 1;
            _ = DllImports.mdf_message_add_string(_messageHandle, Tag, "åäöÅÄÖæß") == 1;
            _ = DllImports.mdf_message_add_string(_messageHandle, Tag, "12-------P----") == 1;
            _ = DllImports.mdf_message_add_string(_messageHandle, Tag, "46--X-B--P-3--") == 1;
            _ = DllImports.mdf_message_add_string(_messageHandle, Tag, default) == 1;
            DllImports.mdf_message_reset(_messageHandle);
        }

        [Benchmark(OperationsPerInvoke = 7)]
        public void AddStringBytes()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddString(Tag, s_FooBar);
            _ = _message.AddString(Tag, s_aaaBBBccc);
            _ = _message.AddString(Tag, s_SE0000108656);
            _ = _message.AddString(Tag, s_åäöÅÄÖ);
            _ = _message.AddString(Tag, s_12P);
            _ = _message.AddString(Tag, s_46XBP3);
            _ = _message.AddString(Tag, default(ReadOnlySpan<byte>));
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 7)]
        public void AddString2()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddString(Tag, "Foo Bar", 7);
            _ = _message.AddString(Tag, "aaaBBBccc", 9);
            _ = _message.AddString(Tag, "SE0000108656", 12);
            _ = _message.AddString(Tag, "åäöÅÄÖæß", 8);
            _ = _message.AddString(Tag, "12-------P----", 14);
            _ = _message.AddString(Tag, "46--X-B--P-3--", 14);
            _ = _message.AddString(Tag, default(string), 10);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 7)]
        public void AddString2UsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = DllImports.mdf_message_add_string2(_messageHandle, Tag, "Foo Bar", 7);
            _ = DllImports.mdf_message_add_string2(_messageHandle, Tag, "aaaBBBccc", 9);
            _ = DllImports.mdf_message_add_string2(_messageHandle, Tag, "SE0000108656", 12);
            _ = DllImports.mdf_message_add_string2(_messageHandle, Tag, "åäöÅÄÖæß", 8);
            _ = DllImports.mdf_message_add_string2(_messageHandle, Tag, "12-------P----", 14);
            _ = DllImports.mdf_message_add_string2(_messageHandle, Tag, "46--X-B--P-3--", 14);
            _ = DllImports.mdf_message_add_string2(_messageHandle, Tag, default, 10);
            DllImports.mdf_message_reset(_messageHandle);
        }

        [Benchmark(Baseline = true, OperationsPerInvoke = 7)]
        public void AddString2Bytes()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddString(Tag, s_FooBar, 7);
            _ = _message.AddString(Tag, s_aaaBBBccc, 9);
            _ = _message.AddString(Tag, s_SE0000108656, 12);
            _ = _message.AddString(Tag, s_åäöÅÄÖ, 21);
            _ = _message.AddString(Tag, s_12P, 14);
            _ = _message.AddString(Tag, s_46XBP3, 14);
            _ = _message.AddString(Tag, default(ReadOnlySpan<byte>));
            _message.Reset();
        }
    }
}