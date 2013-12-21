using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Nejdb.Bson;
using Nejdb.Internals;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Converters;

namespace Nejdb
{
    /// <summary>
    /// Encapsulates operation over non typed cursor
    /// </summary>
    public class Cursor<TDocument> : CursorBase, IEnumerable<TDocument>
    {
        private readonly JsonSerializer serializer;

        internal Cursor(LibraryHandle libraryHandle, CursorHandle cursorHandle, int count)
            : base(libraryHandle, cursorHandle, count)
        {
            this.serializer = new JsonSerializer();
            this.serializer.ContractResolver = NoObjectIdContractResolver.Instance;
            this.serializer.Converters.Add(new ObjectIdConverter());

        }

        /// <summary>
        /// Returns a result with specified index from results
        /// </summary>
        public TDocument this[int index]
        {
            get
            {
                EnsureInRange(index);

                int size;
                var resultPointer = CursorResult(index, out size);

                byte[] bson = new byte[size];

                Marshal.Copy(resultPointer, bson, 0, bson.Length);

                using (var stream = new MemoryStream(bson))
                using (var reader = new BsonReader(stream))
                {
                    var id = new ObjectId(new ArraySegment<byte>(bson, 9, 12));

                    TDocument result = serializer.Deserialize<TDocument>(reader);

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
            NextPosition();
            return this[Position];
        }

        /// <summary>
        /// Returns document at current <see cref="CursorBase#Position"/>
        /// </summary>
        public TDocument Current
        {
            get { return this[Position]; }
        }

        public IEnumerator<TDocument> GetEnumerator()
        {
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
                EnsureInRange(index);

                int size;
                var resultPointer = CursorResult(index, out size);

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
            NextPosition();
            return this[Position];
        }


        /// <summary>
        /// Returns document at current <see cref="CursorBase#Position"/>
        /// </summary>
        public BsonIterator Current
        {
            get { return this[Position]; }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the result set.
        /// </summary>
        public IEnumerator<BsonIterator> GetEnumerator()
        {
            while (IsInRange(Position))
            {
                yield return Current;
                NextPosition();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}