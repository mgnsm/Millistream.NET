using BenchmarkDotNet.Attributes;

namespace Millistream.Streaming.Benchmarks.Message
{
    public class AddNumericBenchmarks : MessageBenchmarks
    {
        private static readonly byte[] s_28 = new byte[] { 50, 56, 0 };
        private static readonly byte[] s_283 = new byte[] { 50, 56, 51, 0 };
        private static readonly byte[] s_0 = new byte[] { 48, 0 };
        private static readonly byte[] s_27 = new byte[] { 50, 55, 0 };
        private static readonly byte[] s_0Point99 = new byte[] { 48, 46, 57, 57, 0 };
        private static readonly byte[] s_255Point99 = new byte[] { 50, 53, 53, 46, 57, 57, 0 };
        private static readonly byte[] s_10Point01 = new byte[] { 49, 48, 46, 48, 49, 0 };
        private static readonly byte[] s_103Point0001 = new byte[] { 49, 48, 51, 46, 48, 48, 48, 49, 0 };
        private static readonly byte[] s_104Point857 = new byte[] { 49, 48, 52, 46, 56, 53, 55, 0 };
        private static readonly byte[] s_104Point8576 = new byte[] { 49, 48, 52, 46, 56, 53, 55, 54, 0 };
        private static readonly byte[] s_0Point0001 = new byte[] { 48, 46, 48, 48, 48, 49, 0 };
        private static readonly byte[] s_10741Point76 = new byte[] { 49, 48, 55, 52, 49, 46, 55, 54, 0 };
        private static readonly byte[] s_256Point01 = new byte[] { 50, 53, 54, 46, 48, 49, 0 };
        private static readonly byte[] s_284 = new byte[] { 50, 56, 52, 0 };
        private static readonly byte[] s_2000 = new byte[] { 50, 48, 48, 48, 0 };
        private static readonly byte[] s_20000000000000000 = new byte[] { 50, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 0 };
        private static readonly byte[] s_18446744073709551610 = new byte[] { 49, 56, 52, 52, 54, 55, 52, 52, 48, 55, 51, 55, 48, 57, 53, 53, 49, 54, 49, 48, 0 };
        private static readonly byte[] s_18446Point744073709551615 = new byte[] { 49, 56, 52, 52, 54, 46, 55, 52, 52, 48, 55, 51, 55, 48, 57, 53, 53, 49, 54, 49, 53, 0 };
        private static readonly byte[] s_1Point0000001 = new byte[] { 49, 46, 48, 48, 48, 48, 48, 48, 49, 0 };
        private static readonly byte[] s_0Point000000000000001 = new byte[] { 48, 46, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 49, 0 };
        private static readonly byte[] s_minus0Point000000000000001 = new byte[] { 45, 48, 46, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 49, 0 };
        private static readonly byte[] s_minus1Point0000001 = new byte[] { 45, 49, 46, 48, 48, 48, 48, 48, 48, 49, 0 };
        private static readonly byte[] s_minus18446Point744073709551615 = new byte[] { 45, 49, 56, 52, 52, 54, 46, 55, 52, 52, 48, 55, 51, 55, 48, 57, 53, 53, 49, 54, 49, 53, 0 };

        [Benchmark(OperationsPerInvoke = 23)]
        public void AddNumericString()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddNumeric(Tag, "28");
            _ = _message.AddNumeric(Tag, "283");
            _ = _message.AddNumeric(Tag, "0");
            _ = _message.AddNumeric(Tag, "27");
            _ = _message.AddNumeric(Tag, "0.99");
            _ = _message.AddNumeric(Tag, "255.99");
            _ = _message.AddNumeric(Tag, "10.01");
            _ = _message.AddNumeric(Tag, "103.0001");
            _ = _message.AddNumeric(Tag, "104.857");
            _ = _message.AddNumeric(Tag, "104.8576");
            _ = _message.AddNumeric(Tag, "0.0001");
            _ = _message.AddNumeric(Tag, "10741.76");
            _ = _message.AddNumeric(Tag, "256.01");
            _ = _message.AddNumeric(Tag, "284");
            _ = _message.AddNumeric(Tag, "2000");
            _ = _message.AddNumeric(Tag, "20000000000000000");
            _ = _message.AddNumeric(Tag, "18446744073709551610");
            _ = _message.AddNumeric(Tag, "18446.744073709551615");
            _ = _message.AddNumeric(Tag, "1.0000001");
            _ = _message.AddNumeric(Tag, "0.000000000000001");
            _ = _message.AddNumeric(Tag, "-0.000000000000001");
            _ = _message.AddNumeric(Tag, "-1.0000001");
            _ = _message.AddNumeric(Tag, "-18446.744073709551615");
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 23)]
        public void AddNumericStringUsingDllImport()
        {
            _ = DllImports.mdf_message_add(_messageHandle, 0, 0) == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "28") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "283") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "0") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "27") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "0.99") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "255.99") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "10.01") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "103.0001") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "104.857") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "104.8576") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "0.0001") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "10741.76") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "256.01") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "284") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "2000") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "20000000000000000") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "18446744073709551610") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "18446.744073709551615") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "1.0000001") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "0.000000000000001") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "-0.000000000000001") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "-1.0000001") == 1;
            _ = DllImports.mdf_message_add_numeric(_messageHandle, Tag, "-18446.744073709551615") == 1;
            DllImports.mdf_message_reset(_messageHandle);
        }

        [Benchmark(Baseline = true, OperationsPerInvoke = 23)]
        public void AddNumericBytes()
        {
            _ = _message.Add(0, 0);
            _ = _message.AddNumeric(Tag, s_28);
            _ = _message.AddNumeric(Tag, s_283);
            _ = _message.AddNumeric(Tag, s_0);
            _ = _message.AddNumeric(Tag, s_27);
            _ = _message.AddNumeric(Tag, s_0Point99);
            _ = _message.AddNumeric(Tag, s_255Point99);
            _ = _message.AddNumeric(Tag, s_10Point01);
            _ = _message.AddNumeric(Tag, s_103Point0001);
            _ = _message.AddNumeric(Tag, s_104Point857);
            _ = _message.AddNumeric(Tag, s_104Point8576);
            _ = _message.AddNumeric(Tag, s_0Point0001);
            _ = _message.AddNumeric(Tag, s_10741Point76);
            _ = _message.AddNumeric(Tag, s_256Point01);
            _ = _message.AddNumeric(Tag, s_284);
            _ = _message.AddNumeric(Tag, s_2000);
            _ = _message.AddNumeric(Tag, s_20000000000000000);
            _ = _message.AddNumeric(Tag, s_18446744073709551610);
            _ = _message.AddNumeric(Tag, s_18446Point744073709551615);
            _ = _message.AddNumeric(Tag, s_1Point0000001);
            _ = _message.AddNumeric(Tag, s_0Point000000000000001);
            _ = _message.AddNumeric(Tag, s_minus0Point000000000000001);
            _ = _message.AddNumeric(Tag, s_minus1Point0000001);
            _ = _message.AddNumeric(Tag, s_minus18446Point744073709551615);
            _message.Reset();
        }
    }
}