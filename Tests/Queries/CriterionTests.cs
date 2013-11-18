using System.IO;
using Nejdb;
using NUnit.Framework;

namespace Ejdb.Tests
{
    public abstract class CriterionTests
    {
        private const string DbName = "test.db";
        private Database _dataBase;
        private Collection _collection;

        public Collection Collection
        {
            get { return _collection; }
        }

        [SetUp]
        public void Setup()
        {
            if (File.Exists(DbName))
            {
                File.Delete(DbName);
            }
            _dataBase = Library.Instance.CreateDatabase();

            _dataBase.Open(DbName, Database.DefaultOpenMode | OpenMode.TruncateOnOpen);

            _collection = _dataBase.CreateCollection("Persons", new CollectionOptions());
            
            using (var tx = _collection.BeginTransaction())
            {
                _collection.Save<Person>(Person.Navalny(), false);
                _collection.Save<Person>(Person.Putin(), false);
                tx.Commit();
            }
        }

        [TearDown]
        public void TearDown()
        {
            _dataBase.Dispose();
        }
    }
}