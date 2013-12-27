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
        public unsafe CollectionHandle(Database database, string name, CollectionOptions options)
            : base(false)
        {
            _database = database;

            UnsafeBuffer buffer;
            UnsafeBuffer.FillFromString(&buffer, name);

            handle = _database.Library.Functions.Collection.CreateCollection(DatabaseHandle, &buffer, ref options);

            if (IsInvalid)
            {
                throw EjdbException.FromDatabase(database, "Unknown error on collection creation");
            }

        }

        //gets collection with specified name
        public unsafe CollectionHandle(Database database, string name)
            : base(false)
        {
            _database = database;

            UnsafeBuffer buffer;
            UnsafeBuffer.FillFromString(&buffer, name);
            handle = _database.Library.Functions.Collection.GetCollection(DatabaseHandle, &buffer);

            if (IsInvalid)
            {
                //TODO: Use meta to get actual collection names
                throw EjdbException.FromDatabase(database, "Get collection error. May be collection does not exist?");
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