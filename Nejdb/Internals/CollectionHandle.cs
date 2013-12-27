using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Nejdb.Infrastructure;

namespace Nejdb.Internals
{
    internal class CollectionHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private Database _database;

        private DatabaseHandle DatabaseHandle
        {
            get { return _database.DatabaseHandle; }
        }

        //Creates collection with specified name
        public CollectionHandle(Database database, string name, CollectionOptions options) : base(false)
        {
            _database = database;

            IntPtr unmanagedName = Native.NativeUtf8FromString(name);//UnixMarshal.StringToHeap(name, Encoding.UTF8);
            try
            {
                handle = _database.Library.Functions.Collection.CreateCollection(DatabaseHandle, unmanagedName, ref options);

                if (IsInvalid)
                {
                    throw EjdbException.FromDatabase(database, "Unknown error on collection creation");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedName); //UnixMarshal.FreeHeap(cptr);
            }
        }

        //gets collection with specified name
        public CollectionHandle(Database database, string name)
            : base(false)
        {
            _database = database;


            IntPtr unmanagedName = Native.NativeUtf8FromString(name);//UnixMarshal.StringToHeap(name, Encoding.UTF8);
            try
            {
                handle = _database.Library.Functions.Collection.GetCollection(DatabaseHandle, unmanagedName);

                if (IsInvalid)
                {
                    //TODO: Use meta to get actual collection names
                    throw EjdbException.FromDatabase(database, "Get collection error. May be collection does not exist?");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(unmanagedName); //UnixMarshal.FreeHeap(cptr);
            }
        }

        protected override bool ReleaseHandle()
        {
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            _database = null;
            base.Dispose(disposing);
        }
    }
}