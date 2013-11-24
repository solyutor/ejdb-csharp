using Nejdb.Bson;

namespace Nejdb.Tests.Queries
{
    public class Person
    {
        public static Person Putin()
        {
            return new Person
            {
                Name = new Name{Firstname = "Vladimir", Surname = "Putin"},
                Age = 61,
                Hobbies = new[] { "Power", "Corruption", "Narcissism", "Patriotic speech" }
            };

        }

        public static Person Navalny()
        {
            return new Person
            {
                Name = new Name{Firstname =  "Alexey", Surname = "Navalny"},
                Age = 36,
                Hobbies = new[] { "Meeting", "Investigation", "Shocking" }
            };
        }
        
        public ObjectId Id { get; protected set; }

        public Name Name { get; set; }

        public int Age { get; set; }

        public string[] Hobbies { get; set; }

    }
}