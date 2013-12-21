namespace Nejdb.Internals
{
    internal class Functions
    {
        public Functions(LibraryHandle handle)
        {
            Database = new DatabaseFunctions(handle);
            Collection = new CollectionFunctions(handle);
            Query = new QueryFunctions(handle);
        }

        internal readonly QueryFunctions Query;

        internal readonly CollectionFunctions Collection;

        internal readonly DatabaseFunctions Database;
    }
}