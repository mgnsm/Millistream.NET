using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Millistream.Streaming.DataTypes.UnitTests
{
    [TestClass]
    public class StringTests
    {
        [TestMethod]
        public void CreateStringTest()
        {
            CreateStringTest("1");
            CreateStringTest("a");
            CreateStringTest("/");
            CreateStringTest("&");
            CreateStringTest(",,");
            CreateStringTest("123.456.918.xxx");
            CreateStringTest("@*-?+-;&&&%%%");

            String @string;
            Assert.AreEqual(0, @string.Length);

            @string = new String("abc");
            Assert.AreEqual(3, @string.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareStringToObjectOfAnotherTypeTest()
        {
            String @string = new String("x");
            @string.CompareTo("x");
        }

        [TestMethod]
        public void CompareStringsTest()
        {
            String @string = new String("e");
            Assert.AreEqual(1, @string.CompareTo(null));
            Assert.AreEqual(0, @string.CompareTo(new String("e".GetBytes())));
            Assert.AreEqual(-1, @string.CompareTo(new String("f")));
            Assert.AreEqual(1, @string.CompareTo(new String("d")));
            Assert.AreEqual(-1, @string.CompareTo(new String("E")));

            Assert.IsTrue(new String("abc").Equals(new String("abc")));
            Assert.IsFalse(new String("abc").Equals(new String("ABC")));
            Assert.IsTrue(new String("abc") == new String("abc"));
            Assert.IsFalse(new String("abc") != new String("abc"));
            Assert.IsFalse(new String("abc").Equals("abc"));
            Assert.IsFalse(new String("def").Equals(1));

            Assert.AreEqual(new String().GetHashCode(), new String().GetHashCode());
            Assert.AreEqual(new String("x").GetHashCode(), new String("x").GetHashCode());
            Assert.AreNotEqual(new String("x").GetHashCode(), new String("y").GetHashCode());
            Assert.AreNotEqual(new String().GetHashCode(), new String("").GetHashCode());
        }

        [TestMethod]
        public void StringToStringTest()
        {
            String @string = new String("test");
            Assert.AreEqual("test", @string.ToString());
        }

        [TestMethod]
        public void StringGetEnumeratorTest()
        {
            Span<char> chars = stackalloc char[4] { 't', 'e', 's', 't' };
            String @string = new String(chars);
            int index = 0;
            foreach (char c in @string)
                Assert.AreEqual(chars[index++], c);

            @string = new String();
            Assert.IsNull(@string.GetEnumerator());
        }

        [TestMethod]
        public void CloneStringTest()
        {
            String @string = new String("test");
            Assert.AreEqual(@string, @string.Clone());
        }

        [TestMethod]
        public void StringIndexerTest()
        {
            String @string = new String("value");
            Assert.AreEqual('a', @string[1]);
            Assert.AreEqual('e', @string[^1]);
        }

        [TestMethod]
        public void StringToSpanTest()
        {
            String @string = new String("value");
            ReadOnlySpan<char> span = @string;
            Assert.AreEqual(@string.Length, span.Length);
        }


        private static void CreateStringTest(string s)
        {
            String @string = new String(s);
            Assert.AreEqual(s, @string.ToString());
            Assert.AreEqual(s.Length, @string.Length);

            @string = new String(s.GetBytes());
            Assert.AreEqual(s, @string.ToString());
            Assert.AreEqual(s.Length, @string.Length);
        }
    }
}