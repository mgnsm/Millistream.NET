using BenchmarkDotNet.Attributes;
using System;

namespace Millistream.Streaming.Benchmarks.Message
{
    [MemoryDiagnoser]
    public class MoveBenchmarks : IDisposable
    {
        private Streaming.Message _source;
        private IntPtr _sourceHandle;
        private Streaming.Message _destination;
        private IntPtr _destinationHandle;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _source = new();
            _sourceHandle = DllImports.mdf_message_create();
            _destination = new();
            _destinationHandle = DllImports.mdf_message_create();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _source?.Dispose();
            DllImports.mdf_message_destroy(_sourceHandle);
            _destination?.Dispose();
            DllImports.mdf_message_destroy(_destinationHandle);
        }

        [Benchmark(Baseline = true)]
        public bool Move() => Streaming.Message.Move(_source, _destination, 1, 2);

        [Benchmark]
        public bool MoveUsingDllImport() => DllImports.mdf_message_move(_sourceHandle, _destinationHandle, 1, 2) == 1;

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
