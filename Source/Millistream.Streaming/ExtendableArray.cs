using System;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    /// <summary>
    /// Implements a variable-size array to which you can add items.
    /// As items are added, the capacity of the array is automatically increased as required by reallocating the internal array.
    /// </summary>
    /// <typeparam name="T">The type of items that are stored in the array.</typeparam>
    internal sealed class ExtendableArray<T>
    {
        #region Fields
        internal const int DefaultCapacity = 4;
        private static readonly T[] s_emptyArray = new T[0];
        private int _size;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of the <see cref="ExtendableArray{T}"/> class. The array is initially empty and has a capacity of zero. 
        /// Upon adding the first element to the capacity is increased to DefaultCapacity, and then increased in multiples of two as required.
        /// </summary>
        internal ExtendableArray() => Items = s_emptyArray;
        #endregion

        #region Properties
        /// <summary>
        /// Gets and sets the capacity of array. The capacity is the size of the internal array used to hold items.  
        /// When set, the internal array is reallocated to the given capacity.
        /// </summary>
        internal int Capacity
        {
            get => Items.Length;
            set
            {
                if (value < _size)
                    throw new ArgumentOutOfRangeException();

                if (value != Items.Length)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (_size > 0)
                            Array.Copy(Items, 0, newItems, 0, _size);
                        Items = newItems;
                    }
                    else
                    {
                        Items = s_emptyArray;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the underlying array of items.
        /// </summary>
        internal T[] Items { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the given item to the end of the array. The size of the array is increased by one. 
        /// If required, the capacity is doubled before adding the new element.
        /// <param name="item">The item to be added to the array.</param>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Add(T item)
        {
            T[] array = Items;
            int size = _size;
            if ((uint)size < (uint)array.Length)
            {
                _size = size + 1;
                array[size] = item;
            }
            else
            {
                AddWithResize(item);
            }
        }

        /// <summary>
        /// Clears the contents of the array.
        /// </summary>
        internal void Clear()
        {
            int size = _size;
            if (size > 0)
                Array.Clear(Items, 0, size);
            _size = 0;
        }

        // Non-inline from List.Add to improve its code quality as uncommon path.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithResize(T item)
        {
            int size = _size;
            EnsureCapacity(size + 1);
            _size = size + 1;
            Items[size] = item;
        }

        // Ensures that the size of _items is at least the given minimum value.
        // If the current capacity is less than min, the capacity is increased to twice the current capacity or to min, whichever is larger.
        private void EnsureCapacity(int min)
        {
            if (Items.Length < min)
            {
                int newCapacity = Items.Length == 0 ? DefaultCapacity : Items.Length * 2;
                // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
                // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
                if ((uint)newCapacity > int.MaxValue)
                    newCapacity = int.MaxValue;
                if (newCapacity < min)
                    newCapacity = min;
                Capacity = newCapacity;
            }
        }
        #endregion
    }
}