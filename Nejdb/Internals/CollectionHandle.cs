using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Nejdb.Internals
{
	internal class CollectionHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private Database _database;
		
		private DatabaseHandle DatabaseHandle
		{
			get { return _database.DatabaseHandle; }
		}

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbgetcoll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbgetcoll([In] IntPtr db, [In] IntPtr name);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbgetcoll")]
		private delegate IntPtr GetCollectionDelegate([In] DatabaseHandle database, [In] IntPtr collectionName);

		//will use the only method for simplicity
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbcreatecoll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbcreatecoll([In] IntPtr db, [In] IntPtr name, IntPtr opts);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbcreatecoll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbcreatecoll([In] IntPtr db, [In] IntPtr name, ref EJDBCollectionOptionsN opts);
		//EJCOLL* ejdbcreatecoll(EJDB *jb, const char *colname, EJCOLLOPTS *opts) 
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbcreatecoll")]
		private delegate IntPtr CreateCollectionDelegate([In] DatabaseHandle database, [In] IntPtr collectionName, ref CollectionOptions options);

		//Creates collection with specified name
		public CollectionHandle(Database database, string name, CollectionOptions options) : base(false)
		{
			_database = database;

			var libraryHandle = DatabaseHandle.LibraryHandle;

			var createCollection = libraryHandle.GetUnmanagedDelegate<CreateCollectionDelegate>();

			IntPtr unmanagedName = Native.NativeUtf8FromString(name);//UnixMarshal.StringToHeap(name, Encoding.UTF8);
			try
			{
				handle = createCollection(DatabaseHandle, unmanagedName, ref options);

				if (IsInvalid)
				{
					throw EjdbException.FromDatabase(database, "Unknown error on collection creation");
				}
			}
			finally
			{
				Marshal.FreeHGlobal(unmanagedName); //UnixMarshal.FreeHeap(cptr);
			}
		}

		//gets collection with specified name
		public CollectionHandle(Database database, string name) : base(false)
		{
			_database = database;

			var libraryHandle = DatabaseHandle.LibraryHandle;
			var getCollection = libraryHandle.GetUnmanagedDelegate<GetCollectionDelegate>();

			IntPtr unmanagedName = Native.NativeUtf8FromString(name);//UnixMarshal.StringToHeap(name, Encoding.UTF8);
			try
			{
				handle = getCollection(DatabaseHandle, unmanagedName);

				if (IsInvalid)
				{
					//TODO: Use meta to get actual collection names
					throw EjdbException.FromDatabase(database, "Get collection error. May be collection does not exist?");
				}
			}
			finally
			{
				Marshal.FreeHGlobal(unmanagedName); //UnixMarshal.FreeHeap(cptr);
			}
		}

		protected override bool ReleaseHandle()
		{
			return true;
		}

		protected override void Dispose(bool disposing)
		{
			_database = null;
			base.Dispose(disposing);
		}
	}
}