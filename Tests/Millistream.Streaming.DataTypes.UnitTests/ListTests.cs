using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class ListTests
    {
        [TestMethod]
        public void ParseListTest()
        {
            ParseListTest("45", ListPrefix.Undefined, new InsRef(45));
            ParseListTest("45 67", ListPrefix.Undefined, new InsRef(45), new InsRef(67));
            ParseListTest("45 67 ", ListPrefix.Undefined, new InsRef(45), new InsRef(67));
            ParseListTest("45 ", ListPrefix.Undefined, new InsRef(45));
            ParseListTest("-77777", ListPrefix.Remove, new InsRef(77777));
            ParseListTest("-77777 56 4 1234556788", ListPrefix.Remove, new InsRef(77777), new InsRef(56), new InsRef(4), new InsRef(1234556788));
            ParseListTest("+111 222 ", ListPrefix.Add, new InsRef(111), new InsRef(222));
            ParseListTest("=999", ListPrefix.Replace, new InsRef(999));

            Assert.IsFalse(List.TryParse("45 5b6", out List list));
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(ListPrefix.Undefined, list.Prefix);
            Assert.IsFalse(list.PrefixCharacter.HasValue);
            Assert.AreEqual(string.Empty, list.ToString());

            Assert.IsFalse(List.TryParse("cccc", out _));

            ParseListTest(" 45", ListPrefix.Undefined, new InsRef(45));
            ParseListTest(" 45 567 43 23", ListPrefix.Undefined, new InsRef(45), new InsRef(567), new InsRef(43), new InsRef(23));
            ParseListTest("45 567 43 23 ", ListPrefix.Undefined, new InsRef(45), new InsRef(567), new InsRef(43), new InsRef(23));

            Assert.IsFalse(List.TryParse($"45 {long.MaxValue}0 33", out _));
            Assert.IsFalse(List.TryParse($"/6%5%%%¤#####", out _));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseListWithInvalidCharsTest() => List.Parse("1.3");

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseListWithInvalidBytesTest() => List.Parse("=abc".GetBytes());

        [TestMethod]
        public void CreateListTest()
        {
            List list = new List();
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(ListPrefix.Undefined, list.Prefix);
            Assert.IsFalse(list.PrefixCharacter.HasValue);
            Assert.AreEqual(string.Empty, list.ToString());
            Assert.IsNull(list.GetEnumerator());

            list = new List(new List<InsRef> { new InsRef(1), new InsRef(1000) });
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(ListPrefix.Undefined, list.Prefix);
            Assert.IsFalse(list.PrefixCharacter.HasValue);
            Assert.AreEqual("1 1000", list.ToString());
            Assert.IsNotNull(list.GetEnumerator());
            Assert.AreEqual(new InsRef(1), list[0]);
            Assert.AreEqual(new InsRef(1000), list[1]);

            list = new List(ListPrefix.Add, new List<InsRef> { new InsRef(8000) });
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(ListPrefix.Add, list.Prefix);
            Assert.IsTrue(list.PrefixCharacter.HasValue);
            Assert.AreEqual('+', list.PrefixCharacter);
            Assert.AreEqual("+8000", list.ToString());
            Assert.IsNotNull(list.GetEnumerator());
            Assert.AreEqual(new InsRef(8000), list[0]);
        }

        [TestMethod]
        public void CompareListsTest()
        {
            Assert.AreEqual(List.Parse("100 101 102 4"), List.Parse("100 101 102 4".GetBytes()));
            Assert.AreEqual(List.Parse("100 101 102 4").GetHashCode(), List.Parse("100 101 102 4".GetBytes()).GetHashCode());
            Assert.IsTrue(List.Parse("100 101 102 4") == List.Parse("100 101 102 4".GetBytes()));
            Assert.IsFalse(List.Parse("100 101 102 4") != List.Parse("100 101 102 4".GetBytes()));
            Assert.AreEqual(new List(), new List());
            Assert.AreEqual(new List().GetHashCode(), new List().GetHashCode());
            Assert.AreNotEqual(List.Parse("100 101"), List.Parse("100 101 102".GetBytes()));
            Assert.AreNotEqual(List.Parse("100 101").GetHashCode(), List.Parse("100 101 102".GetBytes()).GetHashCode());
            Assert.AreEqual(List.Parse("18446744073709551615 10"), List.Parse("18446744073709551615 10".GetBytes()));
            Assert.AreEqual(List.Parse("18446744073709551615 10").GetHashCode(), List.Parse("18446744073709551615 10".GetBytes()).GetHashCode());
            Assert.IsFalse(List.Parse("18446744073709551614") == List.Parse("18446744073709551615".GetBytes()));
            Assert.AreNotEqual(List.Parse("18446744073709551614").GetHashCode(), List.Parse("18446744073709551615".GetBytes()).GetHashCode());
            Assert.IsTrue(List.Parse("109 67 45") != List.Parse("109 45 67"));
            Assert.IsFalse(List.Parse("109 67 45").Equals(List.Parse("109 45 67")));
            Assert.AreNotEqual(List.Parse("109 67 45").GetHashCode(), List.Parse("109 45 67").GetHashCode());
            Assert.IsTrue(List.Parse("109 67 45").Equals(List.Parse("109 67 45")));
            Assert.IsFalse(List.Parse("109").Equals(new List<InsRef> { new InsRef(109) }));
        }

        private static void ParseListTest(string s, ListPrefix prefix, params InsRef[] insRefs)
        {
            Assert.IsFalse(string.IsNullOrEmpty(s));
            Assert.IsNotNull(insRefs);

            void Verify(List list)
            {
                Assert.AreEqual(insRefs.Length, list.Count);
                Assert.AreEqual(prefix, list.Prefix);
                if (prefix != ListPrefix.Undefined)
                    Assert.AreEqual(s[0], list.PrefixCharacter);

                Assert.IsNotNull(list.GetEnumerator());
                for (int i = 0; i < insRefs.Length; ++i)
                {
                    InsRef insRef = insRefs[i];
                    Assert.IsTrue(list.Contains(insRef));
                    Assert.AreEqual(list[i], insRef);
                }

                int index = 0;
                foreach (InsRef insRef in list)
                    Assert.AreEqual(insRefs[index++], insRef);

                Assert.AreEqual(s.Trim(), list.ToString());
            }

            Assert.IsTrue(List.TryParse(s, out List list));
            Verify(list);
            Assert.IsTrue(List.TryParse(s.GetBytes(), out list));
            Verify(list);
        }
    }
}