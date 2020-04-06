using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class TabularTests
    {
        [TestMethod]
        public void ParseTabularTest()
        {
            ParseTabularTest("1 2 3|4 5 6|7 a 99", 3, 3);
            ParseTabularTest("1  3|4 5 6", 2, 3);

            Tabular tabular = ParseTabularTest(@"1 hello\ world 3|4 5 6", 2, 3);
            Assert.AreEqual(@"hello\ world", tabular.GetData(0, 1).ToString());

            tabular = ParseTabularTest(@"abcd hello\ wor\|\|ld 3|4  6", 2, 3);
            Assert.AreEqual(@"hello\ wor\|\|ld", tabular.GetData(0, 1).ToString());
            Assert.AreEqual(ReadOnlyMemory<char>.Empty, tabular.GetData(1, 1));

            tabular = ParseTabularTest(@"1 2 3 4|4 5 6", 2, 4);
            Assert.AreEqual(ReadOnlyMemory<char>.Empty, tabular.GetData(1, 3));

            tabular = Tabular.Parse(@"1 2 3 4|4 5 6  ");
            Assert.AreEqual(@"1 2 3 4|4 5 6 ", tabular.ToString());

            tabular = ParseTabularTest(@"1 \\ 3|4 5 6", 2, 3);
            Assert.AreEqual(@"\\", tabular.GetData(0, 1).ToString());

            tabular = Tabular.Parse("1 2 3 | 4 5 6 | 7 ");
            Assert.AreEqual("4", tabular.GetData(1, 0).ToString());
            Assert.AreEqual("7", tabular.GetData(2, 0).ToString());
            Assert.AreEqual("1 2 3|4 5 6|7", tabular.ToString());

            tabular = Tabular.Parse(@"1 2 3 | 4 5 6 | 7 some\ long\ sentence");
            Assert.AreEqual(3, tabular.Rows);
            Assert.AreEqual(3, tabular.Columns);
            Assert.AreEqual(@"some\ long\ sentence", tabular.GetData(2, 1).ToString());

            ParseTabularTest(@"1 2 3 4\ 5\6", 1, 4);
            ParseTabularTest(@"xyz", 1, 1);

            tabular = Tabular.Parse(@"xyz\|\||");
            Assert.AreEqual(1, tabular.Rows);
            Assert.AreEqual(1, tabular.Columns);
            Assert.AreEqual(@"xyz\|\|", tabular.ToString());
        }

        [TestMethod]
        public void CompareTabularsTest()
        {
            CompareEqualTabularsTest("");
            CompareEqualTabularsTest("1 2 4| 5 4 7");
            CompareEqualTabularsTest("xyz");
            CompareEqualTabularsTest(@"1 2 3|4 5 6|7 hello\ world | 8");

            Assert.AreEqual(Tabular.Parse("1 2 3 | 4 5 6 | 7 "), Tabular.Parse("1 2 3|4 5 6|7".GetBytes()));

            CompareNonEqualTabularsTest("a", "b");
            CompareNonEqualTabularsTest("1 2 3|4 5 6|7 a 99", "1 2 3|4 5 6|7 a 99|1 1 1");
            CompareNonEqualTabularsTest("1 2 3|4 5 6|7 a 99", "1 2 3|4 5 6|7 b 99");
        }

        private static Tabular ParseTabularTest(string s, int rows, int columns)
        {
            Tabular tabular = Tabular.Parse(s);
            AssertPropertyValues(tabular);
            tabular = Tabular.Parse(s.GetBytes());
            AssertPropertyValues(tabular);

            void AssertPropertyValues(Tabular tabular)
            {
                Assert.AreEqual(rows, tabular.Rows);
                Assert.AreEqual(columns, tabular.Columns);
                Assert.AreEqual(s, tabular.ToString());
            }

            return tabular;
        }

        private static void CompareEqualTabularsTest(string s)
        {
            Tabular left = Tabular.Parse(s);
            Tabular right = Tabular.Parse(s.GetBytes());

            Assert.IsTrue(left.Equals(right));
            Assert.IsTrue(left == right);
            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
            Assert.IsFalse(left != right);
        }

        private static void CompareNonEqualTabularsTest(string a, string b)
        {
            Tabular left = Tabular.Parse(a);
            Tabular right = Tabular.Parse(b);

            Assert.IsFalse(left.Equals(right));
            Assert.IsFalse(left == right);
            Assert.AreNotEqual(left.GetHashCode(), right.GetHashCode());
            Assert.IsTrue(left != right);
        }
    }
}