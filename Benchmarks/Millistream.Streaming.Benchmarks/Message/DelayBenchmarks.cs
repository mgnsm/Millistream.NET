using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class DelayBenchmarks : MessageBenchmarks
    {
        [Benchmark(Baseline = true)]
        public void SetMessageDelay() => _message.Delay = 1;

        [Benchmark]
        public bool SetMessageDelayUsingDllImport() =>
            DllImports.mdf_message_set_property(_messageHandle, MDF_MSG_OPTION.MDF_MSG_OPT_DELAY, 1) == 1;
    }
}