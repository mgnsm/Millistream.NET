using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Millistream.Streaming.UnitTests
{
    [TestClass]
    public class ObjectPoolTests
    {
        private const int N = 10;

        [TestMethod]
        public void DoNotRecycleObjectsTest()
        {
            int objectCounter = 0;
            ResponseMessage factory()
            {
                objectCounter++;
                return new ResponseMessage();
            };

            ObjectPool<ResponseMessage> objectPool = new ObjectPool<ResponseMessage>(factory);
            //allocated some objects without recycle them and assert that the factory is invoked each time
            for (int i = 1; i <= N; ++i)
            {
                objectPool.Allocate();
                Assert.AreEqual(objectCounter, i);
            }
        }

        [TestMethod]
        public void RecycleObjectsTest()
        {
            int objectCounter = 0;
            ResponseMessage factory()
            {
                objectCounter++;
                return new ResponseMessage();
            };

            ObjectPool<ResponseMessage> objectPool = new ObjectPool<ResponseMessage>(factory);
            for (int i = 1; i <= N; ++i)
            {
                ResponseMessage @object = objectPool.Allocate();
                objectPool.Free(@object);
                //assert that only one object is constructed
                Assert.AreEqual(objectCounter, 1);
            }
        }

        [TestMethod]
        public void PreAllocateObjectsTest()
        {
            int objectCounter = 0;
            ResponseMessage factory()
            {
                objectCounter++;
                return new ResponseMessage();
            };

            ObjectPool<ResponseMessage> objectPool = new ObjectPool<ResponseMessage>(factory, N);
            //pre-allocate N objects
            for (int i = 1; i <= N; ++i)
                objectPool.Free(new ResponseMessage());
            //...and assert that these are returned from the Allocate method
            for (int i = 1; i <= N; ++i)
                objectPool.Allocate();
            Assert.AreEqual(objectCounter, 0);
        }
    }
}
