using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System;

namespace Millistream.Streaming.Benchmarks.Message
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
    public abstract class MessageBenchmarks : IDisposable
    {
        protected const uint Tag = 0;
        protected readonly Streaming.Message _message = new();
        protected IntPtr _messageHandle;

        [GlobalSetup]
        public void GlobalSetup() =>
            _messageHandle = DllImports.mdf_message_create();

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _message?.Dispose();
            DllImports.mdf_message_destroy(_messageHandle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                GlobalCleanup();
        }
    }
}