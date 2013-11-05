using System;
using System.Runtime.InteropServices;
using Ejdb.Utils;

namespace Ejdb.DB
{
	public class Database 
	{
		/// <summary>
		/// The default open mode (OpenMode.Writer | OpenMode.CreateIfNotExists) <c>(JBOWRITER | JBOCREAT)</c>
		/// </summary>
		public const OpenMode DefaultOpenMode = (OpenMode.Writer | OpenMode.CreateIfNotExists);

		private readonly Library _library;
		
		private OpenDatabaseDelegate _openDatabaseDelegate;
		private CloseDatabaseDelegate _closeDatabase;
		private IsOpenDelegate _isOpen;
		private GetErrorCodeDelegate _getErrorCodeDelegate;
		private GetMetaDelegate _getMetaDelegate;
		private CreateCollectionDelegate _createCollectionDelegate;
		private GetCollectionDelegate _getCollectionDelegate;
		private SyncDelegate _syncDelegate;
		private CommandDelegate _commandDelegate;
		private SafeDatabaseHandle _databaseHandle;


		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbopen", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbopen([In] IntPtr db, [In] IntPtr path, int mode);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbopen")]
		private delegate bool OpenDatabaseDelegate([In] SafeDatabaseHandle database, [In] IntPtr path, [MarshalAs(UnmanagedType.I4)]OpenMode openMode);

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbclose", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbclose([In] IntPtr db);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbclose")]
		private delegate bool CloseDatabaseDelegate([In] SafeDatabaseHandle database);

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbisopen", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbisopen([In] IntPtr db);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbisopen")]
		private delegate bool IsOpenDelegate([In] SafeDatabaseHandle database);

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbecode", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern int _ejdbecode([In] IntPtr db);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbecode")]
		private delegate int GetErrorCodeDelegate([In] SafeDatabaseHandle database);

		////EJDB_EXPORT bson* ejdbmeta(EJDB *jb)
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbmeta", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbmeta([In] IntPtr db);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbmeta")]
		private delegate IntPtr GetMetaDelegate([In] SafeDatabaseHandle database);

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbgetcoll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbgetcoll([In] IntPtr db, [In] IntPtr cname);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbgetcoll")]
		private delegate IntPtr GetCollectionDelegate([In] SafeDatabaseHandle database, [In] IntPtr collectionName);

		//will use the only method for simplicity
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbcreatecoll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbcreatecoll([In] IntPtr db, [In] IntPtr cname, IntPtr opts);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbcreatecoll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbcreatecoll([In] IntPtr db, [In] IntPtr cname, ref EJDBCollectionOptionsN opts);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbcreatecoll")]
		private delegate IntPtr CreateCollectionDelegate([In] SafeDatabaseHandle database, [In] IntPtr collectionName, IntPtr options);

		////EJDB_EXPORT bson* ejdbcommand2(EJDB *jb, void *cmdbsondata);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbcommand2", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbcommand([In] IntPtr db, [In] byte[] cmd);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbcommand2")]
		private delegate IntPtr CommandDelegate([In] SafeDatabaseHandle database, [In] byte[] command);

		////EJDB_EXPORT bool ejdbsyncdb(EJDB *jb)
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbsyncdb", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbsyncdb([In] IntPtr db);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbsyncdb")]
		private delegate bool SyncDelegate([In] SafeDatabaseHandle database);

		public Database(Library library)
		{
			var libraryHandle = library.LibraryHandle;
			_databaseHandle = new SafeDatabaseHandle(libraryHandle);

			_library = library;
			
			_openDatabaseDelegate = libraryHandle.GetUnmanagedDelegate<OpenDatabaseDelegate>();
			_closeDatabase = libraryHandle.GetUnmanagedDelegate<CloseDatabaseDelegate>();
			_isOpen = libraryHandle.GetUnmanagedDelegate<IsOpenDelegate>();

			_getErrorCodeDelegate = libraryHandle.GetUnmanagedDelegate<GetErrorCodeDelegate>();
			_getMetaDelegate = libraryHandle.GetUnmanagedDelegate<GetMetaDelegate>();

			_createCollectionDelegate = libraryHandle.GetUnmanagedDelegate<CreateCollectionDelegate>();
			_getCollectionDelegate = libraryHandle.GetUnmanagedDelegate<GetCollectionDelegate>();
			_commandDelegate = libraryHandle.GetUnmanagedDelegate<CommandDelegate>();
			_syncDelegate = libraryHandle.GetUnmanagedDelegate<SyncDelegate>();
		}

		/// <summary>
		/// Gets the last DB error code or <c>null</c> if underlying native database object does not exist.
		/// </summary>
		/// <value>The last DB error code.</value>
		public int? LastErrorCode
		{
			get { return _getErrorCodeDelegate(_databaseHandle); }
		}

		/// <summary>
		/// Gets a value indicating whether this EJDB databse is open.
		/// </summary>
		/// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
		public bool IsOpen
		{
			get { return _isOpen(_databaseHandle); }
		}

		///// <summary>
		///// Gets info of EJDB database itself and its collections.
		///// </summary>
		///// <value>The DB meta.</value>
		//public BSONDocument DBMeta
		//{
		//	get
		//	{
		//		CheckDisposed(true);
		//		//internal static extern IntPtr _ejdbmeta([In] IntPtr db);
		//		IntPtr bsptr = _getMetaDelegate(this);
		//		if (bsptr == IntPtr.Zero)
		//		{
		//			throw new EJDBException(this);
		//		}
		//		try
		//		{
		//			int size;
		//			IntPtr bsdataptr = _bson_data2(bsptr, out size);
		//			byte[] bsdata = new byte[size];
		//			Marshal.Copy(bsdataptr, bsdata, 0, bsdata.Length);
		//			return new BSONDocument(bsdata);
		//		}
		//		finally
		//		{
		//			_bson_del(bsptr);
		//		}
		//	}
		//}

		/// <summary>
		/// Executes EJDB command.
		/// </summary>
		/// <remarks>
		/// Supported commands:
		///
		/// 1) Exports database collections data. See ejdbexport() method.
		/// 
		/// 	"export" : {
		/// 	"path" : string,                    //Exports database collections data
		/// 	"cnames" : [string array]|null,     //List of collection names to export
		/// 	"mode" : int|null                   //Values: null|`JBJSONEXPORT` See ejdb.h#ejdbexport() method
		/// }
		/// 
		/// Command response:
		/// {
		/// 	"log" : string,        //Diagnostic log about executing this command
		/// 	"error" : string|null, //ejdb error message
		/// 	"errorCode" : int|0,   //ejdb error code
		/// }
		/// 
		/// 2) Imports previously exported collections data into ejdb.
		/// 
		/// 	"import" : {
		/// 	"path" : string                     //The directory path in which data resides
		/// 		"cnames" : [string array]|null,     //List of collection names to import
		/// 		"mode" : int|null                //Values: null|`JBIMPORTUPDATE`|`JBIMPORTREPLACE` See ejdb.h#ejdbimport() method
		/// }
		/// 
		/// Command response:
		/// {
		/// 	"log" : string,        //Diagnostic log about executing this command
		/// 	"error" : string|null, //ejdb error message
		/// 	"errorCode" : int|0,   //ejdb error code
		/// }
		/// </remarks>
		/// <param name="cmd">Command object</param>
		/// <returns>Command response.</returns>
		//public BSONDocument Command(BSONDocument cmd)
		//{
		//	CheckDisposed();
		//	byte[] cmdata = cmd.ToByteArray();
		//	//internal static extern IntPtr _ejdbcommand([In] IntPtr db, [In] byte[] cmd);
		//	IntPtr cmdret = _commandDelegate(this, cmdata);
		//	if (cmdret == IntPtr.Zero)
		//	{
		//		return null;
		//	}
		//	byte[] bsdata = BsonPtrIntoByteArray(cmdret);
		//	if (bsdata.Length == 0)
		//	{
		//		return null;
		//	}
		//	BSONIterator it = new BSONIterator(bsdata);
		//	return it.ToBSONDocument();
		//}

		internal  IntPtr _ejdbgetcoll(IntPtr db, string cname)
		{
			IntPtr cptr = Native.NativeUtf8FromString(cname); //UnixMarshal.StringToHeap(cname, Encoding.UTF8);
			try
			{
				return _getCollectionDelegate(_databaseHandle, cptr);
			}
			finally
			{
				Marshal.FreeHGlobal(cptr); //UnixMarshal.FreeHeap(cptr);
			}
		}

		//internal IntPtr _ejdbcreatecoll(IntPtr db, String cname, EjdbCollectionOptionsN? opts)
		//{
		//	IntPtr cptr = Native.NativeUtf8FromString(cname);//UnixMarshal.StringToHeap(cname, Encoding.UTF8);
		//	try
		//	{
		//		if (opts == null)
		//		{
		//			return _createCollectionDelegate (this, cptr, IntPtr.Zero);
		//		}
		//		else
		//		{
		//			EjdbCollectionOptionsN nopts = (EjdbCollectionOptionsN)opts;
		//			return _createCollectionDelegate(this, cptr, ref nopts);
		//		}
		//	}
		//	finally
		//	{
		//		Marshal.FreeHGlobal(cptr); //UnixMarshal.FreeHeap(cptr);
		//	}
		//}


		public void Open(string dbFilePath, OpenMode openMode = DefaultOpenMode)
		{
			IntPtr pathPointer = Native.NativeUtf8FromString(dbFilePath); //UnixMarshal.StringToHeap(path, Encoding.UTF8);
			
			try
			{
				bool result = _openDatabaseDelegate(_databaseHandle, pathPointer, openMode);
				if (!result)
				{
					var errorCode = LastErrorCode;
					var errorMessage = errorCode.HasValue ? _library.GetLastErrorMessage(errorCode.Value) : "no error message provided";
					throw new EJDBException(errorMessage);
				}	
			}
			catch (Exception)
			{
				Dispose();
				throw;
			}
			finally
			{
				Marshal.FreeHGlobal(pathPointer); //UnixMarshal.FreeHeap(pptr);
			}
		}

		public void Dispose()
		{
			Close();
			_databaseHandle.Dispose();
		}

		public void Close()
		{
			if (_isOpen(_databaseHandle))
			{
				_closeDatabase(_databaseHandle);
			}
		}
	}
}