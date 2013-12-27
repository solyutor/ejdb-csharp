using System;
using System.Runtime.InteropServices;
using Nejdb.Infrastructure;

namespace Nejdb.Internals
{
    internal class DatabaseFunctions
    {
        internal readonly DeleteInstanceDelegate DeleteInstance;
        internal readonly NewInstanceDelegate NewInstance;

        internal readonly OpenDatabaseDelegate OpenDatabase;
        internal readonly CloseDatabaseDelegate CloseDatabase;
        internal readonly IsOpenDelegate IsOpen;
        internal readonly GetErrorCodeDelegate GetErrorCode;
        internal readonly GetMetaDelegate GetMetadata;
        
        internal readonly SyncDelegate Sync;
        internal readonly CommandDelegate Command;


        //Creates new instance of ejdb. Don't know what' is it, but it looks it should be done before opening database
        public unsafe DatabaseFunctions(LibraryHandle handle)
        {
            NewInstance = handle.GetUnmanagedDelegate<NewInstanceDelegate>();
            DeleteInstance = handle.GetUnmanagedDelegate<DeleteInstanceDelegate>();

            OpenDatabase = handle.GetUnmanagedDelegate<OpenDatabaseDelegate>();
            CloseDatabase = handle.GetUnmanagedDelegate<CloseDatabaseDelegate>();
            IsOpen = handle.GetUnmanagedDelegate<IsOpenDelegate>();

            GetErrorCode = handle.GetUnmanagedDelegate<GetErrorCodeDelegate>();
            GetMetadata = handle.GetUnmanagedDelegate<GetMetaDelegate>();


            Command = handle.GetUnmanagedDelegate<CommandDelegate>();
            Sync = handle.GetUnmanagedDelegate<SyncDelegate>();
        }

        // EJDB_EXPORT void ejdbdel(EJDB *jb);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbdel")]
        internal delegate void DeleteInstanceDelegate([In] IntPtr database);

        // EJDB_EXPORT EJDB* ejdbnew(void);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbnew")]
        internal delegate IntPtr NewInstanceDelegate(LibraryHandle handle);

        //EJDB_EXPORT bool ejdbopen(EJDB *jb, const char *path, int mode);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbopen")]
        internal unsafe delegate bool OpenDatabaseDelegate([In] DatabaseHandle database, [In] UnsafeBuffer* path, [MarshalAs(UnmanagedType.I4)]OpenMode openMode);

        //EJDB_EXPORT bool ejdbclose(EJDB *jb);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbclose")]
        internal delegate bool CloseDatabaseDelegate([In] DatabaseHandle database);

        //EJDB_EXPORT bool ejdbisopen(EJDB *jb);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbisopen")]
        internal delegate bool IsOpenDelegate([In] DatabaseHandle database);

        //EJDB_EXPORT int ejdbecode(EJDB *jb);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbecode")]
        internal delegate int GetErrorCodeDelegate([In] DatabaseHandle database);

        //EJDB_EXPORT bson* ejdbmeta(EJDB *jb)
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbmeta")]
        internal delegate IntPtr GetMetaDelegate([In] DatabaseHandle database);

        //EJDB_EXPORT bson* ejdbcommand(EJDB *jb, bson *cmdbson);
        //EJDB_EXPORT bson* ejdbcommand2(EJDB *jb, void *cmdbsondata);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbcommand2")]
        internal delegate IntPtr CommandDelegate([In] DatabaseHandle database, [In] byte[] command);

        //EJDB_EXPORT bool ejdbsyncdb(EJDB *jb);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbsyncdb")]
        internal delegate bool SyncDelegate([In] DatabaseHandle database);
    }
}