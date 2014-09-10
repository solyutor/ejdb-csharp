using System.Collections;
using System.Collections.Generic;
using Nejdb.Bson;
using Nejdb.Infrastructure;
using Nejdb.Infrastructure.Streams;
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
        private JsonSerializer _serializer;

        internal unsafe Cursor(CursorHandle cursorHandle, QueryFunctions.CursorResultDelegate cursorResult, int count, JsonSerializer serializer)
            : base(cursorHandle, cursorResult, count)
        {
            Serializer = serializer;
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

                    TDocument result = Serializer.Deserialize<TDocument>(reader);

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
        /// Returns document at current <see cref="CursorBase.Position"/>
        /// </summary>
        public TDocument Current
        {
            get { return this[Position]; }
        }

        /// <summary>
        /// Used to deserialize documents in an instance of <see cref="Cursor"/>
        /// </summary>
        public JsonSerializer Serializer
        {
            get { return _serializer; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowNull("value");
                }
                _serializer = value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
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
        /// Returns document at current <see cref="CursorBase.Position"/>
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