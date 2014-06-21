using Nejdb.Bson;

namespace Nejdb.Tests
{
    public class Sample
    {
        public ObjectId Id { get; private set; }

        public string Name { get; set; }
    }


    public class PublicObjectIdSample
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
    }
}