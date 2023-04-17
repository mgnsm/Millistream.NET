using BenchmarkDotNet.Attributes;
using System;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class SerializeBenchmarks : MessageBenchmarks
    {
        [Benchmark(Baseline = true)]
        public bool Serialize() => _message.Serialize(out IntPtr _);

        [Benchmark]
        public bool SerializeUsingDllImport()
        {
            IntPtr result = IntPtr.Zero;
            return DllImports.mdf_message_serialize(_messageHandle, ref result) == 1;
        }
    }
}