using Nejdb.Bson;

namespace Nejdb
{
    /// <summary>
    /// Encapsulates strongly typed query
    /// </summary>
    /// <typeparam name="TDocument">Type of document in query</typeparam>
    public class Query<TDocument> : QueryBase
    {
        internal Query(Collection collection) : base(collection)
        {
        }

        /// <summary>
        /// Executes query and returns result as cursor
        /// </summary>
        /// <param name="queryMode">Query mode</param>
        public Cursor<TDocument> Execute(QueryMode queryMode = QueryMode.Normal)
        {
            Handle.SetHints(Hints);
            int count;
            var cursorHandle = Handle.Execute(queryMode, out count);
            var cursor = new Cursor<TDocument>(Handle._collection.Database.Library.LibraryHandle, cursorHandle, count);
            return cursor;
        }

        /// <summary>
        /// Returns the only record thats meets search criteria
        /// </summary>
        /// <returns></returns>
        public TDocument FinOne()
        {
            using (Cursor<TDocument> cursor = Execute(QueryMode.FindOne))
            {
                return cursor.Next();
            }
        }

        /// <summary>
        /// Returns count of records that meets search criteria
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            using (Cursor<TDocument> cursor = Execute(QueryMode.Count))
            {
                return cursor.Count;
            }
        }
    }

    /// <summary>
    /// Encapsulates non typed query to EJDB database.
    /// </summary>
    public class Query : QueryBase
    {
        internal Query(Collection collection) :base(collection)
        {
            
        }

        /// <summary>
        /// Executes query and returns result as cursor
        /// </summary>
        /// <param name="queryMode">Query mode</param>
        public Cursor Execute(QueryMode queryMode = QueryMode.Normal)
        {
            Handle.SetHints(Hints);
            int count;
            var cursorHandle = Handle.Execute(queryMode, out count);
            var cursor = new Cursor(Handle._collection.Database.Library.LibraryHandle, cursorHandle, count);
            return cursor;
        }

        /// <summary>
        /// Returns the only record thats meets search criteria
        /// </summary>
        /// <returns></returns>
        public BsonIterator FinOne()
        {
            using (Cursor cur = Execute(QueryMode.FindOne))
            {
                return cur.Next();
            }
        }

        /// <summary>
        /// Returns count of records that meets search criteria
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            using (Cursor cur = Execute(QueryMode.Count))
            {
                return cur.Count;
            }
        }

        //TODO: Update query needs more actions to perform
        ///// <summary>
        ///// Updates
        ///// </summary>
        ///// <returns></returns>
        //public int Update()
        //{
        //    using (Cursor cur = Execute(QueryMode.Count))
        //    {
        //        return cur.Count;
        //    }
        //}
    }
}