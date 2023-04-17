using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class DeleteBenchmarks : MessageBenchmarks
    {
        [Benchmark(Baseline = true)]
        public bool Delete()
        {
            _ = _message.Add(0, 0);
            return _message.Delete();
        }

        [Benchmark]
        public bool DeleteUsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            return DllImports.mdf_message_del(_messageHandle) == 1;
        }
    }
}