using System;
using System.IO;
using Nejdb;
using Nejdb.Queries;
using NUnit.Framework;

namespace Ejdb.Tests
{
    [TestFixture]
    public class QueryBuilderTests
    {
        private Database _dataBase;
        private Collection _collection;
        private const string DbName = "test.db";

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

        [Test]
        public void Simple_match_query()
        {
            var builder = new QueryBuilder<Person>().Where(x => x.Name.First, Criterions.Equals("Alexey"));

            using (var query = _collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                var person = cursor[0];

                Assert.That(cursor.Count, Is.EqualTo(1));
                Assert.That(person.Name.Surname, Is.EqualTo(Person.Navalny().Name.Surname));
            }
        }

        [Test]
        public void Simple_not_match_query()
        {
            var builder = new QueryBuilder<Person>().Where(x => x.Name.First, Criterions.NotEquals("Alexey"));

            using (var query = _collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                var person = cursor[0];

                Assert.That(cursor.Count, Is.EqualTo(1));
                Assert.That(person.Name.Surname, Is.EqualTo(Person.Putin().Name.Surname));
            }
        }
    }
}