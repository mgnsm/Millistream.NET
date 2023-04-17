using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class AddBenchmarks : MessageBenchmarks
    {
        [Benchmark(Baseline = true)]
        public void Add() => _ = _message.Add(0, 0);

        [Benchmark]
        public void AddUsingDllImport() =>
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
    }
}