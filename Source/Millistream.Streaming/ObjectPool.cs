using System;
using System.Threading;

namespace Millistream.Streaming
{
    /// <summary>
    /// Generic implementation of an object pool with a predefined pool size limit, very similar to the implementation used in the "Roslyn" .NET Compiler platform. 
    /// The object pool allows to reuse a managed instance without creating a new one. The purpose is to reduce memory pressure and the number of garbage collections to improve application performance.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the object pool.</typeparam>
    internal class ObjectPool<T> where T : class
    {
        #region Fields
        private readonly Func<T> _objectFactory;
        private readonly Element[] _objects; //Storage for the pooled objects
        private T _firstObject; //A dedicated field that stores the first (and probably only) pooled object
        #endregion

        //This struct is used to wrap pooled objects to prevent the CLR from performing an additional check to ensure array covariance type safety when T is a reference type.
        private struct Element
        {
            internal T value;
        }

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
        /// </summary>
        /// <param name="objectFactory">A factory method that creates a new instance of <typeparamref name="T"/> when needed.</param>
        internal ObjectPool(Func<T> objectFactory) : this(objectFactory, Environment.ProcessorCount * 2) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPool{T}"/> class.
        /// </summary>
        /// <param name="objectFactory">A factory method that creates a new instance of <typeparamref name="T"/> when needed.</param>
        /// <param name="size">The predefined size limit of the object pool.</param>
        internal ObjectPool(Func<T> objectFactory, int size)
        {
            _objectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
            if (size < 1)
                throw new ArgumentException($"{nameof(size)} cannot be smaller than 1.", nameof(size));
            _objects = new Element[size - 1];
        }
        #endregion

        /// <summary>
        /// Creates and returns an instance of <typeparamref name="T"/>, or returns an already existing instance from the pool.
        /// </summary>
        internal T Allocate()
        {
            //Return the first pooled object. The initial read is intentionally not synchronized. 
            //The only consequence of this is that a recently pooled object may be missed, which means that an additional instance is created.
            T instance = _firstObject;
            if (instance == null || instance != Interlocked.CompareExchange(ref _firstObject, null, instance))
            {
                for (int i = 0; i < _objects.Length; i++)
                {
                    instance = _objects[i].value;
                    if (instance != null && instance == Interlocked.CompareExchange(ref _objects[i].value, null, instance))
                        break;
                }
            }
            return instance ?? _objectFactory();
        }

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <param name="object">The instance to be pooled.</param>
        internal void Free(T @object)
        {
            //Insert the item at the first free (null) slot. 
            //Since there is no synchronization implemented, two objects may be stored into the same slot.
            //The only consequence of this is that one of these will be collected by the GC.
            if (_firstObject == null)
            {
                _firstObject = @object;
            }
            else
            {
                for (int i = 0; i < _objects.Length; i++)
                {
                    if (_objects[i].value == null)
                    {
                        _objects[i].value = @object;
                        break;
                    }
                }
            }
        }
    }
}
