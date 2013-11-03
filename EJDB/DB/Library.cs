using System;
using System.IO;
using System.Runtime.InteropServices;
using Ejdb.DB;
using Microsoft.Win32.SafeHandles;

namespace Ejdb.Utils
{
	public class Library : SafeHandleZeroOrMinusOneIsInvalid
	{
		private string _libraryPath;

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern Library LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeLibrary(IntPtr hModule);

		private NewInstance _newInstance;

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbnew", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbnew();
		//Creates new instance of ejdb. Don't know what' is it, but it looks it should be done before opening database
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbnew")]
		private delegate Database NewInstance(Library handle);

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbversion", CallingConvention = CallingConvention.Cdecl)]
		//EJDB_EXPORT const char *ejdbversion();
		//internal static extern IntPtr _ejdbversion();
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbversion")]
		private delegate IntPtr GetVersion(Library handle);

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdberrmsg", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdberrmsg(int ecode)
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdberrmsg")]
		private delegate IntPtr GetErrorMessage(int errorCode);

		private GetVersion _getVersion;

		private Library() : base(true) { }

		/// <summary>
		/// The EJDB library version hex code.
		/// </summary>
		private long _hexVersion;

		/// <summary>
		/// The EJDB library version
		/// </summary>
		private string _version;

		private GetErrorMessage _getErrorMessage;

		protected override bool ReleaseHandle()
		{
			FreeLibrary(this.handle);

			if (File.Exists(_libraryPath))
			{
				File.Delete(_libraryPath);
			}

			return true;
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

		public static Library Load()
		{
			var libraryPath = ResourceHelper.ExportLibrary();
			var safeLoadLibrary = LoadLibrary(libraryPath);
			if (safeLoadLibrary.IsInvalid)
			{
				var error = Marshal.GetLastWin32Error();
				throw new InvalidOperationException("Win32 error " + error);
			}
			safeLoadLibrary.Initialize(libraryPath);

			return safeLoadLibrary;
		}

		private void Initialize(string libraryPath)
		{
			_libraryPath = libraryPath;
			_newInstance = this.GetUnmanagedDelegate<NewInstance>();
			_getVersion = this.GetUnmanagedDelegate<GetVersion>();
			_getErrorMessage = this.GetUnmanagedDelegate<GetErrorMessage>();
			IntPtr vres = _getVersion(this);
			if (vres == IntPtr.Zero)
			{
				throw new Exception("Unable to get ejdb library version");
			}
			_version = Native.StringFromNativeUtf8(vres); //UnixMarshal.PtrToString(vres, Encoding.UTF8);
			_hexVersion = Convert.ToInt64("0x" + Version.Replace(".", ""), 16);
		}

		public Database OpenDatabase(string path, OpenMode openMode = Database.DefaultOpenMode)
		{
			//No need for this check. I know exactly what version of lib is in embedded resouce
			//if (HexVersion < 0x1113)
			//{
			//	throw new EJDBException("EJDB library version must be at least '1.1.13' or greater");
			//}

			var dbHandle = _newInstance(this);
			if (dbHandle.IsInvalid)
			{
				throw new EJDBException("Unable to create ejdb instance");
			}
			dbHandle.Initialize(this);
			dbHandle.Open(path, openMode);
			return dbHandle;
		}

		public string GetLastErrorMessage(int errorCode)
		{
			return Native.StringFromNativeUtf8(_getErrorMessage(errorCode)); //UnixMarshal.PtrToString(_ejdberrmsg((int) ecode), Encoding.UTF8);
		}
	}
}