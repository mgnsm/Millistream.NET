using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    [ReturnValueValidator]
    public class AddBenchmarks : MessageBenchmarks
    {
        [Benchmark(Baseline = true)]
        public bool Add() => _message.Add(0, 0);

        [Benchmark]
        public bool AddUsingDllImport() =>
            DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;

        [Benchmark]
        public unsafe bool AddUsingFunctionPointer() =>
            FunctionPointers.mdf_message_add(_messageHandle, 0, 0) == 1;
    }
}