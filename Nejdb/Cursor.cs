using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Nejdb.Bson;
using Nejdb.Internals;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Nejdb
{
    public class Cursor<TDocument> : CursorBase, IEnumerable<TDocument>
    {
        internal Cursor(LibraryHandle libraryHandle, CursorHandle cursorHandle, int count) : base(libraryHandle, cursorHandle, count)
        {
        }

        /// <summary>
        /// Returns a result with specified index from results
        /// </summary>
        public TDocument this[int index]
        {
            get
            {
                if (IsInvalid || IsOutOfRange(index))
                {
                    return default(TDocument);
                }

                int size;
                var resultPointer = CursorResult(index, out size);
                
                if (resultPointer == IntPtr.Zero)
                {
                    return default(TDocument);
                }

                byte[] bsdata = new byte[size];
                Marshal.Copy(resultPointer, bsdata, 0, bsdata.Length);
                TDocument result;
                using (var stream = new MemoryStream(bsdata))
                using (var reader = new BsonReader(stream))
                {
                    var serialzer = new JsonSerializer();
                    serialzer.ContractResolver = NoObjectIdContractResolver.Instance;
                    result = serialzer.Deserialize<TDocument>(reader);
                    //TODO: Check the id property
                    //IdHelper<TDocument>.SetId(result, );

                }
                return result;
            }
        }

        /// <summary>
        /// Returns next record in collection
        /// </summary>
        /// <returns></returns>
        public TDocument Next()
        {
            if (IsInvalid || IsOutOfRange(Position + 1))
            {
                return default(TDocument);
            }
            return this[NextPosition()];
        }

        public IEnumerator<TDocument> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

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