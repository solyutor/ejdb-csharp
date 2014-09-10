using Codestellation.Quarks.Native;
using Microsoft.Win32.SafeHandles;

namespace Nejdb.Internals
{
    internal class DatabaseHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private readonly Library _library;

        public DatabaseHandle(Library library) : base(true)
        {
            _library = library;
            handle = library.Functions.Database.NewInstance(library.LibraryHandle);

            if (IsInvalid)
            {
                throw new EjdbException("Unable to create ejdb instance");
            }
        }

        public LibraryHandle LibraryHandle
        {
            get { return _library.LibraryHandle; }
        }

        protected override bool ReleaseHandle()
        {
            _library.Functions.Database.DeleteInstance(handle);
            return true;
        }
    }
}