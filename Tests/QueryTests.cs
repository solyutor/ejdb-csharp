using System;
using System.Configuration;
using System.IO;
using Nejdb;
using Nejdb.Bson;
using NUnit.Framework;

namespace Ejdb.Tests
{
    [TestFixture]
    public class QueryTests
    {
        private Database _dataBase;
        private Collection _collection;
        private BsonDocument _originBson;
        private Sample _originSample;
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

            _collection = _dataBase.CreateCollection("default", new CollectionOptions());

            _originBson = BsonDocument.ValueOf(new { Name = "Denis Gladiolus" });

            _originSample = new Sample { Name = "Denis Gladiolus" };
        }

        [Test]
        public void Can_execute_query_with_count_hint()
        {
            _collection.Save(_originBson, false);

            using (var query = _collection.CreateQuery<Sample>())
            using (var cursor = query.Execute(QueryMode.Count))
            {
                Assert.That(cursor.Count, Is.EqualTo(1));

                var sample = cursor.Next();

                Assert.That(sample, Is.Null);
            }
        }

        [Test]
        public void Can_execute_query()
        {
            _collection.Save(_originBson, false);

            using (var query = _collection.CreateQuery())
            using (var cursor = query.Execute())
            {
                Assert.That(cursor.Count, Is.EqualTo(1));
                Console.WriteLine(new BsonDocument(cursor.Next()));
            }
        }

        [Test]
        public void Can_execute_strongly_typed_query()
        {
            _collection.Save(_originSample, false);

            using (var query = _collection.CreateQuery<Sample>())
            using (var cursor = query.Execute())
            {
                var reloaded = cursor.Next();

                Assert.That(reloaded, Is.Not.Null);
                Assert.That(cursor.Count, Is.EqualTo(1));
                Assert.That(reloaded.Id, Is.EqualTo(_originSample.Id));
                Assert.That(reloaded.Name, Is.EqualTo(_originSample.Name));
            }
        }

        [Test]
        public void Can_enumerate_over_strongly_typed_cursor()
        {
            _collection.Save(_originSample, false);

            using (var query = _collection.CreateQuery<Sample>())
            using (var cursor = query.Execute())
            {
                foreach (Sample sample in cursor)
                {
                    Assert.That(sample, Is.Not.Null);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            _dataBase.Dispose();
        }
    }
}