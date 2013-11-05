using System;
using System.Runtime.InteropServices;
using Ejdb.Utils;

namespace Ejdb.DB
{
	public class Collection : IDisposable
	{
		private readonly Database _database;
		private readonly string _name;
		private CollectionHandle _collectionHandle;
		private RemoveCollectionDelegate _remove;
		private BeginTransactionDelegate _beginTransaction;
		private CommitTransactionDelegate _commitTransaction;
		private RollbackTransactionDelegate _rollbackTransaction;
		private TransactionStatusDelegate _transactionStatus;
		private SyncDelegate _syncCollection;
		//EJDB_EXPORT bool ejdbrmcoll(EJDB *jb, const char *colname, bool unlinkfile);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbrmcoll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbrmcoll([In] IntPtr db, [In] IntPtr cname, bool unlink);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbrmcoll")]
		private delegate bool RemoveCollectionDelegate([In] DatabaseHandle database, [In] IntPtr collectionName, bool unlink);


		//EJDB_EXPORT bool ejdbsavebson3(EJCOLL *jcoll, void *bsdata, bson_oid_t *oid, bool merge);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbsavebson3", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbsavebson([In] IntPtr coll, [In] byte[] bsdata, [Out] byte[] oid, [In] bool merge);

		//EJDB_EXPORT bson* ejdbloadbson(EJCOLL *coll, const bson_oid_t *oid);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbloadbson", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbloadbson([In] IntPtr coll, [In] byte[] oid);

		
		
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

		private LibraryHandle LibraryHandle
		{
			get { return _database.DatabaseHandle.LibraryHandle; }
		}

		//opens existed
		internal Collection(Database database, string name)
		{
			_database = database;
			_name = name;
			_collectionHandle = new CollectionHandle(database, name);
			MapMethods();
		}

		//Creates new;
		internal Collection(Database database, string name, CollectionOptions options)
		{
			_database = database;
			_name = name;
			_collectionHandle = new CollectionHandle(database, name, options);
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
		}


		public void BeginTransaction()
		{
			if (_beginTransaction(_collectionHandle))
			{
				return;
			}

			throw EJDBException.FromDatabase(_database, "Failed to begin transaction");
		}

		public void CommitTransaction()
		{
			if (_commitTransaction(_collectionHandle))
			{
				return;
			}
			throw EJDBException.FromDatabase(_database, "Failed to commit transaction");
		}

		public void RollbackTransaction()
		{
			if (_rollbackTransaction(_collectionHandle))
			{
				return;
			}
			throw EJDBException.FromDatabase(_database, "Failed to rollback transaction");
		}

		public bool TransactionActive
		{
			get
			{
				bool isActive;
				if (_transactionStatus(_collectionHandle, out isActive))
				{
					return isActive;
				}

				throw EJDBException.FromDatabase(_database, "Failed to get transaction status");
			}
		}

		public void Synchronize()
		{
			if (_syncCollection(_collectionHandle))
			{
				return;
			}
			throw EJDBException.FromDatabase(_database, "Failed to sync collection");
		}


		/// <summary>
		/// Drops collection and wipes out all data
		/// </summary>
		public void Drop()
		{
			const bool deleteData = true;

			IntPtr unmanagedName = Native.NativeUtf8FromString(_name);//UnixMarshal.StringToHeap(name, Encoding.UTF8);
			try
			{
				_remove(_database.DatabaseHandle, unmanagedName, deleteData);
			}
			finally
			{
				Marshal.FreeHGlobal(unmanagedName); //UnixMarshal.FreeHeap(cptr);
			}
		}

		public void Dispose()
		{
			if (_collectionHandle != null)
			{
				_collectionHandle.Dispose();
				_collectionHandle = null;
			}
		}
	}
}