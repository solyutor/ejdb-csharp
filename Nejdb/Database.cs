using System;
using System.Runtime.InteropServices;
using Nejdb.Bson;
using Nejdb.Internals;

namespace Nejdb
{
    /// <summary>
    /// Provides operation over ejdb database.
    /// </summary>
    public class Database : IDisposable
    {
        /// <summary>
        /// The default open mode (OpenMode.Writer | OpenMode.CreateIfNotExists) <c>(JBOWRITER | JBOCREAT)</c>
        /// </summary>
        public const OpenMode DefaultOpenMode = (OpenMode.Writer | OpenMode.CreateIfNotExists);

        internal readonly Library Library;

        internal DatabaseHandle DatabaseHandle;

        private OpenDatabaseDelegate _openDatabase;
        private CloseDatabaseDelegate _closeDatabase;
        private IsOpenDelegate _isOpen;
        private GetErrorCodeDelegate _getErrorCode;
        private GetMetaDelegate _getMetadata;

        private SyncDelegate _sync;
        private CommandDelegate _command;

        //[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbopen", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern bool _ejdbopen([In] IntPtr db, [In] IntPtr path, int mode);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbopen")]
        private delegate bool OpenDatabaseDelegate([In] DatabaseHandle database, [In] IntPtr path, [MarshalAs(UnmanagedType.I4)]OpenMode openMode);

        //[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbclose", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern bool _ejdbclose([In] IntPtr db);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbclose")]
        private delegate bool CloseDatabaseDelegate([In] DatabaseHandle database);

        //[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbisopen", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern bool _ejdbisopen([In] IntPtr db);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbisopen")]
        private delegate bool IsOpenDelegate([In] DatabaseHandle database);

        //[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbecode", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern int _ejdbecode([In] IntPtr db);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbecode")]
        private delegate int GetErrorCodeDelegate([In] DatabaseHandle database);

        ////EJDB_EXPORT Bson* ejdbmeta(EJDB *jb)
        //[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbmeta", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern IntPtr _ejdbmeta([In] IntPtr db);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbmeta")]
        private delegate IntPtr GetMetaDelegate([In] DatabaseHandle database);

        ////EJDB_EXPORT Bson* ejdbcommand2(EJDB *jb, void *cmdBsondata);
        //[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbcommand2", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern IntPtr _ejdbcommand([In] IntPtr db, [In] byte[] cmd);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbcommand2")]
        private delegate IntPtr CommandDelegate([In] DatabaseHandle database, [In] byte[] command);

        ////EJDB_EXPORT bool ejdbsyncdb(EJDB *jb)
        //[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbsyncdb", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern bool _ejdbsyncdb([In] IntPtr db);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbsyncdb")]
        private delegate bool SyncDelegate([In] DatabaseHandle database);

        /// <summary>
        /// Creates instance of EJDB. 
        /// </summary>
        /// <param name="library"></param>
        public Database(Library library)
        {
            var libraryHandle = library.LibraryHandle;
            DatabaseHandle = new DatabaseHandle(libraryHandle);

            Library = library;

            _openDatabase = libraryHandle.GetUnmanagedDelegate<OpenDatabaseDelegate>();
            _closeDatabase = libraryHandle.GetUnmanagedDelegate<CloseDatabaseDelegate>();
            _isOpen = libraryHandle.GetUnmanagedDelegate<IsOpenDelegate>();

            _getErrorCode = libraryHandle.GetUnmanagedDelegate<GetErrorCodeDelegate>();
            _getMetadata = libraryHandle.GetUnmanagedDelegate<GetMetaDelegate>();


            _command = libraryHandle.GetUnmanagedDelegate<CommandDelegate>();
            _sync = libraryHandle.GetUnmanagedDelegate<SyncDelegate>();
        }

        /// <summary>
        /// Gets the last DB error code or <c>null</c> if underlying native database object does not exist.
        /// </summary>
        /// <value>The last DB error code.</value>
        public int LastErrorCode
        {
            get { return _getErrorCode(DatabaseHandle); }
        }

        /// <summary>
        /// Gets a value indicating whether this EJDB databse is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get { return _isOpen(DatabaseHandle); }
        }

        /// <summary>
        /// Automatically creates new collection if it does't exists.
        /// </summary>
        /// <remarks>
        /// Collection options <c>options</c> are applied only for newly created collection.
        /// For existing collections <c>options</c> has no effect.
        /// </remarks>
        /// <returns><c>false</c> error ocurried.</returns>
        /// <param name="name">Name of collection.</param>
        /// <param name="options">Collection options.</param>
        public Collection CreateCollection(string name, CollectionOptions options)
        {
            return new Collection(this, name, options);
        }

        /// <summary>
        /// Gets existing collection and throws if in does not exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Collection GetCollection(string name)
        {
            return new Collection(this, name);
        }

        /// <summary>
        /// Synchronize database state with disk
        /// </summary>
        public void Synchronize()
        {
            if (_sync(DatabaseHandle))
            {
                return;
            }
            throw EjdbException.FromDatabase(this, "Failed to synchronize database");
        }


        /// <summary>
        /// Gets info of EJDB database itself and its collections.
        /// </summary>
        /// <value>The DB meta.</value>
        public BsonDocument DatabaseMetadata
        {
            get
            {
                using (var bson = new BsonHandle(() => _getMetadata(DatabaseHandle), Library.FreeBson))
                {
                    if (bson.IsInvalid)
                    {
                        EjdbException.FromDatabase(this, "Could not get database metadata");

                    }

                    return Library.ConvertToBsonDocument(bson);
                }
            }
        }

        ///// <summary>
        ///// Executes EJDB command.
        ///// </summary>
        ///// <remarks>
        ///// Supported commands:
        /////
        ///// 1) Exports database collections data. See ejdbexport() method.
        ///// 
        ///// 	"export" : {
        ///// 	"path" : string,                    //Exports database collections data
        ///// 	"cnames" : [string array]|null,     //List of collection names to export
        ///// 	"mode" : int|null                   //Values: null|`JBJSONEXPORT` See ejdb.h#ejdbexport() method
        ///// }
        ///// 
        ///// Command response:
        ///// {
        ///// 	"log" : string,        //Diagnostic log about executing this command
        ///// 	"error" : string|null, //ejdb error message
        ///// 	"errorCode" : int|0,   //ejdb error code
        ///// }
        ///// 
        ///// 2) Imports previously exported collections data into ejdb.
        ///// 
        ///// 	"import" : {
        ///// 	"path" : string                     //The directory path in which data resides
        ///// 		"cnames" : [string array]|null,     //List of collection names to import
        ///// 		"mode" : int|null                //Values: null|`JBIMPORTUPDATE`|`JBIMPORTREPLACE` See ejdb.h#ejdbimport() method
        ///// }
        ///// 
        ///// Command response:
        ///// {
        ///// 	"log" : string,        //Diagnostic log about executing this command
        ///// 	"error" : string|null, //ejdb error message
        ///// 	"errorCode" : int|0,   //ejdb error code
        ///// }
        ///// </remarks>
        ///// <param name="cmd">Command object</param>
        ///// <returns>Command response.</returns>
        ////public BsonDocument Command(BsonDocument cmd)
        ////{
        ////	CheckDisposed();
        ////	byte[] cmdata = cmd.ToByteArray();
        ////	//internal static extern IntPtr _ejdbcommand([In] IntPtr db, [In] byte[] cmd);
        ////	IntPtr cmdret = _command(this, cmdata);
        ////	if (cmdret == IntPtr.Zero)
        ////	{
        ////		return null;
        ////	}
        ////	byte[] bsdata = BsonPtrIntoByteArray(cmdret);
        ////	if (bsdata.Count == 0)
        ////	{
        ////		return null;
        ////	}
        ////	BsonIterator it = new BsonIterator(bsdata);
        ////	return it.ToBsonDocument();
        ////}

        /// <summary>
        /// Opens or creates database file depending on <see cref="OpenMode"/>
        /// </summary>
        /// <param name="dbFilePath">Database filename</param>
        /// <param name="openMode"></param>
        public void Open(string dbFilePath, OpenMode openMode = DefaultOpenMode)
        {
            IntPtr pathPointer = Native.NativeUtf8FromString(dbFilePath); //UnixMarshal.StringToHeap(path, Encoding.UTF8);

            try
            {
                bool result = _openDatabase(DatabaseHandle, pathPointer, openMode);
                if (!result)
                {
                    throw EjdbException.FromDatabase(this, "Error on open database");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pathPointer); //UnixMarshal.FreeHeap(pptr);
            }
        }

        /// <summary>
        /// Close database and free all resources
        /// </summary>
        public void Dispose()
        {
            Close();
            DatabaseHandle.Dispose();

            _openDatabase = null;
            _closeDatabase = null;
            _isOpen = null;
            _getErrorCode = null;
            _getMetadata = null;
            _sync = null;
            _command = null;
            DatabaseHandle = null;
        }
        /// <summary>
        /// Closes current database
        /// </summary>
        public void Close()
        {
            if (_isOpen(DatabaseHandle))
            {
                _closeDatabase(DatabaseHandle);
            }
        }

        protected internal void ThrowOnError()
        {
            var errorCode = LastErrorCode;

            if (errorCode != 0)
            {
                EjdbException.FromDatabase(this, string.Empty);
            }
        }
    }
}