using BenchmarkDotNet.Attributes;
using System;
using System.Text;

namespace Millistream.Streaming.Benchmarks.Message
{
    [ReturnValueValidator]
    public class DeserializeBenchmarks : MessageBenchmarks
    {
        private const string Base64EncodedMessage = "AQAAAAIAAAADAAAAAAAAAAAAAAAAAAAABQAAAAEAAAAAAAAAAAAAABMAAAABAAAAAAAAAAAAAABLAAAAAgAAAAIAAAAAAAAAMwAAAAEAAAAAX2U=";
        private static readonly byte[] s_base64EncodedMessageBytes = Encoding.ASCII.GetBytes(Base64EncodedMessage);

        [Benchmark]
        public bool Deserialize() => _message.Deserialize(Base64EncodedMessage);

        [Benchmark]
        public bool DeserializeUsingDllImport() => 
            DllImports.mdf_message_deserialize(_messageHandle, Base64EncodedMessage) == 1;

        [Benchmark]
        public unsafe bool DeserializeUsingFunctionPointer() =>
            FunctionPointers.mdf_message_deserialize_str(_messageHandle, Base64EncodedMessage) == 1;

        [Benchmark(Baseline = true)]
        public bool DeserializeBytes() => _message.Deserialize(s_base64EncodedMessageBytes);

        [Benchmark]
        public unsafe bool DeserializeBytesUsingDllImport()
        {
            fixed (byte* ptr = s_base64EncodedMessageBytes)
                return DllImports.mdf_message_deserialize(_messageHandle, (IntPtr)ptr) == 1;
        }

        [Benchmark]
        public unsafe bool DeserializeBytesUsingFunctionPointer()
        {
            fixed (byte* ptr = s_base64EncodedMessageBytes)
                return FunctionPointers.mdf_message_deserialize(_messageHandle, (IntPtr)ptr) == 1;
        }
    }
}