using BenchmarkDotNet.Attributes;
using System;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class AddDateBenchmarks : MessageBenchmarks
    {
        private static readonly byte[] s_20201230 = new byte[] { 50, 48, 50, 48, 45, 49, 50, 45, 51, 48, 0 };
        private static readonly byte[] s_202012 = new byte[] { 50, 48, 50, 48, 45, 49, 50, 0 };
        private static readonly byte[] s_2020H1 = new byte[] { 50, 48, 50, 48, 45, 72, 49, 0 };
        private static readonly byte[] s_2020H2 = new byte[] { 50, 48, 50, 48, 45, 72, 50, 0 };
        private static readonly byte[] s_2020T1 = new byte[] { 50, 48, 50, 48, 45, 84, 49, 0 };
        private static readonly byte[] s_2020T2 = new byte[] { 50, 48, 50, 48, 45, 84, 50, 0 };
        private static readonly byte[] s_2020T3 = new byte[] { 50, 48, 50, 48, 45, 84, 51, 0 };
        private static readonly byte[] s_2020Q1 = new byte[] { 50, 48, 50, 48, 45, 81, 49, 0 };
        private static readonly byte[] s_2020Q2 = new byte[] { 50, 48, 50, 48, 45, 81, 50, 0 };
        private static readonly byte[] s_2020Q3 = new byte[] { 50, 48, 50, 48, 45, 81, 51, 0 };
        private static readonly byte[] s_2020Q4 = new byte[] { 50, 48, 50, 48, 45, 81, 52, 0 };
        private static readonly byte[] s_2020W1 = new byte[] { 50, 48, 50, 48, 45, 87, 49, 0 };
        private static readonly byte[] s_2020W52 = new byte[] { 50, 48, 50, 48, 45, 87, 53, 50, 0 };
        private static readonly byte[] s_20060919 = new byte[] { 50, 48, 48, 54, 45, 48, 57, 45, 49, 57, 0 };
        private static readonly byte[] s_200810 = new byte[] { 50, 48, 48, 56, 45, 49, 48, 0 };
        private static readonly byte[] s_2008 = new byte[] { 50, 48, 48, 56, 0 };
        private static readonly byte[] s_2008H1 = new byte[] { 50, 48, 48, 56, 45, 72, 49, 0 };
        private static readonly byte[] s_2008T2 = new byte[] { 50, 48, 48, 56, 45, 84, 50, 0 };
        private static readonly byte[] s_2008Q4 = new byte[] { 50, 48, 48, 56, 45, 81, 52, 0 };
        private static readonly byte[] s_2008W22 = new byte[] { 50, 48, 48, 56, 45, 87, 50, 50, 0 };
        private static readonly byte[] s_2008W53 = new byte[] { 50, 48, 48, 56, 45, 87, 53, 51, 0 };

        [Benchmark(OperationsPerInvoke = 22)]
        public void AddDateString()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddDate(Tag, "2020-12-30");
            _ = _message.AddDate(Tag, "2020-12");
            _ = _message.AddDate(Tag, "2020-H1");
            _ = _message.AddDate(Tag, "2020-H2");
            _ = _message.AddDate(Tag, "2020-T1");
            _ = _message.AddDate(Tag, "2020-T2");
            _ = _message.AddDate(Tag, "2020-T3");
            _ = _message.AddDate(Tag, "2020-Q1");
            _ = _message.AddDate(Tag, "2020-Q2");
            _ = _message.AddDate(Tag, "2020-Q3");
            _ = _message.AddDate(Tag, "2020-Q4");
            _ = _message.AddDate(Tag, "2020-W1");
            _ = _message.AddDate(Tag, "2020-W52");
            _ = _message.AddDate(Tag, "2006-09-19");
            _ = _message.AddDate(Tag, "2008-10");
            _ = _message.AddDate(Tag, "2008");
            _ = _message.AddDate(Tag, "2008-H1");
            _ = _message.AddDate(Tag, "2008-T2");
            _ = _message.AddDate(Tag, "2008-Q4");
            _ = _message.AddDate(Tag, "2008-W22");
            _ = _message.AddDate(Tag, "2008-W53");
            _ = _message.AddDate(Tag, default(string));
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 22)]
        public void AddDateStringUsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-12-30") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-12") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-H1") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-H2") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-T1") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-T2") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-T3") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-Q1") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-Q2") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-Q3") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-Q4") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-W1") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2020-W52") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2006-09-19") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2008-10") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2008") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2008-H1") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2008-T2") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2008-Q4") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2008-W22") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, "2008-W53") == 1;
            _ = DllImports.mdf_message_add_date(_messageHandle, Tag, default) == 1;
            DllImports.mdf_message_reset(_messageHandle);
        }

        [Benchmark(Baseline = true, OperationsPerInvoke = 22)]
        public void AddDateBytes()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddDate(Tag, s_20201230);
            _ = _message.AddDate(Tag, s_202012);
            _ = _message.AddDate(Tag, s_2020H1);
            _ = _message.AddDate(Tag, s_2020H2);
            _ = _message.AddDate(Tag, s_2020T1);
            _ = _message.AddDate(Tag, s_2020T2);
            _ = _message.AddDate(Tag, s_2020T3);
            _ = _message.AddDate(Tag, s_2020Q1);
            _ = _message.AddDate(Tag, s_2020Q2);
            _ = _message.AddDate(Tag, s_2020Q3);
            _ = _message.AddDate(Tag, s_2020Q4);
            _ = _message.AddDate(Tag, s_2020W1);
            _ = _message.AddDate(Tag, s_2020W52);
            _ = _message.AddDate(Tag, s_20060919);
            _ = _message.AddDate(Tag, s_200810);
            _ = _message.AddDate(Tag, s_2008);
            _ = _message.AddDate(Tag, s_2008H1);
            _ = _message.AddDate(Tag, s_2008T2);
            _ = _message.AddDate(Tag, s_2008Q4);
            _ = _message.AddDate(Tag, s_2008W22);
            _ = _message.AddDate(Tag, s_2008W53);
            _ = _message.AddDate(Tag, default(ReadOnlySpan<byte>));
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 9)]
        public void AddDate2()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddDate(Tag, 2020, 12, 30);
            _ = _message.AddDate(Tag, 2006, 9, 19);
            _ = _message.AddDate(Tag, 2008, 10, 0);
            _ = _message.AddDate(Tag, 2008, 0, 0);
            _ = _message.AddDate(Tag, 2008, 0, 1);
            _ = _message.AddDate(Tag, 2008, 0, 2 + 2);
            _ = _message.AddDate(Tag, 2008, 0, 4 + 5);
            _ = _message.AddDate(Tag, 2008, 0, 22 + 9);
            _ = _message.AddDate(Tag, 2008, 13, 53 - 22);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 9)]
        public void AddDate2UsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
             _ = DllImports.mdf_message_add_date2(_messageHandle, Tag, 2020, 12, 30);
             _ = DllImports.mdf_message_add_date2(_messageHandle, Tag, 2006, 9, 19);
             _ = DllImports.mdf_message_add_date2(_messageHandle, Tag, 2008, 10, 0);
             _ = DllImports.mdf_message_add_date2(_messageHandle, Tag, 2008, 0, 0);
             _ = DllImports.mdf_message_add_date2(_messageHandle, Tag, 2008, 0, 1);
             _ = DllImports.mdf_message_add_date2(_messageHandle, Tag, 2008, 0, 2 + 2);
             _ = DllImports.mdf_message_add_date2(_messageHandle, Tag, 2008, 0, 4 + 5);
             _ = DllImports.mdf_message_add_date2(_messageHandle, Tag, 2008, 0, 22 + 9);
             _ = DllImports.mdf_message_add_date2(_messageHandle, Tag, 2008, 13, 53 - 22);
            DllImports.mdf_message_reset(_messageHandle);
        }
    }
}