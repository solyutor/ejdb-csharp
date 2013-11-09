using System;
using System.IO;
using Ejdb.Bson;
using Ejdb.DB;
using NUnit.Framework;

namespace Ejdb.Tests
{
	[TestFixture]
	public class QueryTests
	{
		private Library _library;
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
			_library = Library.Create();

			_dataBase = _library.CreateDatabase();

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
				extra = Bsonull.VALUE
			});
		}


		[Test]
		public void Can_execute_query()
		{
			_collection.Save(_origin, false);

			using (var query = _collection.CreateQuery())
			using (var cursor = query.Execute())
			{
				Assert.That(cursor.Count, Is.EqualTo(1));
			}
		}

		[TearDown]
		public void TearDown()
		{
			_dataBase.Dispose();
			_library.Dispose();
		} 
	}
}