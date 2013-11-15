using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Nejdb.Bson;
using Nejdb.Internals;

namespace Nejdb
{
    /// <summary>
    /// Encapsulates operation with non typed cursor
    /// </summary>
    public class Cursor : CursorBase, IEnumerable<BsonIterator>
    {
        internal Cursor(LibraryHandle libraryHandle, CursorHandle cursorHandle, int count) : base(libraryHandle, cursorHandle, count)
        {
        }

        /// <summary>
        /// Returns a result with specified index from results
        /// </summary>
        public BsonIterator this[int index]
        {
            get
            {
                if (IsInvalid || IsOutOfRange(index))
                {
                    return null;
                }

                int size;
                var resultPointer = CursorResult(index, out size);
                if (resultPointer == IntPtr.Zero)
                {
                    return null;
                }
                byte[] bsdata = new byte[size];
                Marshal.Copy(resultPointer, bsdata, 0, bsdata.Length);
                return new BsonIterator(bsdata);
            }
        }

        /// <summary>
        /// Returns next record in collection
        /// </summary>
        /// <returns></returns>
        public BsonIterator Next()
        {
            //check future position
            if (IsInvalid || IsOutOfRange(Position+1))
            {
                return null;
            }
            return this[NextPosition()];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the result set.
        /// </summary>
        public IEnumerator<BsonIterator> GetEnumerator()
        {
            BsonIterator it;
            while ((it = Next()) != null)
            {
                yield return it;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}