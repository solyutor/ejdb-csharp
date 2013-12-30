using System;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Nejdb.Infrastructure;

namespace Nejdb.Internals
{
    internal class QueryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public readonly Collection Collection;
        private QueryFunctions _functions;

        public QueryHandle(Collection collection, byte[] queryAsBson) : base(true)
        {
            Collection = collection;
            var database = collection.Database;
            _functions = database.Library.Functions.Query;

            handle = _functions.Create(database.DatabaseHandle, queryAsBson);

            if (IsInvalid)
            {
                EjdbException.FromDatabase(database, "Failed to create query");
            }
        }

        internal void AddOr(byte[] bson)
        {
            IntPtr qptr = _functions.AddOr(Collection.Database.DatabaseHandle, this, bson);
            if (qptr == IntPtr.Zero)
            {
                EjdbException.FromDatabase(Collection.Database, "Failed to append Or restriction");
                //throw new EJDBQueryException(_jb);
            }
        }

        internal void SetHints(QueryHints hints)
        {
            if (hints.IsEmpty)
            {
                return;
            }

            IntPtr qptr = _functions.SetHints(Collection.Database.DatabaseHandle, this, hints.ToByteArray());

            if (qptr == IntPtr.Zero)
            {
                //TODO: throw more specific exception
                //throw new EJDBQueryException(_jb);
                EjdbException.FromDatabase(Collection.Database, "Failed to set hints");
            }
        }

        internal CursorHandle Execute(QueryMode flags, out int count)
        {
            QueryLogBuffer logsptr = null;
            //TODO: check it more elegant way?
            if ((flags & QueryMode.Explain) != 0)
            {
                //static extern IntPtr _tcxstrnew();
                logsptr = new QueryLogBuffer(Collection.Database.Library.Functions.Query); //Create dynamic query execution log buffer
            }

            CursorHandle cursorHandle = null;
            try
            {
                int resultCount = 0;
                //static extern IntPtr _ejdbqryexecute([In] IntPtr jcoll, [In] IntPtr q, out int count, [In] int qflags, [In] IntPtr logxstr);
                IntPtr bufferPointer = logsptr == null ? IntPtr.Zero : logsptr.DangerousGetHandle();
                cursorHandle = new CursorHandle(() => _functions.Execute(Collection.CollectionHandle, this, out resultCount, flags, bufferPointer), pointer => _functions.DeleteCursor(pointer));
                count = resultCount;
            }
            finally
            {
                if (logsptr != null)
                {
                    try
                    {
                        if (cursorHandle != null && !cursorHandle.IsInvalid)
                            unsafe
                            {
                                //static extern IntPtr _tcxstrptr([In] IntPtr strptr);
                                var sbptr = logsptr.AsString();
                                cursorHandle.Log = new string(sbptr);
                            }
                    }
                    finally
                    {
                        //static extern IntPtr _tcxstrdel([In] IntPtr strptr);
                        logsptr.Dispose();
                    }
                }
            }
            Collection.Database.ThrowOnError();

            return cursorHandle;
        }

        protected override bool ReleaseHandle()
        {
            _functions.Delete(handle);
            _functions = null;
            return true;
        }
    }
}