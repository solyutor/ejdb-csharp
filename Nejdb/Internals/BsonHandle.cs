using System;
using Microsoft.Win32.SafeHandles;

namespace Nejdb.Internals
{
    internal class BsonHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private Library _library;

        public BsonHandle(Func<IntPtr> create, Library library) : base(true)
        {
            _library = library;
            handle = create();
            
        }

        public unsafe byte* GetBsonBuffer()
        {
            int size;
            return _library.GetBsonData(this, out size);
        }

        protected override bool ReleaseHandle()
        {
            if (_library != null)
            {
                _library.FreeBson(handle);
            }
            _library = null;
            return true;
        }
    }
}