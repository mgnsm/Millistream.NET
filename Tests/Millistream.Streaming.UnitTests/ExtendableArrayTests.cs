using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Millistream.Streaming.UnitTests
{
    [TestClass]
    public class ExtendableArrayTests
    {
        [TestMethod]
        public void CreateEmptyExtendableArrayTest()
        {
            ExtendableArray<ResponseMessage> extendableArray = new ExtendableArray<ResponseMessage>();
            Assert.IsNotNull(extendableArray.Items);
            Assert.AreEqual(0, extendableArray.Items.Length);
        }

        [TestMethod]
        public void ExtendExtendableArrayTest()
        {
            ExtendableArray<ResponseMessage> extendableArray = new ExtendableArray<ResponseMessage>();
            for (int i = 0; i < ExtendableArray<ResponseMessage>.DefaultCapacity; i++)
            {
                extendableArray.Add(new ResponseMessage());
                Assert.AreEqual(ExtendableArray<ResponseMessage>.DefaultCapacity, extendableArray.Items.Length);
            }
            //assert that the capacity is doubled when the array is full
            extendableArray.Add(new ResponseMessage());
            int expectedLength = ExtendableArray<ResponseMessage>.DefaultCapacity * 2;
            Assert.AreEqual(expectedLength, extendableArray.Items.Length);

            for (int i = ExtendableArray<ResponseMessage>.DefaultCapacity + 2; i < 100; i++)
            {
                extendableArray.Add(new ResponseMessage());
                if (i > expectedLength)
                    expectedLength *= 2;
                Assert.AreEqual(expectedLength, extendableArray.Items.Length);
                Assert.AreEqual(expectedLength, extendableArray.Capacity);
            }
        }

        [TestMethod]
        public void ClearExtendableArrayTest()
        {
            ExtendableArray<ResponseMessage> extendableArray = new ExtendableArray<ResponseMessage>();
            extendableArray.Add(new ResponseMessage());
            extendableArray.Add(new ResponseMessage());
            extendableArray.Add(new ResponseMessage());
            extendableArray.Clear();
            for (int i = 0; i < extendableArray.Items.Length; i++)
                Assert.IsNull(extendableArray.Items[i]);
        }
    }
}