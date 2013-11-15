using System;
using Nejdb.Bson;

namespace Nejdb
{
    /// <summary>
    /// Hint for <see cref="Query"/>
    /// </summary>
    public class QueryHints
    {
        private BsonDocument _hints;

        public QueryHints()
        {
            _hints = new BsonDocument();
        }
        /// <summary>
        /// Sets maximum number of documents to return 
        /// </summary>
        public QueryHints Max(int max)
        {
            if (max < 0)
            {
                throw new ArgumentException("Max limit cannot be negative");
            }

            _hints["$max"] = max;
            return this;
        }

        /// <summary>
        /// Sets number of documents to skip
        /// </summary>
        public QueryHints Skip(int skip)
        {
            if (skip < 0)
            {
                throw new ArgumentException("Skip value cannot be negative");
            }

            _hints["$skip"] = skip;
            return this;
        }

        /// <summary>
        /// Set sorted field
        /// </summary>
        /// <param name="field"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public QueryHints OrderBy(string field, bool asc = true)
        {
            BsonDocument oby = _hints["$orderby"] as BsonDocument;
            if (oby == null)
            {
                oby = new BsonDocument();
                _hints["$orderby"] = oby;
            }
            oby[field] = (asc) ? 1 : -1;

            return this;
        }

        /// <summary>
        /// Specifies fields to returned from documents
        /// </summary>
        public QueryHints IncludeFields(params string[] fields)
        {
            return IncExFields(fields, 1);
        }

        /// <summary>
        /// Specifies fields the should be returned from documents
        /// </summary>
        public QueryHints ExcludeFields(params string[] fields)
        {
            return IncExFields(fields, 0);
        }

        public bool IsEmpty
        {
            get { return _hints.KeysCount == 0; }
        }

        private QueryHints IncExFields(string[] fields, int inc)
        {
            BsonDocument fdoc = _hints["$fields"] as BsonDocument;
            if (fdoc == null)
            {
                fdoc = new BsonDocument();
                _hints["$fields"] = fdoc;
            }
            foreach (var fn in fields)
            {
                fdoc[fn] = inc;
            }

            return this;
        }

        public byte[] ToByteArray()
        {
            return _hints.ToByteArray();
        }
    }
}