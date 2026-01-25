using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace BrUtility.Src
{
    public ref struct FastStackList<T>
    {
        /// <summary>
        /// direct access to the backing buffer. Do not use buffer.Length! Use FastList.length
        /// </summary>
        public Span<T> Buffer;

        /// <summary>
        /// direct access to the length of the filled items in the buffer. Do not change.
        /// </summary>
        public int Length;

        public int Capacity;

        public static FastStackList<T> EMPTY => new FastStackList<T>(Span<T>.Empty, 0);

        public FastStackList(Span<T> buffer, int size)
        {
            this.Buffer = buffer;
            Capacity = size;
            Length = 0;
        }


        public FastStackList(Span<T> buffer) : this(buffer, buffer.Length)
        {
        }


        /// <summary>
        /// provided for ease of access though it is recommended to just access the buffer directly.
        /// </summary>
        /// <param name="index">Index.</param>
        public T this[int index] => Buffer[index];

        public Span<T> Slice()
        {
            return Buffer.Slice(0, Length);
        }

        /// <summary>
        /// clears the list and nulls out all items in the buffer
        /// </summary>
        public void Clear()
        {
            Buffer.Fill(default);
            Length = 0;
        }


        /// <summary>
        /// works just like clear except it does not null our all the items in the buffer. Useful when dealing with structs.
        /// </summary>
        public void Reset()
        {
            Length = 0;
        }


        /// <summary>
        /// adds the item to the list.
        /// If the list is already full (Length == Capacity), the item will not be added.
        /// Returns whether or not adding was successful.
        /// </summary>
        public bool Add(T item)
        {
            if (Length == Buffer.Length)
                return false;
            Buffer[Length++] = item;
            return true;
        }


        /// <summary>
        /// removes the item from the list
        /// </summary>
        /// <param name="item">Item.</param>
        public void Remove(T item)
        {
            var comp = EqualityComparer<T>.Default;
            for (var i = 0; i < Length; ++i)
            {
                if (comp.Equals(Buffer[i], item))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }


        /// <summary>
        /// removes the item at the given index from the list
        /// </summary>
        public void RemoveAt(int index)
        {
            new Assert(index < Length, "Index out of range!", BrUtility.Logger.LogLevel.Error);

            Length--;
            if (index < Length)
                Buffer.Slice(index + 1, Length - index).CopyTo(Buffer.Slice(index, Length - index));
                //Array.Copy(Buffer, index + 1, Buffer, index, Length - index);
            Buffer[Length] = default(T);
        }


        /// <summary>
        /// removes the item at the given index from the list but does NOT maintain list order
        /// </summary>
        /// <param name="index">Index.</param>
        public void RemoveAtWithSwap(int index)
        {
            new Assert(index < Length, "Index out of range!", BrUtility.Logger.LogLevel.Error);

            Buffer[index] = Buffer[Length - 1];
            Buffer[Length - 1] = default(T);
            --Length;
        }


        /// <summary>
        /// checks to see if item is in the FastList
        /// </summary>
        /// <param name="item">Item.</param>
        public bool Contains(T item)
        {
            var comp = EqualityComparer<T>.Default;
            for (var i = 0; i < Length; ++i)
            {
                if (comp.Equals(Buffer[i], item))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// adds all items from array
        /// </summary>
        /// <param name="array">Array.</param>
        public void AddRange(IEnumerable<T> array)
        {
            foreach (var item in array)
                Add(item);
        }

        public void AddRange(Span<T> slice)
        {
            foreach (var item in slice)
                Add(item);
        }
    }
}
