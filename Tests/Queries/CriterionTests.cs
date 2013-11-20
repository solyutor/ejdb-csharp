using System;
using System.IO;
using Nejdb.Queries;
using NUnit.Framework;

namespace Nejdb.Tests.Queries
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
                _collection.Save(Person.Navalny(), false);
                _collection.Save(Person.Putin(), false);
                tx.Commit();
            }
        }

        [TearDown]
        public void TearDown()
        {
            _dataBase.Dispose();
        }

        protected void AssertFoundPutin(QueryBuilder builder)
        {
            using (var query = Collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                Assert.That(cursor.Count, Is.EqualTo(1));

                var person = cursor[0];
                Assert.That(person.Name.Surname, Is.EqualTo(Person.Putin().Name.Surname));
            }
        }

        protected void AssertFoundNavalny(QueryBuilder builder)
        {
            using (var query = Collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                var person = cursor[0];

                Assert.That(cursor.Count, Is.EqualTo(1));
                Assert.That(person.Name.Surname, Is.EqualTo(Person.Navalny().Name.Surname));
            }
        }

        protected void AssertFoundBoth(QueryBuilder builder)
        {
            using (var query = Collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                Assert.That(cursor.Count, Is.EqualTo(2));
            }
        }
    }
}