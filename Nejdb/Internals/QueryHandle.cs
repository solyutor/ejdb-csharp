using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Nejdb.Bson;

namespace Nejdb.Internals
{
	public class QueryHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private readonly DeleteQueryDelegate _delete;
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

		public QueryHandle(Database database) : base(true)
		{
			var libraryHandle = database.Library.LibraryHandle;
			
			var createQuery = libraryHandle.GetUnmanagedDelegate<CreateQueryDelegate>();

			handle = createQuery(database.DatabaseHandle, BsonDocument.Empty.ToByteArray());

			if (IsInvalid)
			{
				EjdbException.FromDatabase(database, "Failed to create query");
			}

			_delete = libraryHandle.GetUnmanagedDelegate<DeleteQueryDelegate>();
		}

		protected override bool ReleaseHandle()
		{
			_delete(handle);
			return true;
		}
	}
}