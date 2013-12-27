using System;
using Nejdb.Bson;
using Nejdb.Tests.Queries;

namespace Nejdb.Tests.Tutorial
{
    public class Author
    {
        public ObjectId Id { get; private set; }

        public Name Name { get; set; }

        public DateTime BirthDay { get; set; }

        public BookHeader[] MainWorks { get; set; }

        public string[] Hobbies { get; set; }
    }
}