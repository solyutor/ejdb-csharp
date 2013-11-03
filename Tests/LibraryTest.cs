using Ejdb.Utils;
using NUnit.Framework;

namespace Ejdb.Tests
{
	[TestFixture]
	public class LibraryTest
	{
		[Test]
		public void Can_create_library()
		{
			var library = Library.Load();

			library.Dispose();
		}
	}
}