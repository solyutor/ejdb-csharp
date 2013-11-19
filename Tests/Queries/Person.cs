using Nejdb.Bson;

namespace Ejdb.Tests
{
    public class Person
    {
        public static Person Putin()
        {
            return new Person
            {
                Name = new Name{First = "Vladimir", Surname = "Putin"},
                Age = 61,
                Hobbies = new[] { "Power", "Corruption", "Narcissism", "Patriotic speech" }
            };

        }

        public static Person Navalny()
        {
            return new Person
            {
                Name = new Name{First =  "Alexey", Surname = "Navalny"},
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