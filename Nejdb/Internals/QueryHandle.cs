using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Nejdb.Bson;

namespace Nejdb.Internals
{
    internal class QueryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        //EJDB_EXPORT EJQ* ejdbcreatequery2(EJDB *jb, void *qbsdata);
        //[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbcreatequery2", CallingConvention = CallingConvention.Cdecl)]
        //static extern IntPtr _ejdbcreatequery([In] IntPtr jb, [In] byte[] bsdata);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbcreatequery2")]
        private delegate IntPtr CreateQueryDelegate([In] DatabaseHandle database, [In] byte[] queryAsBson);

        //EJDB_EXPORT void ejdbquerydel(EJQ *q);
        //[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbquerydel", CallingConvention = CallingConvention.Cdecl)]
        //static extern void _ejdbquerydel([In] IntPtr qptr);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbquerydel")]
        private delegate void DeleteQueryDelegate(IntPtr query);


        ////EJDB_EXPORT EJQ* ejdbqueryhints(EJDB *jb, EJQ *q, void *hintsbsdata)
        //[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbqueryhints", CallingConvention = CallingConvention.Cdecl)]
        //static extern IntPtr _ejdbqueryhints([In] IntPtr jb, [In] IntPtr qptr, [In] byte[] bsdata);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqueryhints")]
        private delegate IntPtr SetHintsDelegate([In] DatabaseHandle database, [In] QueryHandle query, [In] byte[] hintsAsBson);

        ////EJDB_EXPORT EJQ* ejdbqueryaddor(EJDB *jb, EJQ *q, void *orbsdata)
        //[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbqueryaddor", CallingConvention = CallingConvention.Cdecl)]
        //static extern IntPtr _ejdbqueryaddor([In] IntPtr jb, [In] IntPtr qptr, [In] byte[] bsdata);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqueryaddor")]
        private delegate IntPtr AddOrDelegate([In] DatabaseHandle database, [In] QueryHandle query, [In] byte[] addOrAsBson);

        ////EJDB_EXPORT EJQRESULT ejdbqryexecute(EJCOLL *jcoll, const EJQ *q, uint32_t *count, int qflags, TCXSTR *log)
        //[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbqryexecute", CallingConvention = CallingConvention.Cdecl)]
        //static extern IntPtr _ejdbqryexecute([In] IntPtr jcoll, [In] IntPtr q, out int count, [In] int qflags, [In] IntPtr logxstr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqryexecute")]
        private delegate IntPtr ExecuteQueryDelegate([In] CollectionHandle collection, [In] QueryHandle query, out int count, [In] QueryMode flags, [In] IntPtr logxstr);

        ////EJDB_EXPORT void ejdbqresultdispose(EJQRESULT qr);
        //[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbqresultdispose", CallingConvention = CallingConvention.Cdecl)]
        //static extern void _ejdbqresultdispose([In] IntPtr qres);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqresultdispose")]
        private delegate void DeleteCursorDelegate([In] IntPtr cursor);

        private ExecuteQueryDelegate _execute;
        private SetHintsDelegate _setHints;
        private DeleteCursorDelegate _deleteCursor;
        private AddOrDelegate _addOr;

        private readonly DeleteQueryDelegate _delete;
        public readonly Collection _collection;

        public QueryHandle(Collection collection) : base(true)
        {
            _collection = collection;
            var database = collection.Database;
            var libraryHandle = database.Library.LibraryHandle;

            var createQuery = libraryHandle.GetUnmanagedDelegate<CreateQueryDelegate>();

            handle = createQuery(database.DatabaseHandle, BsonDocument.Empty.ToByteArray());

            if (IsInvalid)
            {
                EjdbException.FromDatabase(database, "Failed to create query");
            }

            _delete = libraryHandle.GetUnmanagedDelegate<DeleteQueryDelegate>();

            _execute = libraryHandle.GetUnmanagedDelegate<ExecuteQueryDelegate>();
            _deleteCursor = libraryHandle.GetUnmanagedDelegate<DeleteCursorDelegate>();

            _setHints = libraryHandle.GetUnmanagedDelegate<SetHintsDelegate>();
            _addOr = libraryHandle.GetUnmanagedDelegate<AddOrDelegate>();
        }

        internal void AddOrAdd(byte[] bson)
        {
            IntPtr qptr = _addOr(_collection.Database.DatabaseHandle, this, bson);
            if (qptr == IntPtr.Zero)
            {
                EjdbException.FromDatabase(_collection.Database, "Failed to append Or restriction");
                //throw new EJDBQueryException(_jb);
            }
        }

        internal void SetHints(QueryHints hints)
        {
            if (hints.IsEmpty)
            {
                return;
            }

            IntPtr qptr = _setHints(_collection.Database.DatabaseHandle, this, hints.ToByteArray());

            if (qptr == IntPtr.Zero)
            {
                //TODO: throw more specific exception
                //throw new EJDBQueryException(_jb);
                EjdbException.FromDatabase(_collection.Database, "Failed to set hints");
            }
        }

        internal CursorHandle Execute(QueryMode flags, out int count)
        {
            QueryLogBuffer logsptr = null;
            //TODO: check it more elegant way?
            if ((flags & QueryMode.Explain) != 0)
            {
                //static extern IntPtr _tcxstrnew();
                logsptr = new QueryLogBuffer(_collection.Database.Library.LibraryHandle); //Create dynamic query execution log buffer
            }

            CursorHandle cursorHandle = null;
            try
            {
                int resultCount = 0;
                //static extern IntPtr _ejdbqryexecute([In] IntPtr jcoll, [In] IntPtr q, out int count, [In] int qflags, [In] IntPtr logxstr);
                IntPtr bufferPointer = logsptr == null ? IntPtr.Zero : logsptr.DangerousGetHandle();
                cursorHandle = new CursorHandle(() => _execute(_collection.CollectionHandle, this, out resultCount, flags, bufferPointer), pointer => _deleteCursor(pointer));
                count = resultCount;
            }
            finally
            {
                if (logsptr != null)
                {
                    try
                    {
                        if (cursorHandle != null && !cursorHandle.IsInvalid)
                        {
                            //static extern IntPtr _tcxstrptr([In] IntPtr strptr);
                            IntPtr sbptr = logsptr.AsString();

                            cursorHandle.Log = Native.StringFromNativeUtf8(sbptr); //UnixMarshal.PtrToString(sbptr, Encoding.UTF8);
                        }
                    }
                    finally
                    {
                        //static extern IntPtr _tcxstrdel([In] IntPtr strptr);
                        logsptr.Dispose();
                    }
                }
            }
            _collection.Database.ThrowOnError();

            return cursorHandle;
        }

        

        protected override bool ReleaseHandle()
        {
            _delete(handle);

            _execute = null;
            _setHints = null;
            _deleteCursor = null;
            _addOr = null;

            return true;
        }
    }
}