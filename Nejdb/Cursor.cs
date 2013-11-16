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
    /// <summary>
    /// Encapsulates operation over non typed cursor
    /// </summary>
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
                //TODO: should throw exception if called with count only mode
                if (IsInvalid || !IsInRange(index))
                {
                    return default(TDocument);
                }

                int size;
                var resultPointer = CursorResult(index, out size);

                if (resultPointer == IntPtr.Zero)
                {
                    return default(TDocument);
                }

                byte[] bson = new byte[size];

                Marshal.Copy(resultPointer, bson, 0, bson.Length);

                using (var stream = new MemoryStream(bson))
                using (var reader = new BsonReader(stream))
                {
                    var id = new ObjectId(new ArraySegment<byte>(bson, 9, 12));

                    var serialzer = new JsonSerializer();
                    serialzer.ContractResolver = NoObjectIdContractResolver.Instance;
                    TDocument result = serialzer.Deserialize<TDocument>(reader);

                    IdHelper<TDocument>.SetId(result, id);
                    return result;
                }

            }
        }

        /// <summary>
        /// Returns next document from cursor
        /// </summary>
        /// <returns></returns>
        public TDocument Next()
        {
            //TODO: should throw exception if called with count only mode
            if (IsValid || !IsInRange(NextPosition()))
            {
                return this[Position];
            }
            return default(TDocument);
        }

        public TDocument Current
        {
            get
            {
                //TODO: should throw exception if called with count only mode
                if (IsValid || IsInRange(Position))
                {
                    return this[Position];
                }
                return default(TDocument);
            }
           
        }


        public IEnumerator<TDocument> GetEnumerator()
        {
            //TODO: should throw exception if called with count only mode
            while (IsInRange(Position))
            {
                yield return Current;
                NextPosition();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Encapsulates operation over non typed cursor
    /// </summary>
    public class Cursor : CursorBase, IEnumerable<BsonIterator>
    {
        internal Cursor(LibraryHandle libraryHandle, CursorHandle cursorHandle, int count)
            : base(libraryHandle, cursorHandle, count)
        {
        }

        /// <summary>
        /// Returns a result with specified index from results
        /// </summary>
        public BsonIterator this[int index]
        {
            get
            {
                if (IsInvalid || !IsInRange(index))
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
            if (IsValid || IsInRange(NextPosition()))
            {
                return this[Position];
            }
            return null;
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