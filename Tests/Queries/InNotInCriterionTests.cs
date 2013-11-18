using System;
using Nejdb;
using Nejdb.Queries;
using NUnit.Framework;

namespace Ejdb.Tests
{
    [TestFixture]
    public class InNotInCriterionTests : CriterionTests
    {
        [Test]
        public void In_criterion_query()
        {
            var criterion = Criterions.In(36,52);

            var builder = new QueryBuilder<Person>().Where(x => x.Age, criterion);

            using (var query = Collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                var person = cursor[0];

                Assert.That(cursor.Count, Is.EqualTo(1));
                Assert.That(person.Name.Surname, Is.EqualTo(Person.Navalny().Name.Surname));
            }
        }

        [Test]
        public void Not_in_criterion_query()
        {
            var criterion = Criterions.NotIn(36, 52);

            var builder = new QueryBuilder<Person>().Where(x => x.Age, criterion);

            using (var query = Collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                var person = cursor[0];

                Assert.That(cursor.Count, Is.EqualTo(1));
                Assert.That(person.Name.Surname, Is.EqualTo(Person.Putin().Name.Surname));
            }
        }
    }
}