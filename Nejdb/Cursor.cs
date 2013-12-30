using System.Collections;
using System.Collections.Generic;
using Nejdb.Bson;
using Nejdb.Infrastructure;
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
        private readonly JsonSerializer _serializer;

        internal unsafe Cursor(CursorHandle cursorHandle, QueryFunctions.CursorResultDelegate cursorResult, int count)
            : base(cursorHandle, cursorResult, count)
        {
            _serializer = new JsonSerializer
                          {
                              ContractResolver = ObjectIdContractResolver.Instance
                          };
            _serializer.Converters.Add(ObjectIdConverter.Instance);

        }

        /// <summary>
        /// Returns a result with specified index from results
        /// </summary>
        public unsafe TDocument this[int index]
        {
            get
            {
                EnsureInRange(index);

                int size;
                var resultPointer = CursorResult(index, out size);

                using (var stream = new UnsafeStream(resultPointer))
                using (var reader = new BsonReader(stream))
                {
                    //TODO: Try to move this hack to deserialization step
                    var id = *((ObjectId*) (resultPointer + 9));

                    TDocument result = _serializer.Deserialize<TDocument>(reader);

                    IdHelper<TDocument>.SetId(result, ref id);
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
        internal unsafe Cursor(CursorHandle cursorHandle, QueryFunctions.CursorResultDelegate cursorResult, int count)
            : base(cursorHandle, cursorResult, count)
        {
        }

        /// <summary>
        /// Returns a result with specified index from results
        /// </summary>
        public unsafe BsonIterator this[int index]
        {
            get
            {
                EnsureInRange(index);

                int size;
                var resultPointer = CursorResult(index, out size);
                return new BsonIterator(new UnsafeStream(resultPointer));
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