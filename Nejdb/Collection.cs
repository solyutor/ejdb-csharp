using System;
using System.IO;
using System.Runtime.InteropServices;
using Nejdb.Bson;
using Nejdb.Internals;
using Nejdb.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Nejdb
{
    public class Collection : IDisposable
    {
        public readonly Database Database;
        internal CollectionHandle CollectionHandle;

        private readonly string _name;
        
        private JsonSerializer _serializer;
        private CollectionFunctions _functions;

        //Creates new;
        private Collection(Database database, string name, Func<CollectionHandle> generator)
            
        {
            Database = database;
            _name = name;
            CollectionHandle = generator();

            _functions = database.Library.Functions.Collection;

            _serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = ObjectIdContractResolver.Instance,
            };
            _serializer.Converters.Add(ObjectIdConverter.Instance);
        }

        //opens existed
        internal Collection(Database database, string name) 
            : this(database, name, () => new CollectionHandle(database, name) )
        {

        }

        //Creates new;
        internal Collection(Database database, string name, CollectionOptions options) 
            : this(database, name, () => new CollectionHandle(database, name, options))
        {
        }

        /// <summary>
        /// Begins new transaction
        /// </summary>
        public Transaction BeginTransaction()
        {
            return new Transaction(this);
        }


        /// <summary>
        /// Begins new transaction
        /// </summary>
        internal void BeginTransactionInternal()
        {
            if (_functions.BeginTransaction(CollectionHandle))
            {
                return;
            }

            throw EjdbException.FromDatabase(Database, "Failed to begin transaction");
        }

        /// <summary>
        /// Commits current transaction
        /// </summary>
        internal void CommitTransactionInternal()
        {
            if (_functions.CommitTransaction(CollectionHandle))
            {
                return;
            }
            throw EjdbException.FromDatabase(Database, "Failed to commit transaction");
        }

        /// <summary>
        /// Rollbacks current transaction
        /// </summary>
        internal void RollbackTransactionInternal()
        {
            if (_functions.RollbackTransaction(CollectionHandle))
            {
                return;
            }
            throw EjdbException.FromDatabase(Database, "Failed to rollback transaction");
        }

        /// <summary>
        /// Returns true if transaction is active, false otherwise
        /// </summary>
        internal bool TransactionActive
        {
            get
            {
                bool isActive;
                if (_functions.TransactionStatus(CollectionHandle, out isActive))
                {
                    return isActive;
                }
                throw EjdbException.FromDatabase(Database, "Failed to get transaction status");
            }
        }
        /// <summary>
        /// Synchronize content of a EJDB collection database with the file on device
        /// </summary>
        public void Synchronize()
        {
            if (_functions.SyncCollection(CollectionHandle))
            {
                return;
            }
            throw EjdbException.FromDatabase(Database, "Failed to sync collection");
        }


        /// <summary>
        /// Drops collection and wipes out all data and indexes
        /// </summary>
        public void Drop()
        {
            const bool deleteData = true;

            IntPtr unmanagedName = Native.NativeUtf8FromString(_name);//UnixMarshal.StringToHeap(name, Encoding.UTF8);
            try
            {
                _functions.Remove(Database.DatabaseHandle, unmanagedName, deleteData);
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedName); //UnixMarshal.FreeHeap(cptr);
            }
        }

        /// <summary>
        /// Removes collection from database, but keeps data on disk
        /// </summary>
        public void Unlink()
        {
            const bool deleteData = false;

            IntPtr unmanagedName = Native.NativeUtf8FromString(_name);//UnixMarshal.StringToHeap(name, Encoding.UTF8);
            try
            {
                _functions.Remove(Database.DatabaseHandle, unmanagedName, deleteData);
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedName); //UnixMarshal.FreeHeap(cptr);
            }
        }

        /// <summary>
        /// Saves document to collection
        /// </summary>
        /// <param name="doc">Document to save</param>
        /// <param name="merge">If true the merge will be performed with old and new objects. Otherwise old object will be replaced</param>
        public ObjectId Save(BsonDocument doc, bool merge)
        {
            BsonValue id = doc.GetBsonValue("_id");

            ObjectId oiddata = new ObjectId();

            if (id != null)
            {
                oiddata = (ObjectId)id.Value;
            }

            using (var stream = new MemoryStream())
            {
                doc.Serialize(stream);
                var saveOk = _functions.SaveBson(CollectionHandle, stream.GetBuffer(), ref oiddata, merge);

                if (saveOk && id == null)
                {
                    doc.SetOID("_id", oiddata);
                }

                if (!saveOk)
                {
                    throw EjdbException.FromDatabase(Database, "Failed to save Bson");
                }
            }
            return oiddata;
        }

        /// <summary>
        /// Saves strongly typed document to collection. 
        /// <remarks>If document has property of type <see cref="ObjectId"/> and named 'Id' - it will be set after save</remarks>
        /// </summary>
        /// <typeparam name="TDocument">Document type</typeparam>
        /// <param name="document">Document to save</param>
        /// <param name="merge">If true the merge will be performed with old and new objects. Otherwise old object will be replaced</param>
        /// <returns>Id of saved document</returns>
        public ObjectId Save<TDocument>(TDocument document, bool merge)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BsonWriter(stream))
            {
                _serializer.Serialize(writer, document);

                ObjectId id = IdHelper<TDocument>.GetId(document);

                var saveOk = _functions.SaveBson(CollectionHandle, stream.GetBuffer(), ref id, merge);

                if (!saveOk)
                {
                    throw EjdbException.FromDatabase(Database, "Failed to save document");
                }

                IdHelper<TDocument>.SetId(document, id);

                return id;
            }
        }

        /// <summary>
        /// Loads JSON object identified by OID from the collection.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if object is not found.
        /// </remarks>
        /// <param name="id">Id of an object</param>
        public BsonDocument Load(ObjectId id)
        {
            using (var bson = new BsonHandle(() => _functions.LoadBson(CollectionHandle, ref id), Database.Library.FreeBson))
            {
                //document does not exists
                if (bson.IsInvalid)
                {
                    return null;
                }
                return Database.Library.ConvertToBsonDocument(bson);
            }
        }

        /// <summary>
        /// Loads JSON object identified by OID from the collection.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if object is not found.
        /// </remarks>
        /// <param name="id">Id of an object</param>
        public TDocument Load<TDocument>(ObjectId id)
        {
            using (var bson = new BsonHandle(() => _functions.LoadBson(CollectionHandle, ref id), Database.Library.FreeBson))
            {
                //document does not exists
                if (bson.IsInvalid)
                {
                    return default(TDocument);
                }
                var bsonBuffer = Database.Library.ConvertToBytes(bson);

                using (var stream = new MemoryStream(bsonBuffer))
                using (var reader = new BsonReader(stream))
                {
                    var document = _serializer.Deserialize<TDocument>(reader);
                    IdHelper<TDocument>.SetId(document, id);
                    return document;
                }
            }
        }



        /// <summary>
        /// Loads JSON object identified by OID from the collection.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if object is not found.
        /// </remarks>
        /// <param name="id">Id of an object</param>
        public void Delete(ObjectId id)
        {
            if (_functions.DeleteBson(CollectionHandle, ref id))
            {
                return;
            }
            throw EjdbException.FromDatabase(Database, "Failed to delete document");
        }


        /// <summary>
        /// Performs provided operations on collection indexes.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="flags"></param>
        public void Index(string path, IndexOperations flags)
        {
            IntPtr pathPointer = Native.NativeUtf8FromString(path); //UnixMarshal.StringToHeap(ipath, Encoding.UTF8);
            try
            {
                if (_functions.SetIndex(CollectionHandle, pathPointer, (int)flags))
                {
                    return;
                }
                throw EjdbException.FromDatabase(Database, "Failed to perform index operation");
            }
            finally
            {
                Marshal.FreeHGlobal(pathPointer); //UnixMarshal.FreeHeap(ipathptr);
            }
        }

        /// <summary>
        /// Creates new query over the collections
        /// </summary>
        public Query CreateQuery()
        {
            return new Query(this);
        }

        /// <summary>
        /// Creates new strongly typed query over the collections
        /// </summary>
        public Query<TDocument> CreateQuery<TDocument>()
        {
            return new Query<TDocument>(this);
        }

        /// <summary>
        /// Creates new strongly typed query over the collections
        /// </summary>
        public Query<TDocument> CreateQuery<TDocument>(QueryBuilder queryBuilder)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BsonWriter(stream))
            {
                queryBuilder.WriteTo(writer);
                return new Query<TDocument>(this, stream.GetBuffer());
            }
        }

        /// <summary>
        /// Closes collection and disposes all owned resources
        /// </summary>
        public void Dispose()
        {
            if (CollectionHandle != null)
            {
                CollectionHandle.Dispose();
                CollectionHandle = null;
                _functions = null;
                _serializer = null;
            }
        }
    }
}