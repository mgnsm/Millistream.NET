using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class CompressionLevelBenchmarks : MessageBenchmarks
    {
#pragma warning disable CS0618
        private const CompressionLevel Level = CompressionLevel.Z_BEST_COMPRESSION;

        [Benchmark(Baseline = true)]
        public void SetCompressionLevel() => _message.CompressionLevel = Level;
#pragma warning restore CS0618

        [Benchmark]
        public int SetCompressionLevelUsingDllImport() =>
            DllImports.mdf_message_set_property(_messageHandle, MDF_MSG_OPTION.MDF_MSG_OPT_COMPRESSION, (int)Level);

        [Benchmark]
        public unsafe void SetCompressionLevelUsingFunctionPointer() =>
            FunctionPointers.mdf_message_set_property(_messageHandle, MDF_MSG_OPTION.MDF_MSG_OPT_COMPRESSION, (int)Level);
    }
}