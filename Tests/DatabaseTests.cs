using System.IO;
using Ejdb.DB;
using NUnit.Framework;

namespace Ejdb.Tests
{
	[TestFixture]
	public class DatabaseTests
	{
		private const string DbName = "test.db";

		[SetUp]
		public void Setup()
		{
			if (File.Exists(DbName))
			{
				File.Delete(DbName);
			}
		}

		[Test]
		public void Can_open_data_base()
		{
			var library = Library.Create();

			var dataBase = library.CreateDatabase();

			 dataBase.Open(DbName);

			var isOpen = dataBase.IsOpen;

			//dataBase.Close();
			dataBase.Dispose();

			library.Dispose();

			Assert.That(isOpen, Is.True);
		}
	}
}