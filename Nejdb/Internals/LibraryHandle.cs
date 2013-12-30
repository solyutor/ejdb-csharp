using Microsoft.Win32.SafeHandles;
using Nejdb.Infrastructure;

namespace Nejdb.Internals
{
    internal class LibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal string LibraryPath;

        public LibraryHandle() : base(true)
        {

        }

        protected override bool ReleaseHandle()
        {
            Platform.FreeLibraryEx(this);
            LibraryPath = null;
            return true;
        }
    }
}