using System;
using Nejdb;
using Nejdb.Queries;
using NUnit.Framework;

namespace Ejdb.Tests
{
    [TestFixture]
    public class NumberComparisonCriterionTests : CriterionTests
    {
        [Test]
        public void Greater_than_query()
        {
            var criterion = Criterions.GreaterThan(40);

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

        [Test]
        public void Greater_than_or_equal_query()
        {
            var criterion = Criterions.GreaterThanOrEqual(61);

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

        [Test]
        public void Lower_than_query()
        {
            var criterion = Criterions.LowerThan(40);

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
        public void Lower_than_or_equal_query()
        {
            var criterion = Criterions.LowerThanOrEqual(36);

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
        public void Between_query()
        {
            var criterion = Criterions.Between(30, 40);

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
    }
}