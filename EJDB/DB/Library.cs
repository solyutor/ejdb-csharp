using System;
using System.Runtime.InteropServices;
using Ejdb.BSON;
using Ejdb.Utils;

namespace Ejdb.DB
{
	public class Library : IDisposable
	{
		internal readonly LibraryHandle LibraryHandle;

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbversion", CallingConvention = CallingConvention.Cdecl)]
		//EJDB_EXPORT const char *ejdbversion();
		//internal static extern IntPtr _ejdbversion();
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbversion")]
		private delegate IntPtr GetVersion(LibraryHandle handle);

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdberrmsg", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdberrmsg(int ecode)
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdberrmsg")]
		private delegate IntPtr GetErrorMessage(int errorCode);


		//EJDB_EXPORT void bson_del(bson *b);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "bson_del", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern void _bson_del([In] IntPtr bsptr);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("bson_del")]
		private delegate void FreeBsonDelegate(IntPtr bson);

		//EJDB_EXPORT const char* bson_data2(const bson *b, int *bsize);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "bson_data2", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _bson_data2([In] IntPtr bsptr, out int size);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("bson_data2")]
		private delegate IntPtr BsonToStringDelegate(BsonHandle bson, out int size);


		//EJDB_EXPORT bson* json2bson(const char *jsonstr);
		//[DllImport(EJDB_LIB_NAME, EntryPoint = "json2bson", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _json2bson([In] IntPtr jsonstr);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("json2bson")]
		private delegate IntPtr JsonToBsonDelegate([In]IntPtr json);

		/// <summary>
		/// The EJDB library version hex code.
		/// </summary>
		private readonly long _hexVersion;

		/// <summary>
		/// The EJDB library version
		/// </summary>
		private readonly string _version;

		private readonly GetErrorMessage _getErrorMessage;
		private readonly FreeBsonDelegate _freeBson;
		private readonly BsonToStringDelegate _bsonToSTring;
		private JsonToBsonDelegate _jsonToBson;

		private Library(LibraryHandle libraryHandle)
		{
			LibraryHandle = libraryHandle;
			
			
			_getErrorMessage = LibraryHandle.GetUnmanagedDelegate<GetErrorMessage>();

			_freeBson = libraryHandle.GetUnmanagedDelegate<FreeBsonDelegate>();
			_bsonToSTring = libraryHandle.GetUnmanagedDelegate<BsonToStringDelegate>();
			//_jsonToBson = libraryHandle.GetUnmanagedDelegate<JsonToBsonDelegate>();

			var getVersion = LibraryHandle.GetUnmanagedDelegate<GetVersion>();

			IntPtr version = getVersion(LibraryHandle);
			
			if (version == IntPtr.Zero)
			{
				throw new Exception("Unable to get ejdb library version");
			}

			_version = Native.StringFromNativeUtf8(version); //UnixMarshal.PtrToString(vres, Encoding.UTF8);
			_hexVersion = Convert.ToInt64("0x" + Version.Replace(".", ""), 16);
		}

		/// <summary>
		/// Gets the EJDB library hex encoded version.
		/// </summary>
		/// <remarks>
		/// E.g: for the version "1.1.13" return value will be: 0x1113
		/// </remarks>
		/// <value>The lib hex version.</value>
		public long HexVersion
		{
			get { return _hexVersion; }
		}

		/// <summary>
		/// Gets the EJDB library version.
		/// </summary>
		/// <value>The LIB version.</value>
		public string Version
		{
			get { return _version; }
		}

		public static Library Create()
		{
			var libraryHandle = LibraryHandle.Load();

			var result = new Library(libraryHandle);

			return result;
		}

		public Database CreateDatabase()
		{
			return new Database(this);
		}

		public string GetLastErrorMessage(int errorCode)
		{
			return Native.StringFromNativeUtf8(_getErrorMessage(errorCode)); //UnixMarshal.PtrToString(_ejdberrmsg((int) ecode), Encoding.UTF8);
		}
		//Used internally by BsonHandle
		internal void FreeBson(IntPtr bson)
		{
			_freeBson(bson);
		}

		internal BSONDocument ConvertToBsonDocument(BsonHandle bson)
		{
			int size;
			IntPtr bsdataptr = _bsonToSTring(bson, out size);
			byte[] bsdata = new byte[size];
			Marshal.Copy(bsdataptr, bsdata, 0, bsdata.Length);
			return new BSONDocument(bsdata);
		}

		///// <summary>
		///// Convert JSON string into BSONDocument.
		///// Returns `null` if conversion failed.
		///// </summary>
		///// <returns>The BSONDocument instance on success.</returns>
		///// <param name="json">JSON string</param>
		//public BSONDocument Json2Bson(string json)
		//{
		//	IntPtr jsonptr = Native.NativeUtf8FromString(json);

		//	try
		//	{
				
		//		using (var bson = new BsonHandle())_jsonToBson(jsonptr))
		//		{
		//			return ConvertToBsonDocument(bson);
		//		}
		//	}
		//	finally
		//	{
		//		Marshal.FreeHGlobal(jsonptr); //UnixMarshal.FreeHeap(jsonptr);
		//	}
		//}

		public void Dispose()
		{
			LibraryHandle.Dispose();
		}
	}
}