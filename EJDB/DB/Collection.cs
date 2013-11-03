using System;
using System.Runtime.InteropServices;
using Ejdb.Utils;
using Microsoft.Win32.SafeHandles;

namespace Ejdb.DB
{
	public class Collection : SafeHandleZeroOrMinusOneIsInvalid
	{
		//EJDB_EXPORT bool ejdbrmcoll(EJDB *jb, const char *colname, bool unlinkfile);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbrmcoll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbrmcoll([In] IntPtr db, [In] IntPtr cname, bool unlink);

		//EJDB_EXPORT bool ejdbsavebson3(EJCOLL *jcoll, void *bsdata, bson_oid_t *oid, bool merge);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbsavebson3", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbsavebson([In] IntPtr coll, [In] byte[] bsdata, [Out] byte[] oid, [In] bool merge);

		//EJDB_EXPORT bson* ejdbloadbson(EJCOLL *coll, const bson_oid_t *oid);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbloadbson", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbloadbson([In] IntPtr coll, [In] byte[] oid);

		//EJDB_EXPORT bool ejdbtranbegin(EJCOLL *coll);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbtranbegin", CallingConvention = CallingConvention.Cdecl)]

		//internal static extern bool _ejdbtranbegin([In] IntPtr coll);
		//EJDB_EXPORT bool ejdbtrancommit(EJCOLL *coll);
	
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbtrancommit", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbtrancommit([In] IntPtr coll);
		
		////EJDB_EXPORT bool ejdbtranabort(EJCOLL *coll);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbtranabort", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbtranabort([In] IntPtr coll);
		
		////EJDB_EXPORT bool ejdbtranstatus(EJCOLL *jcoll, bool *txactive);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbtranstatus", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbtranstatus([In] IntPtr coll, out bool txactive);

		////EJDB_EXPORT bool ejdbsyncoll(EJDB *jb)
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbsyncoll", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbsyncoll([In] IntPtr coll);
		
		////EJDB_EXPORT bool ejdbsetindex(EJCOLL *coll, const char *ipath, int flags);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbsetindex", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbsetindex([In] IntPtr coll, [In] IntPtr ipathptr, int flags);

		//internal static bool _ejdbrmcoll(IntPtr db, string cname, bool unlink)
		//{
		//	IntPtr cptr = Native.NativeUtf8FromString(cname);//UnixMarshal.StringToHeap(cname, Encoding.UTF8);
		//	try
		//	{
		//		return _ejdbrmcoll(db, cptr, unlink);
		//	}
		//	finally
		//	{
		//		Marshal.FreeHGlobal(cptr); //UnixMarshal.FreeHeap(cptr);
		//	}
		//}
	
		public Collection() : base(true)
		{
		}

		protected override bool ReleaseHandle()
		{
			throw new System.NotImplementedException();
		}
	}
}