using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Nejdb.Bson;
using Nejdb.Internals;

namespace Nejdb
{
	public class Cursor : IDisposable, IEnumerable<BsonIterator>
	{
		////const void* ejdbqresultBsondata(EJQRESULT qr, int pos, int *size)
		//[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbqresultBsondata", CallingConvention = CallingConvention.Cdecl)]
		//static extern IntPtr _ejdbqresultBsondata([In] IntPtr qres, [In] int pos, out int size);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqueryhints")]
		private delegate IntPtr CursorResultDelegate([In] CursorHandle cursor, [In] int position, out int size);
		
		private readonly CursorHandle _cursorHandle;
		private readonly CursorResultDelegate _cursorResult;

		//current cursor position
		private int _position;
		//cursor Count
		private readonly int _count;

		internal Cursor(LibraryHandle libraryHandle, CursorHandle cursorHandle, int count)
		{
			_cursorHandle = cursorHandle;
			_count = count;
			_cursorResult = libraryHandle.GetUnmanagedDelegate<CursorResultDelegate>();
		}

		public BsonIterator this[int index]
		{
			get
			{
				if (_cursorHandle.IsInvalid  || index >= _count || index < 0)
				{
					return null;
				}
				//static extern IntPtr _ejdbqresultBsondata([In] IntPtr qres, [In] int index, out int size)
				int size;

				IntPtr bsdataptr = _cursorResult(_cursorHandle, index, out size);
				if (bsdataptr == IntPtr.Zero)
				{
					return null;
				}
				byte[] bsdata = new byte[size];
				Marshal.Copy(bsdataptr, bsdata, 0, bsdata.Length);
				return new BsonIterator(bsdata);
			}
		}

		/// <summary>
		/// Gets the number of result records stored in this cursor.
		/// </summary>
		public int Count
		{
			get
			{
				return _count;
			}
		}

		public BsonIterator Next()
		{
			if (_cursorHandle.IsInvalid || _position >= _count)
			{
				return null;
			}
			return this[_position++];
		}

		public IEnumerator<BsonIterator> GetEnumerator()
		{
			BsonIterator it;
			while ((it = Next()) != null)
			{
				yield return it;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Reset cursor position state to its initial value.
		/// </summary>
		public void Reset()
		{
			_position = 0;
		}

		public void Dispose()
		{
			_cursorHandle.Dispose();
		}
	}
}