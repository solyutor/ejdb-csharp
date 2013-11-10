using System;
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
		} 
	}
}