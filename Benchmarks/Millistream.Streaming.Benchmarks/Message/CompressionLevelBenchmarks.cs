using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class CompressionLevelBenchmarks : MessageBenchmarks
    {
        private const CompressionLevel Level = CompressionLevel.Z_BEST_COMPRESSION;

        [Benchmark(Baseline = true)]
        public void SetCompressionLevel() => _message.CompressionLevel = Level;

        [Benchmark]
        public void SetCompressionLevelUsingDllImport() =>
            DllImports.mdf_message_set_property(_messageHandle, MDF_MSG_OPTION.MDF_MSG_OPT_COMPRESSION, (int)Level);
    }
}