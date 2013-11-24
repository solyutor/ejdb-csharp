using System;
using Nejdb.Tests.Queries;
using NUnit.Framework;

namespace Nejdb.Tests
{
    [TestFixture]
    public class Tutorial
    {
        [Test]
        public void Experiment_goes_here()
        {
            var originalHamingway = new Author
                         {
                             Name = new Name { Firstname = "Ernest", Surname = "Hemingway" },
                             BirthDay = new DateTime(1899, 7, 21),
                             Hobbies = new[] { "Fishing", "Hunting" },
                             MainWorks = new[]
                                         {
                                             new BookHeader("The Torrents of Spring", 1926),
                                             new BookHeader("The Sun Also Rises", 1926),
                                             new BookHeader("A Farewell to Arms", 1929),
                                             new BookHeader("To Have and Have Not", 1937),
                                             new BookHeader("For Whom the Bell Tolls", 1940),
                                             new BookHeader("Across the River and into the Trees", 1950),
                                             new BookHeader("The Old Man and the Sea", 1952)
                                         }
                         };

            using (var database = Library.Instance.CreateDatabase())
            {
                database.Open("Biblio.db");
                using (var collection = database.CreateCollection("Authors", CollectionOptions.None))
                {
                    //Note, while document not saved id property is empty: 000000000000000000000000
                    Console.WriteLine(originalHamingway.Id);

                    var id = collection.Save<Author>(originalHamingway, false);
                    
                    //When document is save Nejdb automatically assigns id value: 5292021a4d57000000000000 
                    // It happens for documents with property named 'Id' of type ObjectId.  
                    Console.WriteLine(originalHamingway.Id);

                    var reloadedHamingway = collection.Load<Author>(id);
                }
            }
        }
    }
}