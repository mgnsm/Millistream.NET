using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class AddUInt64Benchmarks : MessageBenchmarks
    {
        [Benchmark(Baseline = true, OperationsPerInvoke = 21)]
        public void AddUInt64()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddUInt64(Tag, 12345UL, 2);
            _ = _message.AddUInt64(Tag, 28UL, 0);
            _ = _message.AddUInt64(Tag, 283UL, 0);
            _ = _message.AddUInt64(Tag, 0UL, 0);
            _ = _message.AddUInt64(Tag, 27UL, 0);
            _ = _message.AddUInt64(Tag, 99UL, 2);
            _ = _message.AddUInt64(Tag, 25599UL, 2);
            _ = _message.AddUInt64(Tag, 1001UL, 2);
            _ = _message.AddUInt64(Tag, 1030001UL, 4);
            _ = _message.AddUInt64(Tag, 104857UL, 3);
            _ = _message.AddUInt64(Tag, 1048576UL, 4);
            _ = _message.AddUInt64(Tag, 1UL, 4);
            _ = _message.AddUInt64(Tag, 1074176UL, 2);
            _ = _message.AddUInt64(Tag, 25601UL, 2);
            _ = _message.AddUInt64(Tag, 284UL, 0);
            _ = _message.AddUInt64(Tag, 2000UL, 0);
            _ = _message.AddUInt64(Tag, 20000000000000000UL, 0);
            _ = _message.AddUInt64(Tag, 18446744073709551610UL, 0);
            _ = _message.AddUInt64(Tag, 18446744073709551615UL, 15);
            _ = _message.AddUInt64(Tag, 10000001UL, 7);
            _ = _message.AddUInt64(Tag, 1UL, 15);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 21)]
        public void AddUInt64UsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 12345UL, 2) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 28UL, 0) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 283UL, 0) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 0UL, 0) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 27UL, 0) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 99UL, 2) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 25599UL, 2) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 1001UL, 2) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 1030001UL, 4) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 104857UL, 3) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 1048576UL, 4) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 1UL, 4) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 1074176UL, 2) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 25601UL, 2) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 284UL, 0) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 2000UL, 0) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 20000000000000000UL, 0) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 18446744073709551610UL, 0) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 18446744073709551615UL, 15) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 10000001UL, 7) == 1;
            _ = DllImports.mdf_message_add_uint(_messageHandle, Tag, 1UL, 15) == 1;
            DllImports.mdf_message_reset(_messageHandle);
        }
    }
}