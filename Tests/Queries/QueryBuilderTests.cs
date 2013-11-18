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
        public void Match_criterion_query()
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
        public void Negate_criterion_query()
        {
            var criterion = Criterions.Not(Criterions.Equals("Alexey"));
            var builder = new QueryBuilder<Person>().Where(x => x.Name.First, criterion);

            using (var query = _collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                var person = cursor[0];

                Assert.That(cursor.Count, Is.EqualTo(1));
                Assert.That(person.Name.Surname, Is.EqualTo(Person.Putin().Name.Surname));
            }
        }

        [Test]
        public void Starts_with_criterion_query()
        {
            var startsWith = Criterions.StartsWith("Ale");
            var builder = new QueryBuilder<Person>().Where(x => x.Name.First, startsWith);
            
            using (var query = _collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                var person = cursor[0];

                Assert.That(cursor.Count, Is.EqualTo(1));
                Assert.That(person.Name.Surname, Is.EqualTo(Person.Navalny().Name.Surname));
            }
        }
    }
}