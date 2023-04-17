using BenchmarkDotNet.Attributes;
using System.Text;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class DeserializeBenchmarks : MessageBenchmarks
    {
        private const string Base64EncodedMessage = "AQAAAAIAAAADAAAAAAAAAAAAAAAAAAAABQAAAAEAAAAAAAAAAAAAABMAAAABAAAAAAAAAAAAAABLAAAAAgAAAAIAAAAAAAAAMwAAAAEAAAAAX2U=";
        private static readonly byte[] s_base64EncodedMessageBytes = Encoding.ASCII.GetBytes(Base64EncodedMessage);

        [Benchmark]
        public bool Deserialize() => _message.Deserialize(Base64EncodedMessage);

        [Benchmark]
        public bool DeserializeUsingDllImport() => 
            _ = DllImports.mdf_message_deserialize(_messageHandle, Base64EncodedMessage) == 1;

        [Benchmark(Baseline = true)]
        public bool DeserializeBytes() => _message.Deserialize(s_base64EncodedMessageBytes);
    }
}