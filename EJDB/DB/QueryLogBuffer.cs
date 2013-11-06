using System;
using System.Runtime.InteropServices;
using Ejdb.Utils;
using Microsoft.Win32.SafeHandles;

namespace Ejdb.DB
{
	internal class QueryLogBuffer : SafeHandleZeroOrMinusOneIsInvalid
	{
		private NewBufferDelegate _newBuffder;
		private DeleteBufferDelegate _delete;
		private GetSizeDelegate _size;
		private ToStringDelegate _toString;

		////EJDB_EXPORT TCXSTR *tcxstrnew(void)
		//[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "tcxstrnew", CallingConvention = CallingConvention.Cdecl)]
		//static extern IntPtr _tcxstrnew();
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("tcxstrnew")]
		private delegate IntPtr NewBufferDelegate();

		////EJDB_EXPORT void tcxstrdel(TCXSTR *xstr);
		//[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "tcxstrdel", CallingConvention = CallingConvention.Cdecl)]
		//static extern IntPtr _tcxstrdel([In] IntPtr strptr);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("tcxstrdel")]
		private delegate IntPtr DeleteBufferDelegate(IntPtr query);

		////EJDB_EXPORT int tcxstrsize(const TCXSTR *xstr);
		//[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "tcxstrsize", CallingConvention = CallingConvention.Cdecl)]
		//static extern int _tcxstrsize([In] IntPtr strptr);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("tcxstrsize")]
		private delegate int GetSizeDelegate(IntPtr query);

		////EJDB_EXPORT int tcxstrptr(const TCXSTR *xstr);
		//[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "tcxstrptr", CallingConvention = CallingConvention.Cdecl)]
		//static extern IntPtr _tcxstrptr([In] IntPtr strptr);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("tcxstrptr")]
		private delegate IntPtr ToStringDelegate(IntPtr query);

		public QueryLogBuffer(LibraryHandle libraryHandle) : base(true)
		{
			_newBuffder = libraryHandle.GetUnmanagedDelegate<NewBufferDelegate>();
			_delete = libraryHandle.GetUnmanagedDelegate<DeleteBufferDelegate>();
			_size = libraryHandle.GetUnmanagedDelegate<GetSizeDelegate>();
			_toString = libraryHandle.GetUnmanagedDelegate<ToStringDelegate>();

			handle = _newBuffder();
		}

		protected override bool ReleaseHandle()
		{
			_delete(handle);
			return true;
		}

		public int Size
		{
			get { return _size(handle); }
		}

		public IntPtr AsString()
		{
			return _toString(handle);
		}
	}
}