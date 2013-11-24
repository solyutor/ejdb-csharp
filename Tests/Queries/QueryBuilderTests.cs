using System;
using System.IO;
using Nejdb.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using NUnit.Framework;

namespace Nejdb.Tests.Queries
{
    [TestFixture]
    public class QueryBuilderTests : CriterionTests
    {
        [Test]
        public void Match_criterion_query()
        {
            var criterion = Criterions.Equals("Alexey");

            var builder = new QueryBuilder(Criterions.Field<Person, string>(x => x.Name.Firstname, criterion));

            AssertFoundNavalny(builder);
        }

        [Test]
        public void Negate_criterion_query()
        {
            var criterion = Criterions.Not(Criterions.Equals("Alexey"));
            var builder = new QueryBuilder(Criterions.Field<Person, string>(x => x.Name.Firstname, criterion));

            AssertFoundPutin(builder);
        }

        [Test]
        public void Exists_criterion_query()
        {
            var criterion = Criterions.FieldExists();

            var builder = new QueryBuilder(Criterions.Field<Person, string>(x => x.Name.Firstname, criterion));

            using (var query = Collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                Assert.That(cursor.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void Not_exists_criterion_query()
        {
            var criterion = Criterions.FieldNotExists();

            var builder = new QueryBuilder(Criterions.Field<Person, string>(x => x.Name.Firstname, criterion));

            using (var query = Collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                Assert.That(cursor.Count, Is.EqualTo(0));
            }
        }
        [Test]
        public void And_criterion_query()
        {
            var byName = Criterions.Field<Person, string>(x => x.Name.Firstname, Criterions.Equals("Alexey"));
            var byAge = Criterions.Field<Person, int>(x => x.Age, Criterions.Equals(36));

            var andCritertion = Criterions.And(byName, byAge);

            var builder = new QueryBuilder(andCritertion);

            AssertFoundNavalny(builder);
        }


        [Test]
        public void Or_criterion_query()
        {
            var byName = Criterions.Field<Person, string>(x => x.Name.Firstname, Criterions.Equals("Alexey"));
            var byAge = Criterions.Field<Person, int>(x => x.Age, Criterions.Equals(61));

            var criterion = Criterions.Or(byName, byAge);

            var builder = new QueryBuilder(criterion);
            AssertFoundBoth(builder);
        }

        [Test]
        public void Complex_criterion_combination_query()
        {
            var byName = Criterions.Field<Person, string>(x => x.Name.Firstname, Criterions.Equals("Alexey"));
            var byAge = Criterions.Field<Person, int>(x => x.Age, Criterions.Equals(36));

            var andCriterion = Criterions.And(byName, byAge);

            var ignoreCaseHobby = Criterions.IgnoreCase(Criterions.Any("corruption"));
            var byHobbies = Criterions.Field<Person, string[]>(x => x.Hobbies, ignoreCaseHobby);
            var criterion = Criterions.Or(andCriterion, byHobbies);

            var builder = new QueryBuilder(criterion);

            AssertFoundBoth(builder);
        }

        [Test]
        public void Element_match_criterion_simple_array_query()
        {
            var criterion = Criterions.Field<Person, string[]>(
                x=> x.Hobbies, 
                Criterions.MatchElement(
                    Criterions.Equals("Power")));

            var builder = new QueryBuilder(criterion);

            AssertFoundPutin(builder);
        }

        [Test]
        public void Element_match_criterion_complex_property_query()
        {
            var criterion = Criterions.Field<Person, Name>(
                x => x.Name,
                Criterions.MatchElement(
                    Criterions.Object(
                        Criterions.Field<Name, string>(x => x.Firstname, Criterions.Equals("Vladimir")),
                        Criterions.Field<Name, string>(x => x.Surname, Criterions.Equals("Putin")))
                    ));

            var builder = new QueryBuilder(criterion);

            AssertFoundPutin(builder);
        }
    }


}