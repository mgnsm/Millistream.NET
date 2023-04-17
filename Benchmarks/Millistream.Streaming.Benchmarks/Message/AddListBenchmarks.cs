using BenchmarkDotNet.Attributes;
using System;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class AddListBenchmarks : MessageBenchmarks
    {
        private static readonly byte[] s_28 = new byte[] { 50, 56, 0 };
        private static readonly byte[] s_2828 = new byte[] { 50, 56, 32, 50, 56, 0 };
        private static readonly byte[] s_2828343 = new byte[] { 50, 56, 32, 50, 56, 32, 51, 52, 51, 0 };
        private static readonly byte[] s_equals28 = new byte[] { 61, 50, 56, 0 };
        private static readonly byte[] s_equals2828 = new byte[] { 61, 50, 56, 32, 50, 56, 0 };
        private static readonly byte[] s_equals2828343 = new byte[] { 61, 50, 56, 32, 50, 56, 32, 51, 52, 51, 0 };
        private static readonly byte[] s_plus28 = new byte[] { 43, 50, 56, 0 };
        private static readonly byte[] s_plus2828 = new byte[] { 43, 50, 56, 32, 50, 56, 0 };
        private static readonly byte[] s_plus2828343 = new byte[] { 43, 50, 56, 32, 50, 56, 32, 51, 52, 51, 0 };
        private static readonly byte[] s_minus28 = new byte[] { 45, 50, 56, 0 };
        private static readonly byte[] s_minus2828 = new byte[] { 45, 50, 56, 32, 50, 56, 0 };
        private static readonly byte[] s_minus2828343 = new byte[] { 45, 50, 56, 32, 50, 56, 32, 51, 52, 51, 0 };

        [Benchmark(OperationsPerInvoke = 13)]
        public void AddListString()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddList(Tag, "28");
            _ = _message.AddList(Tag, "28 28");
            _ = _message.AddList(Tag, "28 28 343");
            _ = _message.AddList(Tag, "=28");
            _ = _message.AddList(Tag, "=28 28");
            _ = _message.AddList(Tag, "=28 28 343");
            _ = _message.AddList(Tag, "+28");
            _ = _message.AddList(Tag, "+28 28");
            _ = _message.AddList(Tag, "+28 28 343");
            _ = _message.AddList(Tag, "-28");
            _ = _message.AddList(Tag, "-28 28");
            _ = _message.AddList(Tag, "-28 28 343");
            _ = _message.AddList(Tag, default(string));
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 13)]
        public void AddListStringUsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "28") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "28 28") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "28 28 343") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "=28") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "=28 28") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "=28 28 343") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "+28") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "+28 28") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "+28 28 343") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "-28") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "-28 28") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, "-28 28 343") == 1;
            _ = DllImports.mdf_message_add_list(_messageHandle, Tag, default) == 1;
            DllImports.mdf_message_reset(_messageHandle);
        }

        [Benchmark(Baseline = true, OperationsPerInvoke = 13)]
        public void AddListBytes()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddList(Tag, s_28);
            _ = _message.AddList(Tag, s_2828);
            _ = _message.AddList(Tag, s_2828343);
            _ = _message.AddList(Tag, s_equals28);
            _ = _message.AddList(Tag, s_equals2828);
            _ = _message.AddList(Tag, s_equals2828343);
            _ = _message.AddList(Tag, s_plus28);
            _ = _message.AddList(Tag, s_plus2828);
            _ = _message.AddList(Tag, s_plus2828343);
            _ = _message.AddList(Tag, s_minus28);
            _ = _message.AddList(Tag, s_minus2828);
            _ = _message.AddList(Tag, s_minus2828343);
            _ = _message.AddList(Tag, default(ReadOnlySpan<byte>));
            _message.Reset();
        }
    }
}