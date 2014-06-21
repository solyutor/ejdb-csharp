using Nejdb.Bson;

namespace Nejdb.Tests
{
    public class Sample
    {
        public Sample()
        {
            
        }
        public Sample(string privateName)
        {
            PrivateName = privateName;
        }
        public ObjectId Id { get; private set; }

        public string Name { get; set; }

        public string PrivateName { get; private set; }
    }


    public class PublicObjectIdSample
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
    }


    public class NonIdObjectIdSample
    {
        public ObjectId Id { get; set; }

        public ObjectId OtherId { get; set; }
    }
}