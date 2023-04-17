using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class AddInt64Benchmarks : MessageBenchmarks
    {
        [Benchmark(Baseline = true, OperationsPerInvoke = 20)]
        public void AddInt64()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddInt64(Tag, -12345L, 2);
            _ = _message.AddInt64(Tag, 28L, 0);
            _ = _message.AddInt64(Tag, 283L, 0);
            _ = _message.AddInt64(Tag, 0L, 0);
            _ = _message.AddInt64(Tag, 27L, 0);
            _ = _message.AddInt64(Tag, 99L, 2);
            _ = _message.AddInt64(Tag, 25599L, 2);
            _ = _message.AddInt64(Tag, 1001L, 2);
            _ = _message.AddInt64(Tag, 1030001L, 4);
            _ = _message.AddInt64(Tag, 104857L, 3);
            _ = _message.AddInt64(Tag, 1048576L, 4);
            _ = _message.AddInt64(Tag, 1L, 4);
            _ = _message.AddInt64(Tag, 1074176L, 2);
            _ = _message.AddInt64(Tag, 25601L, 2);
            _ = _message.AddInt64(Tag, 284L, 0);
            _ = _message.AddInt64(Tag, 2000L, 0);
            _ = _message.AddInt64(Tag, 20000000000000000L, 0);
            _ = _message.AddInt64(Tag, -10000001L, 7);
            _ = _message.AddInt64(Tag, 10000001L, 7);
            _ = _message.AddInt64(Tag, 1L, 15);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 20)]
        public void AddInt64UsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, -12345L, 2) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 28L, 0) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 283L, 0) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 0L, 0) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 27L, 0) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 99L, 2) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 25599L, 2) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 1001L, 2) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 1030001L, 4) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 104857L, 3) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 1048576L, 4) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 1L, 4) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 1074176L, 2) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 25601L, 2) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 284L, 0) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 2000L, 0) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 20000000000000000L, 0) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, -10000001L, 7) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 10000001L, 7) == 1;
            _ = DllImports.mdf_message_add_int(_messageHandle, Tag, 1L, 15) == 1;
            DllImports.mdf_message_reset(_messageHandle);
        }
    }
}