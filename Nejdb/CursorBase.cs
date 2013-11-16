using System;
using System.Runtime.InteropServices;
using Nejdb.Internals;

namespace Nejdb
{
    /// <summary>
    /// Encapsulates shared functionality for all cursors
    /// </summary>
    public class CursorBase : IDisposable
    {
        ////const void* ejdbqresultBsondata(EJQRESULT qr, int pos, int *size)
        //[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbqresultBsondata", CallingConvention = CallingConvention.Cdecl)]
        //static extern IntPtr _ejdbqresultBsondata([In] IntPtr qres, [In] int pos, out int size);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqresultbsondata")]
        private delegate IntPtr CursorResultDelegate([In] CursorHandle cursor, [In] int position, out int size);
        
        private CursorHandle _cursorHandle;
        private CursorResultDelegate _cursorResult;
        private int _position;
        private int _count;

        internal CursorBase(LibraryHandle libraryHandle, CursorHandle cursorHandle, int count)
        {
            _cursorHandle = cursorHandle;
            _count = count;
            _cursorResult = libraryHandle.GetUnmanagedDelegate<CursorResultDelegate>();
        }

        internal IntPtr CursorResult(int index, out int size)
        {
            IntPtr bsdataptr = _cursorResult(_cursorHandle, index, out size);
            
            return bsdataptr;
        }

        internal bool IsInvalid
        {
            get { return _cursorHandle.IsInvalid; }
        }

        internal bool IsValid
        {
            get { return !IsInvalid; }
        }

        /// <summary>
        /// Gets the number of result records stored in this cursor.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Returns current position of cursor
        /// </summary>
        public int Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Returns true if index is not valid for current cursor, false otherwise.
        /// </summary>
        /// <param name="index">Index of a result in cursor</param>
        /// <returns></returns>
        public bool IsInRange(int index)
        {
            return 0 <= index && index < Count;
        }

        /// <summary>
        /// Increases current position and returns it's value
        /// </summary>
        /// <returns></returns>
        protected int NextPosition()
        {
            return _position++;
        }

        /// <summary>
        /// Reset cursor position state to its initial value.
        /// </summary>
        public void Reset()
        {
            _position = 0;
        }

        /// <summary>
        /// Closes cursor and frees all resources.
        /// </summary>
        public void Dispose()
        {
            _cursorHandle.Dispose();
        }
    }
}