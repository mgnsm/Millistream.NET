using BenchmarkDotNet.Attributes;
using Millistream.Streaming.Interop;
using System;

namespace Millistream.Streaming.Benchmarks
{
    [MemoryDiagnoser]
    public unsafe class PInvokeBenchmarks
    {
#pragma warning disable IDE0079
#pragma warning disable CA1822
        [Benchmark]
        public int NativeFunctionCall() => NativeImplementation.Default.mdf_message_del(IntPtr.Zero);
#pragma warning restore CA1822
#pragma warning restore IDE0079
    }
}