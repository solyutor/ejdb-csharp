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
            IntPtr resultPointer = _cursorResult(_cursorHandle, index, out size);

            if (resultPointer == IntPtr.Zero)
            {
                throw new EjdbException("Cursor result returned invalid pointer.");
            }

            return resultPointer;
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
        /// Returns query log if query executed with <see cref="QueryMode#Explain"/>
        /// </summary>
        /// <returns></returns>
        public string GetLog()
        {
            if (string.IsNullOrWhiteSpace(_cursorHandle.Log))
            {
                throw new InvalidOperationException("Query log available only with QueryMode.Explain hint.");
            }
            return _cursorHandle.Log;
        }

        /// <summary>
        /// Closes cursor and frees all resources.
        /// </summary>
        public void Dispose()
        {
            _cursorHandle.Dispose();
        }

        /// <summary>
        /// Throws <see cref="EjdbException"/> if cursor does not contains data.
        /// </summary>
        protected void EnsureValid()
        {
            if (IsValid) return;

            const string template = "This operation is not valid for current state of cursor. May by query executed with {0} hint?";
            var errorMessage = string.Format(template, QueryMode.Count);
            throw new EjdbException(errorMessage);
        }

        /// <summary>
        /// Ensures that correct index supplied and cursor is in valid state.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Index argument is invalid.</exception>
        /// /// <exception cref="EjdbException">Cursor state is invalid.</exception>
        /// <param name="index"></param>
        protected void EnsureInRange(int index)
        {
            EnsureValid();

            if (IsInRange(index)) return;

            const string template = "Expected index between 0 and {0}, but was {1}.";
            var errorMessage = string.Format(template, Count - 1, index);
            throw new ArgumentOutOfRangeException(errorMessage);
        }
    }
}