using System;
using System.Runtime.InteropServices;
using Nejdb.Bson;

namespace Nejdb.Internals
{
    internal class CollectionFunctions
    {
        public CollectionFunctions(LibraryHandle handle)
        {
            CreateCollection = handle.GetUnmanagedDelegate<CreateCollectionDelegate>();
            GetCollection = handle.GetUnmanagedDelegate<GetCollectionDelegate>();

            Remove = handle.GetUnmanagedDelegate<RemoveCollectionDelegate>();

            BeginTransaction = handle.GetUnmanagedDelegate<BeginTransactionDelegate>();
            CommitTransaction = handle.GetUnmanagedDelegate<CommitTransactionDelegate>();
            RollbackTransaction = handle.GetUnmanagedDelegate<RollbackTransactionDelegate>();
            TransactionStatus = handle.GetUnmanagedDelegate<TransactionStatusDelegate>();

            SyncCollection = handle.GetUnmanagedDelegate<SyncDelegate>();

            SaveBson = handle.GetUnmanagedDelegate<SaveBsonDelegate>();
            LoadBson = handle.GetUnmanagedDelegate<LoadBsonDelegate>();
            DeleteBson = handle.GetUnmanagedDelegate<DeleteBsonDelegate>();

            SetIndex = handle.GetUnmanagedDelegate<SetIndexDelegate>();
        }

        internal readonly CreateCollectionDelegate CreateCollection;
        internal readonly GetCollectionDelegate GetCollection;
        internal readonly RemoveCollectionDelegate Remove;

        internal readonly BeginTransactionDelegate BeginTransaction;
        internal readonly CommitTransactionDelegate CommitTransaction;
        internal readonly RollbackTransactionDelegate RollbackTransaction;
        internal readonly TransactionStatusDelegate TransactionStatus;

        internal readonly SyncDelegate SyncCollection;
        
        internal readonly SaveBsonDelegate SaveBson;
        internal readonly LoadBsonDelegate LoadBson;
        internal readonly DeleteBsonDelegate DeleteBson;
        
        internal readonly SetIndexDelegate SetIndex;
        

        //EJDB_EXPORT EJCOLL* ejdbgetcoll(EJDB *jb, const char *colname);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbgetcoll")]
        internal delegate IntPtr GetCollectionDelegate([In] DatabaseHandle database, [In] IntPtr collectionName);

        //EJDB_EXPORT EJCOLL* ejdbcreatecoll(EJDB *jb, const char *colname, EJCOLLOPTS *opts);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbcreatecoll")]
        internal delegate IntPtr CreateCollectionDelegate([In] DatabaseHandle database, [In] IntPtr collectionName, ref CollectionOptions options);


        //EJDB_EXPORT bool ejdbrmcoll(EJDB *jb, const char *colname, bool unlinkfile);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbrmcoll")]
        internal delegate bool RemoveCollectionDelegate([In] DatabaseHandle database, [In] IntPtr collectionName, bool unlink);


        //EJDB_EXPORT bool ejdbsaveBson3(EJCOLL *jcoll, void *bsdata, Bson_oid_t *id, bool merge);
        //TODO: Possible save methods: bool ejdbsaveBson(EJCOLL *coll, Bson *bs, Bson_oid_t *id) 
        //TODO: Possible save methods: bool ejdbsaveBson2(EJCOLL *coll, Bson *bs, Bson_oid_t *id, bool merge) - this one is preferable. Other two calls it. 		
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbsavebson3")]
        internal delegate bool SaveBsonDelegate([In] CollectionHandle collection, [In] byte[] bsdata, [In, Out] ref ObjectId oid, [In] bool merge);

        //EJDB_EXPORT Bson* ejdbloadbson(EJCOLL *coll, const Bson_oid_t *id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbloadbson")]
        internal delegate IntPtr LoadBsonDelegate([In] CollectionHandle collection, [In] ref ObjectId oid);

        //EJDB_EXPORT bool ejdbrmBson(EJCOLL *coll, Bson_oid_t *id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbrmbson")]
        internal delegate bool DeleteBsonDelegate([In] CollectionHandle collection, [In] ref ObjectId objectId);

        //EJDB_EXPORT bool ejdbtranbegin(EJCOLL *coll);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbtranbegin")]
        internal delegate bool BeginTransactionDelegate([In] CollectionHandle collection);

        //EJDB_EXPORT bool ejdbtrancommit(EJCOLL *coll);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbtrancommit")]
        internal delegate bool CommitTransactionDelegate([In] CollectionHandle collection);

        //EJDB_EXPORT bool ejdbtranabort(EJCOLL *coll);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbtranabort")]
        internal delegate bool RollbackTransactionDelegate([In] CollectionHandle collection);

        //EJDB_EXPORT bool ejdbtranstatus(EJCOLL *jcoll, bool *txactive);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbtranstatus")]
        internal delegate bool TransactionStatusDelegate([In] CollectionHandle collection, out bool isActive);

        //EJDB_EXPORT bool ejdbsyncoll(EJCOLL *coll)
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbsyncoll")]
        internal delegate bool SyncDelegate([In] CollectionHandle collection);

        ////EJDB_EXPORT bool ejdbsetindex(EJCOLL *coll, const char *ipath, int flags);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbsetindex")]
        internal delegate bool SetIndexDelegate([In] CollectionHandle collection, [In] IntPtr indexPath, [In] int operation);
    }
}