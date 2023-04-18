using BenchmarkDotNet.Attributes;
using System;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class AddTimeBenchmarks : MessageBenchmarks
    {
        private static readonly byte[] s_170301 = new byte[] { 49, 55, 58, 48, 51, 58, 48, 49, 0 };
        private static readonly byte[] s_170301999 = new byte[] { 49, 55, 58, 48, 51, 58, 48, 49, 46, 57, 57, 57, 0 };
        private static readonly byte[] s_000000 = new byte[] { 48, 48, 58, 48, 48, 58, 48, 48, 0 };
        private static readonly byte[] s_235958 = new byte[] { 50, 51, 58, 53, 57, 58, 53, 56, 0 };
        private static readonly byte[] s_235959001 = new byte[] { 50, 51, 58, 53, 57, 58, 53, 57, 46, 48, 48, 49, 0 };
        private static readonly byte[] s_235959999 = new byte[] { 50, 51, 58, 53, 57, 58, 53, 57, 46, 57, 57, 57, 0 };
        private static readonly byte[] s_235959000000001 = new byte[] { 50, 51, 58, 53, 57, 58, 53, 57, 46, 48, 48, 48, 48, 48, 48, 48, 48, 49, 0 };
        private static readonly byte[] s_235959999999999 = new byte[] { 50, 51, 58, 53, 57, 58, 53, 57, 46, 57, 57, 57, 57, 57, 57, 57, 57, 57, 0 };

        [Benchmark(OperationsPerInvoke = 8)]
        public void AddTimeString()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddTime(Tag, "17:03:01");
            _ = _message.AddTime(Tag, "17:03:01.999");
            _ = _message.AddTime(Tag, "00:00:00");
            _ = _message.AddTime(Tag, "23:59:58");
            _ = _message.AddTime(Tag, "23:59:59.001");
            _ = _message.AddTime(Tag, "23:59:59.999");
            _ = _message.AddTime(Tag, "23:59:59.000000001");
            _ = _message.AddTime(Tag, "23:59:59.999999999");
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 8)]
        public void AddTimeStringUsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = DllImports.mdf_message_add_time(_messageHandle, Tag, "17:03:01") == 1;
            _ = DllImports.mdf_message_add_time(_messageHandle, Tag, "17:03:01.999") == 1;
            _ = DllImports.mdf_message_add_time(_messageHandle, Tag, "00:00:00") == 1;
            _ = DllImports.mdf_message_add_time(_messageHandle, Tag, "23:59:58") == 1;
            _ = DllImports.mdf_message_add_time(_messageHandle, Tag, "23:59:59.001") == 1;
            _ = DllImports.mdf_message_add_time(_messageHandle, Tag, "23:59:59.999") == 1;
            _ = DllImports.mdf_message_add_time(_messageHandle, Tag, "23:59:59.000000001") == 1;
            _ = DllImports.mdf_message_add_time(_messageHandle, Tag, "23:59:59.999999999") == 1;
            DllImports.mdf_message_reset(_messageHandle);
        }

        [Benchmark(OperationsPerInvoke = 8)]
        public unsafe void AddTimeStringUsingFunctionPointer()
        {
            _ = FunctionPointers.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = FunctionPointers.mdf_message_add_time_str(_messageHandle, Tag, "17:03:01") == 1;
            _ = FunctionPointers.mdf_message_add_time_str(_messageHandle, Tag, "17:03:01.999") == 1;
            _ = FunctionPointers.mdf_message_add_time_str(_messageHandle, Tag, "00:00:00") == 1;
            _ = FunctionPointers.mdf_message_add_time_str(_messageHandle, Tag, "23:59:58") == 1;
            _ = FunctionPointers.mdf_message_add_time_str(_messageHandle, Tag, "23:59:59.001") == 1;
            _ = FunctionPointers.mdf_message_add_time_str(_messageHandle, Tag, "23:59:59.999") == 1;
            _ = FunctionPointers.mdf_message_add_time_str(_messageHandle, Tag, "23:59:59.000000001") == 1;
            _ = FunctionPointers.mdf_message_add_time_str(_messageHandle, Tag, "23:59:59.999999999") == 1;
            FunctionPointers.mdf_message_reset(_messageHandle);
        }

        [Benchmark(Baseline = true, OperationsPerInvoke = 8)]
        public void AddTimeBytes()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddTime(Tag, s_170301);
            _ = _message.AddTime(Tag, s_170301999);
            _ = _message.AddTime(Tag, s_000000);
            _ = _message.AddTime(Tag, s_235958);
            _ = _message.AddTime(Tag, s_235959001);
            _ = _message.AddTime(Tag, s_235959999);
            _ = _message.AddTime(Tag, s_235959000000001);
            _ = _message.AddTime(Tag, s_235959999999999);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 8)]
        public unsafe void AddTimeBytesUsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            fixed (byte* ptr = s_170301)
                _ = DllImports.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_170301999)
                _ = DllImports.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_000000)
                _ = DllImports.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_235958)
                _ = DllImports.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_235959001)
                _ = DllImports.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_235959999)
                _ = DllImports.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_235959000000001)
                _ = DllImports.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_235959999999999)
                _ = DllImports.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            DllImports.mdf_message_reset(_messageHandle);
        }

        [Benchmark(OperationsPerInvoke = 8)]
        public unsafe void AddTimeBytesUsingFunctionPointer()
        {
            _ = FunctionPointers.mdf_message_add(_messageHandle, 0, 0) == 1;
            fixed (byte* ptr = s_170301)
                _ = FunctionPointers.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_170301999)
                _ = FunctionPointers.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_000000)
                _ = FunctionPointers.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_235958)
                _ = FunctionPointers.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_235959001)
                _ = FunctionPointers.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_235959999)
                _ = FunctionPointers.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_235959000000001)
                _ = FunctionPointers.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            fixed (byte* ptr = s_235959999999999)
                _ = FunctionPointers.mdf_message_add_time(_messageHandle, Tag, (IntPtr)ptr);
            FunctionPointers.mdf_message_reset(_messageHandle);
        }

        [Benchmark(OperationsPerInvoke = 5)]
        public void AddTime2()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddTime2(Tag, 17, 3, 1, 999);
            _ = _message.AddTime2(Tag, 0, 0, 0, 0);
            _ = _message.AddTime2(Tag, 23, 59, 58, 0);
            _ = _message.AddTime2(Tag, 23, 59, 59, 1);
            _ = _message.AddTime2(Tag, 23, 59, 59, 999);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 5)]
        public void AddTime2UsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = DllImports.mdf_message_add_time2(_messageHandle, Tag, 17, 3, 1, 999);
            _ = DllImports.mdf_message_add_time2(_messageHandle, Tag, 0, 0, 0, 0);
            _ = DllImports.mdf_message_add_time2(_messageHandle, Tag, 23, 59, 58, 0);
            _ = DllImports.mdf_message_add_time2(_messageHandle, Tag, 23, 59, 59, 1);
            _ = DllImports.mdf_message_add_time2(_messageHandle, Tag, 23, 59, 59, 999);
            DllImports.mdf_message_reset(_messageHandle);
        }

        [Benchmark(OperationsPerInvoke = 5)]
        public unsafe void AddTime2UsingFunctionPointer()
        {
            _ = FunctionPointers.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = FunctionPointers.mdf_message_add_time2(_messageHandle, Tag, 17, 3, 1, 999);
            _ = FunctionPointers.mdf_message_add_time2(_messageHandle, Tag, 0, 0, 0, 0);
            _ = FunctionPointers.mdf_message_add_time2(_messageHandle, Tag, 23, 59, 58, 0);
            _ = FunctionPointers.mdf_message_add_time2(_messageHandle, Tag, 23, 59, 59, 1);
            _ = FunctionPointers.mdf_message_add_time2(_messageHandle, Tag, 23, 59, 59, 999);
            FunctionPointers.mdf_message_reset(_messageHandle);
        }

        [Benchmark(OperationsPerInvoke = 7)]
        public void AddTime3()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddTime3(Tag, 17, 3, 1, 999999999);
            _ = _message.AddTime3(Tag, 17, 3, 1, 999);
            _ = _message.AddTime3(Tag, 0, 0, 0, 0);
            _ = _message.AddTime3(Tag, 23, 59, 58, 0);
            _ = _message.AddTime3(Tag, 0, 0, 0, 1);
            _ = _message.AddTime3(Tag, 23, 59, 59, 1);
            _ = _message.AddTime3(Tag, 23, 59, 59, 999999999);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 7)]
        public void AddTime3UsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
           _ = DllImports.mdf_message_add_time3(_messageHandle, Tag, 17, 3, 1, 999999999);
           _ = DllImports.mdf_message_add_time3(_messageHandle, Tag, 17, 3, 1, 999);
           _ = DllImports.mdf_message_add_time3(_messageHandle, Tag, 0, 0, 0, 0);
           _ = DllImports.mdf_message_add_time3(_messageHandle, Tag, 23, 59, 58, 0);
           _ = DllImports.mdf_message_add_time3(_messageHandle, Tag, 0, 0, 0, 1);
           _ = DllImports.mdf_message_add_time3(_messageHandle, Tag, 23, 59, 59, 1);
           _ = DllImports.mdf_message_add_time3(_messageHandle, Tag, 23, 59, 59, 999999999);
            DllImports.mdf_message_reset(_messageHandle);
        }

        [Benchmark(OperationsPerInvoke = 7)]
        public unsafe void AddTime3UsingFunctionPointer()
        {
            _ = FunctionPointers.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = FunctionPointers.mdf_message_add_time3(_messageHandle, Tag, 17, 3, 1, 999999999);
            _ = FunctionPointers.mdf_message_add_time3(_messageHandle, Tag, 17, 3, 1, 999);
            _ = FunctionPointers.mdf_message_add_time3(_messageHandle, Tag, 0, 0, 0, 0);
            _ = FunctionPointers.mdf_message_add_time3(_messageHandle, Tag, 23, 59, 58, 0);
            _ = FunctionPointers.mdf_message_add_time3(_messageHandle, Tag, 0, 0, 0, 1);
            _ = FunctionPointers.mdf_message_add_time3(_messageHandle, Tag, 23, 59, 59, 1);
            _ = FunctionPointers.mdf_message_add_time3(_messageHandle, Tag, 23, 59, 59, 999999999);
            FunctionPointers.mdf_message_reset(_messageHandle);
        }
    }
}