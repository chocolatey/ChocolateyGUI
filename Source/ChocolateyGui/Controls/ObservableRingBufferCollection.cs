// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Chocolatey" file="ObservableRingBufferCollection.cs">
//   Copyright 2014 - Present Rob Reynolds, the maintainers of Chocolatey, and RealDimensions Software, LLC
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace ChocolateyGui.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;

    /// <summary>
    /// Represents a fixed length ring buffer to store a specified maximal count of items within.
    /// </summary>
    /// <typeparam name="T">The generic type of the items stored within the ring buffer.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ObservableRingBufferCollection<T> : INotifyCollectionChanged, ICollection<T>
    {
        /// <summary>
        /// the internal buffer
        /// </summary>
        private T[] _buffer;

        /// <summary>
        /// The all-over position within the ring buffer. The position 
        /// increases continuously by adding new items to the buffer. This 
        /// value is needed to calculate the current relative position within the 
        /// buffer.
        /// </summary>
        private int _position;

        /// <summary>
        /// The current version of the buffer, this is required for a correct 
        /// exception handling while enumerating over the items of the buffer.
        /// </summary>
        private long _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRingBufferCollection{T}"/> class. 
        /// The observable ring buffer collection.
        /// </summary>
        /// <param name="capacity">
        /// The maximum count of items to be stored within 
        /// the ring buffer.
        /// </param>
        public ObservableRingBufferCollection(int capacity)
        {
            // validate capacity
            if (capacity <= 0)
            {
                throw new ArgumentException("Must be greater than zero", "capacity");
            }

            // set capacity and init the cache
            this.Capacity = capacity;
            this._buffer = new T[capacity];
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Gets the maximal count of items within the ring buffer.
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// Gets the current count of items within the ring buffer.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the buffer is read-only. This method always returns false.
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Adds a new item to the buffer.
        /// </summary>
        /// <param name="item">The item to be added to the buffer.</param>
        public void Add(T item)
        {
            // add a new item to the current relative position within the
            // buffer and increase the position
            var index = this._position++ % this.Capacity;
            if (this._buffer[index] != null)
            {
                this.NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, this._buffer[index]));
            }

            this._buffer[index] = item;
            
            // increase the count if capacity is not yet reached
            if (this.Count < this.Capacity)
            {
                this.Count++;
            }

            // buffer changed; next version
            this._version++;
            this.NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        /// <summary>
        /// Clears the whole buffer and releases all referenced objects 
        /// currently stored within the buffer.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this._buffer[i] = default(T);
            }

            this._position = 0;
            this.Count = 0;
            this._version++;
            this.NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Determines if a specified item is currently present within
        /// the buffer.
        /// </summary>
        /// <param name="item">The item to search for within the current
        /// buffer.</param>
        /// <returns>True if the specified item is currently present within 
        /// the buffer; otherwise false.</returns>
        public bool Contains(T item)
        {
            int index = this.IndexOf(item);
            return index != -1;
        }

        /// <summary>
        /// Copies the current items within the buffer to a specified array.
        /// </summary>
        /// <param name="array">The target array to copy the items of 
        /// the buffer to.</param>
        /// <param name="arrayIndex">The start position within the target
        /// array to start copying.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            for (int i = 0; i < this.Count; i++)
            {
                array[i + arrayIndex] = this._buffer[(this._position - this.Count + i) % this.Capacity];
            }
        }

        /// <summary>
        /// Gets an enumerator over the current items within the buffer.
        /// </summary>
        /// <returns>An enumerator over the current items within the buffer.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            long version = this._version;
            for (int i = 0; i < this.Count; i++)
            {
                if (version != this._version)
                {
                    throw new InvalidOperationException("Collection changed");
                }

                yield return this._buffer[(this._position - this.Count + i) % this.Capacity];
            }
        }

        /// <summary>
        /// See generic implementation of <see cref="GetEnumerator"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Removes a specified item from the current buffer.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        /// <returns>True if the specified item was successfully removed
        /// from the buffer; otherwise false.</returns>
        /// <remarks>
        /// <b>Warning</b>
        /// Frequent usage of this method might become a bad idea if you are 
        /// working with a large buffer capacity. The removing of an item 
        /// requires a scan of the buffer to get the position of the specified
        /// item. If the item was found, the deletion requires a move of all 
        /// items stored above the found position.
        /// </remarks>
        public bool Remove(T item)
        {
            // find the position of the specified item
            int index = this.IndexOf(item);

            // item was not found; return false
            if (index == -1)
            {
                return false;
            }

            // remove the item at the specified position
            this.RemoveAt(index);
            this.NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return true;
        }

        /// <summary>
        /// Gets the position of a specified item within the ring buffer.
        /// </summary>
        /// <param name="item">The item to get the current position for.</param>
        /// <returns>The zero based index of the found item within the 
        /// buffer. If the item was not present within the buffer, this
        /// method returns -1.</returns>
        private int IndexOf(T item)
        {
            // loop over the current count of items
            for (int i = 0; i < this.Count; i++)
            {
                // get the item at the relative position within the internal array
                T item2 = this._buffer[(this._position - this.Count + i) % this.Capacity];

                // if both items are null, return true
                if (null == item && null == item2)
                {
                    return i;
                }

                // if equal return the position
                if (item != null && item.Equals(item2))
                {
                    return i;
                }
            }

            // nothing found
            return -1;
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, args);
            }
        }

        /// <summary>
        /// Removes an item at a specified position within the buffer.
        /// </summary>
        /// <param name="index">The position of the item to be removed.</param>
        /// <exception cref="IndexOutOfRangeException">Thrown when incorrect argument passed in.</exception>
        /// <remarks>
        /// <b>Warning</b>
        /// Frequent usage of this method might become a bad idea if you are 
        /// working with a large buffer capacity. The deletion requires a move 
        /// of all items stored above the found position.
        /// </remarks>
        private void RemoveAt(int index)
        {
            // validate the index
            if (index < 0 || index >= this.Count)
            {
                throw new IndexOutOfRangeException();
            }

            // move all items above the specified position one step
            // closer to zero
            for (int i = index; i < this.Count - 1; i++)
            {
                // get the next relative target position of the item
                int to = (this._position - this.Count + i) % this.Capacity;
                
                // get the next relative source position of the item
                int from = (this._position - this.Count + i + 1) % this.Capacity;
                
                // move the item
                this._buffer[to] = this._buffer[from];
            }

            // get the relative position of the last item, which becomes empty
            // after deletion and set the item as empty
            int last = (this._position - 1) % this.Capacity;
            this._buffer[last] = default(T);

            // adjust storage information
            this._position--;
            this.Count--;
            
            // buffer changed; next version
            this._version++;

            this.NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
        }
    }
}