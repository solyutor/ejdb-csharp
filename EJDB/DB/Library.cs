using System;
using System.Runtime.InteropServices;
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

		private readonly GetVersion _getVersion;

		/// <summary>
		/// The EJDB library version hex code.
		/// </summary>
		private readonly long _hexVersion;

		/// <summary>
		/// The EJDB library version
		/// </summary>
		private readonly string _version;

		private readonly GetErrorMessage _getErrorMessage;

		private Library(LibraryHandle libraryHandle)
		{
			LibraryHandle = libraryHandle;
			
			_getVersion = LibraryHandle.GetUnmanagedDelegate<GetVersion>();
			_getErrorMessage = LibraryHandle.GetUnmanagedDelegate<GetErrorMessage>();
			
			IntPtr version = _getVersion(LibraryHandle);
			
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

		public void Dispose()
		{
			LibraryHandle.Dispose();
		}
	}
}