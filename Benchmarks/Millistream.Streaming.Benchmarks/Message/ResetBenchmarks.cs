using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class ResetBenchmarks : MessageBenchmarks
    {
        [Benchmark(Baseline = true)]
        public void Reset()
        {
            _ = _message.Add(0, 0);
            _message.Reset();
        }

        [Benchmark]
        public void ResetUsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            DllImports.mdf_message_reset(_messageHandle);
        }
    }
}