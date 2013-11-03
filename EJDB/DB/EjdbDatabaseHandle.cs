using System;
using System.Runtime.InteropServices;
using Ejdb.DB;
using Microsoft.Win32.SafeHandles;

namespace Ejdb.Utils
{
	public class EjdbDatabaseHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		/// <summary>
		/// The default open mode (OpenMode.Writer | OpenMode.CreateIfNotExists) <c>(JBOWRITER | JBOCREAT)</c>
		/// </summary>
		public const OpenMode DefaultOpenMode = (OpenMode.Writer | OpenMode.CreateIfNotExists);

		private EjbdLibraryHandle _library;
		private EjdbDel _ejdbDel;

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbdel", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern IntPtr _ejdbdel([In] IntPtr db);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbdel")]
		private delegate void EjdbDel(EjdbDatabaseHandle handle);

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbopen", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbopen([In] IntPtr db, [In] IntPtr path, int mode);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbopen")]
		private delegate bool OpenDatabase([In] EjdbDatabaseHandle handle);

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbclose", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbclose([In] IntPtr db);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbclose")]
		private delegate bool CloseDatabase([In] EjdbDatabaseHandle handle);

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbisopen", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern bool _ejdbisopen([In] IntPtr db);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbisopen")]
		private delegate bool IsOpen([In] EjdbDatabaseHandle handle);

		//[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbecode", CallingConvention = CallingConvention.Cdecl)]
		//internal static extern int _ejdbecode([In] IntPtr db);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbecode")]
		private delegate int GetErrorCode([In] EjdbDatabaseHandle handle);

		public void Initialize(EjbdLibraryHandle library)
		{
			_library = library;
			_ejdbDel = _library.GetUnmanagedDelegate<EjdbDel>();
			_openDatabase = _library.GetUnmanagedDelegate<OpenDatabase>();
			_closeDatabase = _library.GetUnmanagedDelegate<CloseDatabase>();
			_isOpen = _library.GetUnmanagedDelegate<IsOpen>();
			_getErrorCode = _library.GetUnmanagedDelegate<GetErrorCode>();
		}

		public EjdbDatabaseHandle() : base(true)
		{
		}

		protected override bool ReleaseHandle()
		{
			_ejdbDel(this);
			return true;
		}

		public EjdbDatabaseHandle Open(string dbFilePath, OpenMode openMode)
		{
			bool rv;
			

			
			try
			{
				rv = _ejdbopen(_db, path, omode);
			}
			catch (Exception)
			{
				Dispose();
				throw;
			}
			if (!rv)
			{
				throw new EJDBException(this);
			}




			
		}

		
	}
}