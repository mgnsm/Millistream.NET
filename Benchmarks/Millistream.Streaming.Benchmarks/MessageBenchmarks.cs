using BenchmarkDotNet.Attributes;
using System;

namespace Millistream.Streaming.Benchmarks
{
    [MemoryDiagnoser]
    public class MessageBenchmarks : IDisposable
    {
        private const uint Tag = 0;
        private readonly Message _message = new();
        private readonly Message _source = new();
        private readonly Message _destination = new();

        public MessageBenchmarks()
        {
            _source.Add(1, 1);
            _source.AddString(Field.MDF_F_ADDRESS, "abc");

            _destination.Add(2, 1);
            _source.AddString(Field.MDF_F_ADDRESS, "def");
        }

        [Benchmark]
        public bool Add() => _message.Add(0, 0);

        [Benchmark(OperationsPerInvoke = 23)]
        public void AddNumericStrings()
        {
            _message.Add(0, 0);
            _message.AddNumeric(Tag, "28");
            _message.AddNumeric(Tag, "283");
            _message.AddNumeric(Tag, "0");
            _message.AddNumeric(Tag, "27");
            _message.AddNumeric(Tag, "0.99");
            _message.AddNumeric(Tag, "255.99");
            _message.AddNumeric(Tag, "10.01");
            _message.AddNumeric(Tag, "103.0001");
            _message.AddNumeric(Tag, "104.857");
            _message.AddNumeric(Tag, "104.8576");
            _message.AddNumeric(Tag, "0.0001");
            _message.AddNumeric(Tag, "10741.76");
            _message.AddNumeric(Tag, "256.01");
            _message.AddNumeric(Tag, "284");
            _message.AddNumeric(Tag, "2000");
            _message.AddNumeric(Tag, "20000000000000000");
            _message.AddNumeric(Tag, "18446744073709551610");
            _message.AddNumeric(Tag, "18446.744073709551615");
            _message.AddNumeric(Tag, "1.0000001");
            _message.AddNumeric(Tag, "0.000000000000001");
            _message.AddNumeric(Tag, "-0.000000000000001");
            _message.AddNumeric(Tag, "-1.0000001");
            _message.AddNumeric(Tag, "-18446.744073709551615");
            _message.Reset();
        }

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
        private static readonly byte[] s_Minus0Point000000000000001 = new byte[] { 45, 48, 46, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 48, 49, 0 };
        private static readonly byte[] s_Minus1Point0000001 = new byte[] { 45, 49, 46, 48, 48, 48, 48, 48, 48, 49, 0 };
        private static readonly byte[] s_Minus18446Point744073709551615 = new byte[] { 45, 49, 56, 52, 52, 54, 46, 55, 52, 52, 48, 55, 51, 55, 48, 57, 53, 53, 49, 54, 49, 53, 0 };
        [Benchmark(OperationsPerInvoke = 23)]
        public void AddNumericBytes()
        {
            _message.Add(0, 0);
            _message.AddNumeric(Tag, s_28);
            _message.AddNumeric(Tag, s_283);
            _message.AddNumeric(Tag, s_0);
            _message.AddNumeric(Tag, s_27);
            _message.AddNumeric(Tag, s_0Point99);
            _message.AddNumeric(Tag, s_255Point99);
            _message.AddNumeric(Tag, s_10Point01);
            _message.AddNumeric(Tag, s_103Point0001);
            _message.AddNumeric(Tag, s_104Point857);
            _message.AddNumeric(Tag, s_104Point8576);
            _message.AddNumeric(Tag, s_0Point0001);
            _message.AddNumeric(Tag, s_10741Point76);
            _message.AddNumeric(Tag, s_256Point01);
            _message.AddNumeric(Tag, s_284);
            _message.AddNumeric(Tag, s_2000);
            _message.AddNumeric(Tag, s_20000000000000000);
            _message.AddNumeric(Tag, s_18446744073709551610);
            _message.AddNumeric(Tag, s_18446Point744073709551615);
            _message.AddNumeric(Tag, s_1Point0000001);
            _message.AddNumeric(Tag, s_0Point000000000000001);
            _message.AddNumeric(Tag, s_Minus0Point000000000000001);
            _message.AddNumeric(Tag, s_Minus1Point0000001);
            _message.AddNumeric(Tag, s_Minus18446Point744073709551615);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 20)]
        public void AddInt64()
        {
            _message.Add(0, 0);
            _message.AddInt64(Tag, -12345L, 2);
            _message.AddInt64(Tag, 28L, 0);
            _message.AddInt64(Tag, 283L, 0);
            _message.AddInt64(Tag, 0L, 0);
            _message.AddInt64(Tag, 27L, 0);
            _message.AddInt64(Tag, 99L, 2);
            _message.AddInt64(Tag, 25599L, 2);
            _message.AddInt64(Tag, 1001L, 2);
            _message.AddInt64(Tag, 1030001L, 4);
            _message.AddInt64(Tag, 104857L, 3);
            _message.AddInt64(Tag, 1048576L, 4);
            _message.AddInt64(Tag, 1L, 4);
            _message.AddInt64(Tag, 1074176L, 2);
            _message.AddInt64(Tag, 25601L, 2);
            _message.AddInt64(Tag, 284L, 0);
            _message.AddInt64(Tag, 2000L, 0);
            _message.AddInt64(Tag, 20000000000000000L, 0);
            _message.AddInt64(Tag, -10000001L, 7);
            _message.AddInt64(Tag, 10000001L, 7);
            _message.AddInt64(Tag, 1L, 15);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 21)]
        public void AddUInt64()
        {
            _message.Add(0, 0);
            _message.AddUInt64(Tag, 12345UL, 2);
            _message.AddUInt64(Tag, 28UL, 0);
            _message.AddUInt64(Tag, 283UL, 0);
            _message.AddUInt64(Tag, 0UL, 0);
            _message.AddUInt64(Tag, 27UL, 0);
            _message.AddUInt64(Tag, 99UL, 2);
            _message.AddUInt64(Tag, 25599UL, 2);
            _message.AddUInt64(Tag, 1001UL, 2);
            _message.AddUInt64(Tag, 1030001UL, 4);
            _message.AddUInt64(Tag, 104857UL, 3);
            _message.AddUInt64(Tag, 1048576UL, 4);
            _message.AddUInt64(Tag, 1UL, 4);
            _message.AddUInt64(Tag, 1074176UL, 2);
            _message.AddUInt64(Tag, 25601UL, 2);
            _message.AddUInt64(Tag, 284UL, 0);
            _message.AddUInt64(Tag, 2000UL, 0);
            _message.AddUInt64(Tag, 20000000000000000UL, 0);
            _message.AddUInt64(Tag, 18446744073709551610UL, 0);
            _message.AddUInt64(Tag, 18446744073709551615UL, 15);
            _message.AddUInt64(Tag, 10000001UL, 7);
            _message.AddUInt64(Tag, 1UL, 15);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 7)]
        public void AddString()
        {
            _message.Add(0, 0);
            _message.AddString(Tag, "Foo Bar");
            _message.AddString(Tag, "aaaBBBccc");
            _message.AddString(Tag, "SE0000108656");
            _message.AddString(Tag, "åäöÅÄÖæß");
            _message.AddString(Tag, "12-------P----");
            _message.AddString(Tag, "46--X-B--P-3--");
            _message.AddString(Tag, default(string));
            _message.Reset();
        }

        private static readonly byte[] s_FooBar = new byte[] { 70, 111, 111, 32, 66, 97, 114, 0 };
        private static readonly byte[] s_aaaBBBccc = new byte[] { 97, 97, 97, 66, 66, 66, 99, 99, 99, 0 };
        private static readonly byte[] s_SE0000108656 = new byte[] { 83, 69, 48, 48, 48, 48, 49, 48, 56, 54, 53, 54, 0 };
        private static readonly byte[] s_åäöÅÄÖ = new byte[] { 195, 165, 195, 164, 195, 182, 195, 133, 195, 132, 195, 150, 195, 166, 195, 159, 226, 130, 172, 194, 181, 0 };
        private static readonly byte[] s_12P = new byte[] { 49, 50, 45, 45, 45, 45, 45, 45, 45, 80, 45, 45, 45, 45, 0 };
        private static readonly byte[] s_46XBP3 = new byte[] { 52, 54, 45, 45, 88, 45, 66, 45, 45, 80, 45, 51, 45, 45, 0 };
        [Benchmark(OperationsPerInvoke = 7)]
        public void AddStringBytes()
        {
            _message.Add(0, 0);
            _message.AddString(Tag, s_FooBar);
            _message.AddString(Tag, s_aaaBBBccc);
            _message.AddString(Tag, s_SE0000108656);
            _message.AddString(Tag, s_åäöÅÄÖ);
            _message.AddString(Tag, s_12P);
            _message.AddString(Tag, s_46XBP3);
            _message.AddString(Tag, default(ReadOnlySpan<byte>));
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 7)]
        public void AddString2()
        {
            _message.Add(0, 0);
            _message.AddString(Tag, "Foo Bar", 7);
            _message.AddString(Tag, "aaaBBBccc", 9);
            _message.AddString(Tag, "SE0000108656", 12);
            _message.AddString(Tag, "åäöÅÄÖæß", 8);
            _message.AddString(Tag, "12-------P----", 14);
            _message.AddString(Tag, "46--X-B--P-3--", 14);
            _message.AddString(Tag, default(string), 10);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 7)]
        public void AddString2Bytes()
        {
            _message.Add(0, 0);
            _message.AddString(Tag, s_FooBar, 7);
            _message.AddString(Tag, s_aaaBBBccc, 9);
            _message.AddString(Tag, s_SE0000108656, 12);
            _message.AddString(Tag, s_åäöÅÄÖ, 21);
            _message.AddString(Tag, s_12P, 14);
            _message.AddString(Tag, s_46XBP3, 14);
            _message.AddString(Tag, default(ReadOnlySpan<byte>));
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 22)]
        public void AddDateStrings()
        {
            _message.Add(0, 0);
            _message.AddDate(Tag, "2020-12-30");
            _message.AddDate(Tag, "2020-12");
            _message.AddDate(Tag, "2020-H1");
            _message.AddDate(Tag, "2020-H2");
            _message.AddDate(Tag, "2020-T1");
            _message.AddDate(Tag, "2020-T2");
            _message.AddDate(Tag, "2020-T3");
            _message.AddDate(Tag, "2020-Q1");
            _message.AddDate(Tag, "2020-Q2");
            _message.AddDate(Tag, "2020-Q3");
            _message.AddDate(Tag, "2020-Q4");
            _message.AddDate(Tag, "2020-W1");
            _message.AddDate(Tag, "2020-W52");
            _message.AddDate(Tag, "2006-09-19");
            _message.AddDate(Tag, "2008-10");
            _message.AddDate(Tag, "2008");
            _message.AddDate(Tag, "2008-H1");
            _message.AddDate(Tag, "2008-T2");
            _message.AddDate(Tag, "2008-Q4");
            _message.AddDate(Tag, "2008-W22");
            _message.AddDate(Tag, "2008-W53");
            _message.AddDate(Tag, default(string));
            _message.Reset();
        }

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
        public void AddDateBytes()
        {
            _message.Add(0, 0);
            _message.AddDate(Tag, s_20201230);
            _message.AddDate(Tag, s_202012);
            _message.AddDate(Tag, s_2020H1);
            _message.AddDate(Tag, s_2020H2);
            _message.AddDate(Tag, s_2020T1);
            _message.AddDate(Tag, s_2020T2);
            _message.AddDate(Tag, s_2020T3);
            _message.AddDate(Tag, s_2020Q1);
            _message.AddDate(Tag, s_2020Q2);
            _message.AddDate(Tag, s_2020Q3);
            _message.AddDate(Tag, s_2020Q4);
            _message.AddDate(Tag, s_2020W1);
            _message.AddDate(Tag, s_2020W52);
            _message.AddDate(Tag, s_20060919);
            _message.AddDate(Tag, s_200810);
            _message.AddDate(Tag, s_2008);
            _message.AddDate(Tag, s_2008H1);
            _message.AddDate(Tag, s_2008T2);
            _message.AddDate(Tag, s_2008Q4);
            _message.AddDate(Tag, s_2008W22);
            _message.AddDate(Tag, s_2008W53);
            _message.AddDate(Tag, default(ReadOnlySpan<byte>));
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 9)]
        public void AddDate2()
        {
            _message.Add(0, 0);
            _message.AddDate(Tag, 2020, 12, 30);
            _message.AddDate(Tag, 2006, 9, 19);
            _message.AddDate(Tag, 2008, 10, 0);
            _message.AddDate(Tag, 2008, 0, 0);
            _message.AddDate(Tag, 2008, 0, 1);
            _message.AddDate(Tag, 2008, 0, 2 + 2);
            _message.AddDate(Tag, 2008, 0, 4 + 5);
            _message.AddDate(Tag, 2008, 0, 22 + 9);
            _message.AddDate(Tag, 2008, 13, 53 - 22);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 8)]
        public void AddTimeStrings()
        {
            _message.Add(0, 0);
            _message.AddTime(Tag, "17:03:01");
            _message.AddTime(Tag, "17:03:01.999");
            _message.AddTime(Tag, "00:00:00");
            _message.AddTime(Tag, "23:59:58");
            _message.AddTime(Tag, "23:59:59.001");
            _message.AddTime(Tag, "23:59:59.999");
            _message.AddTime(Tag, "23:59:59.000000001");
            _message.AddTime(Tag, "23:59:59.999999999");
            _message.Reset();
        }

        private static readonly byte[] s_170301 = new byte[] { 49, 55, 58, 48, 51, 58, 48, 49, 0 };
        private static readonly byte[] s_170301999 = new byte[] { 49, 55, 58, 48, 51, 58, 48, 49, 46, 57, 57, 57, 0 };
        private static readonly byte[] s_000000 = new byte[] { 48, 48, 58, 48, 48, 58, 48, 48, 0 };
        private static readonly byte[] s_235958 = new byte[] { 50, 51, 58, 53, 57, 58, 53, 56, 0 };
        private static readonly byte[] s_235959001 = new byte[] { 50, 51, 58, 53, 57, 58, 53, 57, 46, 48, 48, 49, 0 };
        private static readonly byte[] s_235959999 = new byte[] { 50, 51, 58, 53, 57, 58, 53, 57, 46, 57, 57, 57, 0 };
        private static readonly byte[] s_235959000000001 = new byte[] { 50, 51, 58, 53, 57, 58, 53, 57, 46, 48, 48, 48, 48, 48, 48, 48, 48, 49, 0 };
        private static readonly byte[] s_235959999999999 = new byte[] { 50, 51, 58, 53, 57, 58, 53, 57, 46, 57, 57, 57, 57, 57, 57, 57, 57, 57, 0 };
        [Benchmark(OperationsPerInvoke = 8)]
        public void AddTimeBytes()
        {
            _message.Add(0, 0);
            _message.AddTime(Tag, s_170301);
            _message.AddTime(Tag, s_170301999);
            _message.AddTime(Tag, s_000000);
            _message.AddTime(Tag, s_235958);
            _message.AddTime(Tag, s_235959001);
            _message.AddTime(Tag, s_235959999);
            _message.AddTime(Tag, s_235959000000001);
            _message.AddTime(Tag, s_235959999999999);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 5)]
        public void AddTime2()
        {
            _message.Add(0, 0);
            _message.AddTime2(Tag, 17, 3, 1, 999);
            _message.AddTime2(Tag, 0, 0, 0, 0);
            _message.AddTime2(Tag, 23, 59, 58, 0);
            _message.AddTime2(Tag, 23, 59, 59, 1);
            _message.AddTime2(Tag, 23, 59, 59, 999);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 7)]
        public void AddTime3()
        {
            _message.Add(0, 0);
            _message.AddTime3(Tag, 17, 3, 1, 999999999);
            _message.AddTime3(Tag, 17, 3, 1, 999);
            _message.AddTime3(Tag, 0, 0, 0, 0);
            _message.AddTime3(Tag, 23, 59, 58, 0);
            _message.AddTime3(Tag, 0, 0, 0, 1);
            _message.AddTime3(Tag, 23, 59, 59, 1);
            _message.AddTime3(Tag, 23, 59, 59, 999999999);
            _message.Reset();
        }

        [Benchmark(OperationsPerInvoke = 13)]
        public void AddListStrings()
        {
            _message.Add(0, 0);
            _message.AddList(Tag, "28");
            _message.AddList(Tag, "28 28");
            _message.AddList(Tag, "28 28 343");
            _message.AddList(Tag, "=28");
            _message.AddList(Tag, "=28 28");
            _message.AddList(Tag, "=28 28 343");
            _message.AddList(Tag, "+28");
            _message.AddList(Tag, "+28 28");
            _message.AddList(Tag, "+28 28 343");
            _message.AddList(Tag, "-28");
            _message.AddList(Tag, "-28 28");
            _message.AddList(Tag, "-28 28 343");
            _message.AddList(Tag, default(string));
            _message.Reset();
        }

        private static readonly byte[] s_2828 = new byte[] { 50, 56, 32, 50, 56, 0 };
        private static readonly byte[] s_2828343 = new byte[] { 50, 56, 32, 50, 56, 32, 51, 52, 51, 0 };
        private static readonly byte[] s_Equals28 = new byte[] { 61, 50, 56, 0 };
        private static readonly byte[] s_Equals2828 = new byte[] { 61, 50, 56, 32, 50, 56, 0 };
        private static readonly byte[] s_Equals2828343 = new byte[] { 61, 50, 56, 32, 50, 56, 32, 51, 52, 51, 0 };
        private static readonly byte[] s_Plus28 = new byte[] { 43, 50, 56, 0 };
        private static readonly byte[] s_Plus2828 = new byte[] { 43, 50, 56, 32, 50, 56, 0 };
        private static readonly byte[] s_Plus2828343 = new byte[] { 43, 50, 56, 32, 50, 56, 32, 51, 52, 51, 0 };
        private static readonly byte[] s_Minus28 = new byte[] { 45, 50, 56, 0 };
        private static readonly byte[] s_Minus2828 = new byte[] { 45, 50, 56, 32, 50, 56, 0 };
        private static readonly byte[] s_Minus2828343 = new byte[] { 45, 50, 56, 32, 50, 56, 32, 51, 52, 51, 0 };
        [Benchmark(OperationsPerInvoke = 13)]
        public void AddListBytes()
        {
            _message.Add(0, 0);
            _message.AddList(Tag, s_28);
            _message.AddList(Tag, s_2828);
            _message.AddList(Tag, s_2828343);
            _message.AddList(Tag, s_Equals28);
            _message.AddList(Tag, s_Equals2828);
            _message.AddList(Tag, s_Equals2828343);
            _message.AddList(Tag, s_Plus28);
            _message.AddList(Tag, s_Plus2828);
            _message.AddList(Tag, s_Plus2828343);
            _message.AddList(Tag, s_Minus28);
            _message.AddList(Tag, s_Minus2828);
            _message.AddList(Tag, s_Minus2828343);
            _message.AddList(Tag, default(ReadOnlySpan<byte>));
            _message.Reset();
        }

        [Benchmark]
        public void Reset() => _message.Reset();

        [Benchmark]
        public void Delete() => _message.Delete();

        [Benchmark]
        public bool Move() => Message.Move(_source, _destination, 1, 2);

        [Benchmark]
        public bool Serialize() => _message.Serialize(out IntPtr _);

        [Benchmark]
        public bool Deserialize() => _message.Deserialize("AQAAAAIAAAADAAAAAAAAAAAAAAAAAAAABQAAAAEAAAAAAAAAAAAAABMAAAABAAAAAAAAAAAAAABLAAAAAgAAAAIAAAAAAAAAMwAAAAEAAAAAX2U=");

        [Benchmark]
        public void SetCompressionLevel() => _message.CompressionLevel = CompressionLevel.Z_BEST_COMPRESSION;

        [Benchmark]
        public int GetCount() => _message.Count;

        [Benchmark]
        public int GetActiveCount() => _message.ActiveCount;

        [Benchmark]
        public void SetUtf8Validation() => _message.Utf8Validation = false;

        [Benchmark]
        public void Destroy() => _message.Dispose();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _message.Dispose();
                _source.Dispose();
                _destination.Dispose();
            }
        }
    }
}