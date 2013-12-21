using System;
using Microsoft.Win32.SafeHandles;

namespace Nejdb.Internals
{
    internal class QueryLogBuffer : SafeHandleZeroOrMinusOneIsInvalid
    {
        private readonly QueryFunctions _functions;

        public QueryLogBuffer(QueryFunctions functions) : base(true)
        {
            _functions = functions;
            handle = _functions.NewBuffer();
        }

        protected override bool ReleaseHandle()
        {
            _functions.DeleteBuffer(handle);
            return true;
        }

        public int Size
        {
            get { return _functions.BufferSize(handle); }
        }

        public IntPtr AsString()
        {
            return _functions.BufferToString(handle);
        }
    }
}