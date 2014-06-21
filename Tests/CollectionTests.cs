using System;
using System.IO;
using System.Runtime.InteropServices;
using Nejdb.Bson;
using NUnit.Framework;

namespace Nejdb.Tests
{
    [TestFixture]
    public class CollectionTests
    {
        private Database _dataBase;
        private Collection _collection;
        private BsonDocument _origin;
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

            _origin = BsonDocument.ValueOf(new
                {
                    name = "Grenny",
                    type = "African Grey",
                    male = true,
                    age = 1,
                    birthdate = DateTime.Now,
                    likes = new[] { "green color", "night", "toys" },
                    extra = BsonNull.VALUE
                });
        }

        [TearDown]
        public void TearDown()
        {
            _dataBase.Dispose();
        }

        [Test]
        public void Can_begin_and_commit_transaction()
        {
            Transaction transaction;
            using (transaction = _collection.BeginTransaction())
            {
                _collection.Save(_origin, false);
                Assert.That(transaction.Active, Is.True);
                transaction.Commit();
            }
            Assert.That(transaction.Active, Is.False);

            using (var query = _collection.CreateQuery())
            {
                var count = query.Count();

                Assert.That(count, Is.EqualTo(1), "The sample document was not persisted");
            }
        }

        [Test]
        public void Can_begin_and_rollback_transaction()
        {
            Transaction transaction;
            using (transaction = _collection.BeginTransaction())
            {
                _collection.Save(_origin, false);
                Assert.That(transaction.Active, Is.True);
                transaction.Rollback();
            }
            Assert.That(transaction.Active, Is.False);

            using (var query = _collection.CreateQuery())
            {
                var count = query.Count();

                Assert.That(count, Is.EqualTo(0), "The sample document was persisted");
            }
        }

        [Test]
        public void Can_synchronize_collection()
        {
            _collection.Synchronize();

            //TODO: Assert something?
        }

        [Test]
        public void Can_save_and_load_strongly_typed_document()
        {
            var origin = new Sample { Name = "John Wayne" };
            var id = _collection.Save(origin, false);

            var reloaded = _collection.Load<Sample>(id);

            Assert.That(reloaded.Id, Is.EqualTo(origin.Id));
            Assert.That(reloaded.Name, Is.EqualTo(origin.Name));
        }
        
        [Test]
        public void Can_save_and_load_strongly_typed_document_with_public_id()
        {
            var origin = new PublicObjectIdSample { Name = "John Wayne" };
            var id = _collection.Save(origin, false);

            var reloaded = _collection.Load<PublicObjectIdSample>(id);

            Assert.That(reloaded.Id, Is.EqualTo(origin.Id));
            Assert.That(reloaded.Name, Is.EqualTo(origin.Name));
        }

        [Test]
        public void Can_update_strongly_typed_document()
        {
            var origin = new Sample { Name = "John Wayne" };
            var id = _collection.Save(origin, false);

            var reloaded = _collection.Load<Sample>(id);


            reloaded.Name = "Max Payne";

            var id2 = _collection.Save<Sample>(reloaded, false);

            var reloaded2 = _collection.Load<Sample>(id);

            Assert.That(id,  Is.EqualTo(id2));
            Assert.That(reloaded2.Name, Is.EqualTo(reloaded.Name));
        }

        [Test]
        public void Can_save_and_load_document()
        {
            _collection.Save(_origin, false);

            var id = _origin.GetBsonValue("_id");

            var reloaded = _collection.Load((ObjectId)id.Value);
            //TODO: made more string assertion
            Assert.That(reloaded, Is.Not.Null);
        }

        [Test]
        public void Can_delete_document()
        {
            _collection.Save(_origin, false);
            var id = _origin.GetBsonValue("_id");

            var BsonOid = (ObjectId)id.Value;

            _collection.Delete(BsonOid);

            _collection.Synchronize();
            var reloaded = _collection.Load(BsonOid);

            var meta = _dataBase.DatabaseMetadata;

            Console.WriteLine(meta);

            Assert.That(reloaded, Is.Null);
        }

        [Test]
        public void Can_create_index()
        {
            _collection.Index("name", IndexOperations.String);

            var meta = _dataBase.DatabaseMetadata;

            var metaAsString = meta.ToString();
            const string indexEvidence = "test.db_default.idx.sname.lex";

            StringAssert.Contains(indexEvidence, metaAsString);
        }
    }
}