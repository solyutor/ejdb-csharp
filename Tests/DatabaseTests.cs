using System;
using System.IO;
using Ejdb.BSON;
using Ejdb.DB;
using NUnit.Framework;

namespace Ejdb.Tests
{
	[TestFixture]
	public class DatabaseTests
	{
		private Library _library;
		private Database _dataBase;
		private const string DbName = "test.db";

		[SetUp]
		public void Setup()
		{
			if (File.Exists(DbName))
			{
				File.Delete(DbName);
			}
			_library = Library.Create();

			_dataBase = _library.CreateDatabase();
		}

		[TearDown]
		public void TearDown()
		{
			_dataBase.Dispose();
			_library.Dispose();
		}

		[Test]
		public void Can_open_data_base()
		{
			 _dataBase.Open(DbName);

			var isOpen = _dataBase.IsOpen;

			Assert.That(isOpen, Is.True);
		}

		[Test]
		public void Can_sync_database()
		{
			_dataBase.Open(DbName);

			_dataBase.Synchronize();
		}


		[Test]
		public void Database_metadata_returns_info_about_collections()
		{
			_dataBase.Open(DbName);

			_dataBase.CreateCollection("TheFirst", new CollectionOptions());

			var metaData = _dataBase.DatabaseMetadata;

			var metaDataAsString = metaData.ToString();

			StringAssert.Contains("TheFirst", metaDataAsString);
		}

		[Test]
		public void Can_create_collection_data_base()
		{
			_dataBase.Open(DbName);

			var collection = _dataBase.CreateCollection("TheFirst", new CollectionOptions());

			collection.Drop();

			var metaData = _dataBase.DatabaseMetadata;

			var metaDataAsString = metaData.ToString();

			StringAssert.DoesNotContain("TheFirst", metaDataAsString);
		}

		[Test]
		public void Can_get_collection()
		{
			_dataBase.Open(DbName);

			_dataBase.CreateCollection("TheFirst", new CollectionOptions());

			Assert.DoesNotThrow(() => _dataBase.GetCollection("TheFirst"));
		}

		[Test]
		public void Get_collection_throws_if_not_exists()
		{
			_dataBase.Open(DbName);

			Assert.Throws<EJDBException>(() =>  _dataBase.GetCollection("TheFirst"));
		}

		[Test]
		public void Can_begin_and_commit_transaction()
		{
			_dataBase.Open(DbName);

			var collection = _dataBase.CreateCollection("Test", new CollectionOptions());

			collection.BeginTransaction();

			var isActive = collection.TransactionActive;

			collection.CommitTransaction();

			var notActiveTransaction = !collection.TransactionActive;

			Assert.That(isActive, Is.True, "Transaction should be active after begin");
			Assert.That(notActiveTransaction, Is.True, "Transaction should be active after commit");
		}

		[Test]
		public void Can_begin_and_rollback_transaction()
		{
			_dataBase.Open(DbName);

			var collection = _dataBase.CreateCollection("Test", new CollectionOptions());

			collection.BeginTransaction();

			var isActive = collection.TransactionActive;

			collection.RollbackTransaction();

			var notActiveTransaction = !collection.TransactionActive;

			Assert.That(isActive, Is.True, "Transaction should be active after begin");
			Assert.That(notActiveTransaction, Is.True, "Transaction should be active after commit");
		}

		[Test]
		public void Can_synchronize_collection()
		{
			_dataBase.Open(DbName);

			var collection = _dataBase.CreateCollection("Test", new CollectionOptions());

			collection.Synchronize();
		}

		[Test]
		public void Can_save_and_load_document()
		{
			_dataBase.Open(DbName);

			var collection = _dataBase.CreateCollection("Test", new CollectionOptions());

			var origin = BSONDocument.ValueOf(new
			{
				name = "Grenny",
				type = "African Grey",
				male = true,
				age = 1,
				birthdate = DateTime.Now,
				likes = new[] { "green color", "night", "toys" },
				extra = BSONull.VALUE
			});

			collection.Save(origin, false);

			var id = origin.GetBSONValue("_id");

			var reloaded = collection.Load((BSONOid)id.Value);
			//TODO: made more string assertion
			Assert.That(reloaded, Is.Not.Null);
		}
	}
}