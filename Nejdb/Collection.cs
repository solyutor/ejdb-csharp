using System;
using System.Runtime.InteropServices;
using Nejdb.Bson;
using Nejdb.Internals;

namespace Nejdb
{
	public class Collection : IDisposable
	{
		public readonly Database Database;
		internal CollectionHandle CollectionHandle;

		private readonly string _name;
		private RemoveCollectionDelegate _remove;
		private BeginTransactionDelegate _beginTransaction;
		private CommitTransactionDelegate _commitTransaction;
		private RollbackTransactionDelegate _rollbackTransaction;
		private TransactionStatusDelegate _transactionStatus;
		private SyncDelegate _syncCollection;
		private SaveBsonDelegate _saveBson;
		private LoadBsonDelegate _loadBson;
		private DeleteBsonDelegate _deleteBson;
		private SetIndexDelegate _setIndex;

		//EJDB_EXPORT bool ejdbrmcoll(EJDB *jb, const char *colname, bool unlinkfile);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbrmcoll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbrmcoll([In] IntPtr db, [In] IntPtr cname, bool unlink);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbrmcoll")]
		private delegate bool RemoveCollectionDelegate([In] DatabaseHandle database, [In] IntPtr collectionName, bool unlink);


		//EJDB_EXPORT bool ejdbsaveBson3(EJCOLL *jcoll, void *bsdata, Bson_oid_t *oid, bool merge);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbsaveBson3", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbsaveBson([In] IntPtr coll, [In] byte[] bsdata, [Out] byte[] oid, [In] bool merge);
		//TODO: Possible save methods: bool ejdbsaveBson(EJCOLL *coll, Bson *bs, Bson_oid_t *oid) 
		//TODO: Possible save methods: bool ejdbsaveBson2(EJCOLL *coll, Bson *bs, Bson_oid_t *oid, bool merge) - this one is preferable. Other two calls it. 		
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbsavebson3")]
		private delegate bool SaveBsonDelegate([In] CollectionHandle collection, [In] byte[] bsdata, [Out] out ObjectId oid, [In] bool merge);


		//EJDB_EXPORT Bson* ejdbloadbson(EJCOLL *coll, const Bson_oid_t *oid);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbloadbson", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbloadbson([In] IntPtr coll, [In] byte[] oid);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbloadbson")]
		private delegate IntPtr LoadBsonDelegate([In] CollectionHandle collection, [Out] ObjectId oid);

		//EJDB_EXPORT bool ejdbrmBson(EJCOLL *coll, Bson_oid_t *oid);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbrmbson", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbrmBson([In] IntPtr coll, [In] byte[] oid);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbrmbson")]
		private delegate bool DeleteBsonDelegate([In] CollectionHandle collection, [In] ObjectId objectId);

		//EJDB_EXPORT bool ejdbtranbegin(EJCOLL *coll);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbtranbegin", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbtranbegin([In] IntPtr coll);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbtranbegin")]
		private delegate bool BeginTransactionDelegate([In] CollectionHandle collection);

		//EJDB_EXPORT bool ejdbtrancommit(EJCOLL *coll);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbtrancommit", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbtrancommit([In] IntPtr coll);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbtrancommit")]
		private delegate bool CommitTransactionDelegate([In] CollectionHandle collection);

		////EJDB_EXPORT bool ejdbtranabort(EJCOLL *coll);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbtranabort", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbtranabort([In] IntPtr coll);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbtranabort")]
		private delegate bool RollbackTransactionDelegate([In] CollectionHandle collection);

		////EJDB_EXPORT bool ejdbtranstatus(EJCOLL *jcoll, bool *txactive);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbtranstatus", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbtranstatus([In] IntPtr coll, out bool txactive);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbtranstatus")]
		private delegate bool TransactionStatusDelegate([In] CollectionHandle collection, out bool isActive);

		////EJDB_EXPORT bool ejdbsyncoll(EJCOLL *coll)
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbsyncoll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbsyncoll([In] IntPtr coll);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbsyncoll")]
		private delegate bool SyncDelegate([In] CollectionHandle collection);

		////EJDB_EXPORT bool ejdbsetindex(EJCOLL *coll, const char *ipath, int flags);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbsetindex", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbsetindex([In] IntPtr coll, [In] IntPtr ipathptr, int flags);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbsetindex")]
		private delegate bool SetIndexDelegate([In] CollectionHandle collection, [In] IntPtr indexPath, [In] int operation);

		private LibraryHandle LibraryHandle
		{
			get { return Database.DatabaseHandle.LibraryHandle; }
		}

		//opens existed
		internal Collection(Database database, string name)
		{
			Database = database;
			_name = name;
			CollectionHandle = new CollectionHandle(database, name);
			MapMethods();
		}

		//Creates new;
		internal Collection(Database database, string name, CollectionOptions options)
		{
			Database = database;
			_name = name;
			CollectionHandle = new CollectionHandle(database, name, options);
			MapMethods();
		}

		private void MapMethods()
		{
			_remove = LibraryHandle.GetUnmanagedDelegate<RemoveCollectionDelegate>();

			_beginTransaction = LibraryHandle.GetUnmanagedDelegate<BeginTransactionDelegate>();
			_commitTransaction = LibraryHandle.GetUnmanagedDelegate<CommitTransactionDelegate>();
			_rollbackTransaction = LibraryHandle.GetUnmanagedDelegate<RollbackTransactionDelegate>();
			_transactionStatus = LibraryHandle.GetUnmanagedDelegate<TransactionStatusDelegate>();

			_syncCollection = LibraryHandle.GetUnmanagedDelegate<SyncDelegate>();

			_saveBson = LibraryHandle.GetUnmanagedDelegate<SaveBsonDelegate>();
			_loadBson = LibraryHandle.GetUnmanagedDelegate<LoadBsonDelegate>();
			_deleteBson = LibraryHandle.GetUnmanagedDelegate<DeleteBsonDelegate>();

			_setIndex = LibraryHandle.GetUnmanagedDelegate<SetIndexDelegate>();
		}

		/// <summary>
		/// Begins new transaction
		/// </summary>
		public void BeginTransaction()
		{
			if (_beginTransaction(CollectionHandle))
			{
				return;
			}

			throw EjdbException.FromDatabase(Database, "Failed to begin transaction");
		}

		/// <summary>
		/// Commits current transaction
		/// </summary>
		public void CommitTransaction()
		{
			if (_commitTransaction(CollectionHandle))
			{
				return;
			}
			throw EjdbException.FromDatabase(Database, "Failed to commit transaction");
		}

		/// <summary>
		/// Rollbacks current transaction
		/// </summary>
		public void RollbackTransaction()
		{
			if (_rollbackTransaction(CollectionHandle))
			{
				return;
			}
			throw EjdbException.FromDatabase(Database, "Failed to rollback transaction");
		}

		/// <summary>
		/// Returns true if transaction is active, false otherwise
		/// </summary>
		public bool TransactionActive
		{
			get
			{
				bool isActive;
				if (_transactionStatus(CollectionHandle, out isActive))
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
			if (_syncCollection(CollectionHandle))
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
				_remove(Database.DatabaseHandle, unmanagedName, deleteData);
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
				_remove(Database.DatabaseHandle, unmanagedName, deleteData);
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
		public void Save(BsonDocument doc, bool merge)
		{
			BsonValue id = doc.GetBsonValue("_id");

			byte[] bsdata = doc.ToByteArray();

			ObjectId oiddata = new ObjectId();
			if (id != null)
			{
				oiddata = (ObjectId) id.Value;
			}

			var saveOk = _saveBson(CollectionHandle, bsdata, out oiddata, merge);

			if (saveOk && id == null)
			{
				doc.SetOID("_id", oiddata);
			}

			if (!saveOk)
			{
				throw EjdbException.FromDatabase(Database, "Failed to save Bson");
			}
		}

		/// <summary>
		/// Loads JSON object identified by OID from the collection.
		/// </summary>
		/// <remarks>
		/// Returns <c>null</c> if object is not found.
		/// </remarks>
		/// <param name="oid">Id of an object</param>
		public BsonDocument Load(ObjectId oid)
		{
			using (var bson = new BsonHandle(() => _loadBson(CollectionHandle, oid), Database.Library.FreeBson))
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
		/// <param name="oid">Id of an object</param>
		public void Delete(ObjectId oid)
		{
			if (_deleteBson(CollectionHandle, oid))
			{
				return;
			}
			throw EjdbException.FromDatabase(Database, "Failed to save Bson");
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
				if (_setIndex(CollectionHandle, pathPointer, (int)flags))
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
		/// Closes collection and disposes all owned resources
		/// </summary>
		public void Dispose()
		{
			if (CollectionHandle != null)
			{
				CollectionHandle.Dispose();
				CollectionHandle = null;
			}
		}
	}
}