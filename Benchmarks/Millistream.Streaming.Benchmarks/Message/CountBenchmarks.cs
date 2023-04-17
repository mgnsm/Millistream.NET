using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class CountBenchmarks : MessageBenchmarks
    {
        [Benchmark]
        public int GetMessageCount() => _message.Count;

        [Benchmark]
        public int GetActiveMessageCount() => _message.ActiveCount;

        [Benchmark]
        public int GetFieldCount() => _message.FieldCount;

        [Benchmark]
        public int GetMessageNumUsingDllImport() => DllImports.mdf_message_get_num(_messageHandle);

        [Benchmark]
        public int GetMessageNumActiveUsingDllImport() => DllImports.mdf_message_get_num_active(_messageHandle);

        [Benchmark]
        public int GetMessageNumFieldsUsingDllImport() => DllImports.mdf_message_get_num_fields(_messageHandle);
    }
}
