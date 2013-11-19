using System;
using Nejdb;
using Nejdb.Queries;
using NUnit.Framework;

namespace Ejdb.Tests
{
    [TestFixture]
    public class QueryBuilderTests : CriterionTests
    {
        [Test]
        public void Match_criterion_query()
        {
            var builder = new QueryBuilder<Person>().Where(x => x.Name.First, Criterions.Equals("Alexey"));

            AssertFoundNavalny(builder);
        }

        [Test]
        public void Negate_criterion_query()
        {
            var criterion = Criterions.Not(Criterions.Equals("Alexey"));
            var builder = new QueryBuilder<Person>().Where(x => x.Name.First, criterion);

            AssertFoundPutin(builder);
        }

        [Test]
        public void Exists_criterion_query()
        {
            var criterion = Criterions.FieldExists();

            var builder = new QueryBuilder<Person>().Where(x => x.Name.First, criterion);

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

            var builder = new QueryBuilder<Person>().Where(x => x.Name.First, criterion);

            using (var query = Collection.CreateQuery<Person>(builder))
            using (var cursor = query.Execute(QueryMode.Explain))
            {
                Console.WriteLine(cursor.GetLog());

                Assert.That(cursor.Count, Is.EqualTo(0));
            }
        }
    }
}