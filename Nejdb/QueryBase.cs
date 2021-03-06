﻿using System;
using System.Threading;
using Nejdb.Bson;
using Nejdb.Internals;

namespace Nejdb
{
    /// <summary>
    /// Encapsulates common fuctionality for all query objects
    /// </summary>
    public class QueryBase : IDisposable
    {
        private QueryHints _hints;
        private QueryHandle _handle;

        protected QueryBase(Collection collection, byte[] queryAsBson)
        {
            _handle = new QueryHandle(collection, queryAsBson);
            _hints = new QueryHints();
        }

        internal QueryHandle Handle
        {
            get { return _handle; }
        }

        /// <summary>
        /// Allows to tweak query using special hints
        /// </summary>
        public QueryHints Hints
        {
            get { return _hints; }
        }

        /// <summary>
        /// Append OR joined restriction to this query.
        /// </summary>
        /// <returns>This query object.</returns>
        /// <param name="docobj">Query document.</param>
        public void AppendOrRestriction(object docobj)
        {
            BsonDocument doc = BsonDocument.ValueOf(docobj);
            _handle.AddOr(doc.ToByteArray());
        }

        /// <summary>
        /// Releases query handle;
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            var handle = _handle;
            Thread.MemoryBarrier();

            if (handle == null)
            {
                return;
            }
            handle.Dispose();
            _handle = null;
            _hints = null;
        }
    }
}