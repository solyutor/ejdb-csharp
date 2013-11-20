using System;
using System.IO;
using Nejdb.Bson;
using NUnit.Framework;

namespace Nejdb.Tests
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

        [TearDown]
        public void TearDown()
        {
            _dataBase.Dispose();
        }

        [Test]
        public void Can_execute_strongly_typed_query_with_count_hint()
        {
            _collection.Save(_originBson, false);

            using (var query = _collection.CreateQuery<Sample>())
            using (var cursor = query.Execute(QueryMode.Count))
            {
                Assert.That(cursor.Count, Is.EqualTo(1));

                Assert.Throws<EjdbException>(() => cursor.Next());
            }
        }

        [Test]
        public void Can_execute_non_typed_query_with_count_hint()
        {
            _collection.Save(_originBson, false);

            using (var query = _collection.CreateQuery())
            using (var cursor = query.Execute(QueryMode.Count))
            {
                Assert.That(cursor.Count, Is.EqualTo(1));

                Assert.Throws<EjdbException>(() => cursor.Next());
            }
        }

        [Test]
        public void Can_execute_strongly_typed_query_with_explain_hint()
        {
            _collection.Save(_originBson, false);

            using (var query = _collection.CreateQuery<Sample>())
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                var log = cursor.GetLog();

                Assert.That(string.IsNullOrWhiteSpace(log), Is.False);
                Console.WriteLine(log);
            }
        }

        [Test]
        public void Can_execute_non_typed_query_with_explain_hint()
        {
            _collection.Save(_originBson, false);

            using (var query = _collection.CreateQuery())
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                var log = cursor.GetLog();

                Assert.That(string.IsNullOrWhiteSpace(log), Is.False);
                Console.WriteLine(log);
            }
        }

        [Test]
        public void Can_execute_strongly_typed_query_count()
        {
            _collection.Save(_originBson, false);

            using (var query = _collection.CreateQuery<Sample>())
            {
                var count = query.Count();
                Assert.That(count, Is.EqualTo(1));
            }
        }

        [Test]
        public void Can_execute_non_typed_query_count()
        {
            _collection.Save(_originBson, false);

            using (var query = _collection.CreateQuery())
            {
                var count = query.Count();
                Assert.That(count, Is.EqualTo(1));
            }
        }

        [Test]
        public void Can_execute_non_typed_query()
        {
            _collection.Save(_originBson, false);

            using (var query = _collection.CreateQuery())
            using (var cursor = query.Execute())
            {
                Assert.That(cursor.Count, Is.EqualTo(1));
                var bsonDocument = new BsonDocument(cursor.Current);

                Console.WriteLine(bsonDocument);

                Assert.That(bsonDocument["_id"], Is.EqualTo(_originBson["_id"]));
            }
        }

        [Test]
        public void Can_execute_strongly_typed_query()
        {
            _collection.Save(_originSample, false);

            using (var query = _collection.CreateQuery<Sample>())
            using (var cursor = query.Execute())
            {
                var reloaded = cursor.Current;

                Assert.That(reloaded, Is.Not.Null);
                Assert.That(cursor.Count, Is.EqualTo(1));
                Assert.That(reloaded.Id, Is.EqualTo(_originSample.Id));
                Assert.That(reloaded.Name, Is.EqualTo(_originSample.Name));
            }
        }

        [Test]
        public void Can_execute_non_typed_query_with_find_one_hint()
        {
            _collection.Save(_originBson, false);

            using (var query = _collection.CreateQuery())
            {
                var bsonDocument = new BsonDocument(query.FinOne());

                Console.WriteLine(bsonDocument);

                Assert.That(bsonDocument["_id"], Is.EqualTo(_originBson["_id"]));
            }
        }

        [Test]
        public void Can_execute_strongly_typed_query_with_find_one_hint()
        {
            _collection.Save(_originSample, false);

            using (var query = _collection.CreateQuery<Sample>())
            {
                var reloaded = query.FinOne();

                Assert.That(reloaded, Is.Not.Null);
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

        [Test]
        public void Can_enumerate_over_non_typed_cursor()
        {
            _collection.Save(_originBson, false);

            using (var query = _collection.CreateQuery())
            using (var cursor = query.Execute())
            {
                foreach (var bsonIterator in cursor)
                {
                    Assert.That(bsonIterator, Is.Not.Null);
                }
            }
        }
    }
}