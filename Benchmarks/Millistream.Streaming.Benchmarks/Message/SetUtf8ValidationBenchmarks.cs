using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class SetUtf8ValidationBenchmarks : MessageBenchmarks
    {
        [Benchmark(Baseline = true)]
        public void SetUtf8Validation() => _message.Utf8Validation = false;

        [Benchmark]
        public bool SetUtf8ValidationUsingDllImport() => 
            DllImports.mdf_message_set_property(_messageHandle, MDF_MSG_OPTION.MDF_MSG_OPT_UTF8, 0) == 1;

        [Benchmark]
        public unsafe bool SetUtf8ValidationUsingFunctionPointer() =>
            FunctionPointers.mdf_message_set_property(_messageHandle, MDF_MSG_OPTION.MDF_MSG_OPT_UTF8, 0) == 1;
    }
}