﻿using System;
using System.IO;
using Nejdb;
using Nejdb.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using NUnit.Framework;

namespace Ejdb.Tests
{
    [TestFixture]
    public class QueryBuilderTests : CriterionTests
    {
        [Test]
        public void Match_criterion_query()
        {
            var criterion = Criterions.Equals("Alexey");

            var builder = new QueryBuilder(Criterions.Field<Person, string>(x => x.Name.First, criterion));

            AssertFoundNavalny(builder);
        }

        [Test]
        public void Negate_criterion_query()
        {
            var criterion = Criterions.Not(Criterions.Equals("Alexey"));
            var builder = new QueryBuilder(Criterions.Field<Person, string>(x => x.Name.First, criterion));

            AssertFoundPutin(builder);
        }

        [Test]
        public void Exists_criterion_query()
        {
            var criterion = Criterions.FieldExists();

            var builder = new QueryBuilder(Criterions.Field<Person, string>(x => x.Name.First, criterion));

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

            var builder = new QueryBuilder(Criterions.Field<Person, string>(x => x.Name.First, criterion));

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
            var byName = Criterions.Field<Person, string>(x => x.Name.First, Criterions.Equals("Alexey"));
            var byAge = Criterions.Field<Person, int>(x => x.Age, Criterions.Equals(36));

            var andCritertion = Criterions.And(byName, byAge);

            var builder = new QueryBuilder(andCritertion);

            AssertFoundNavalny(builder);
        }

        [Test]
        public void Or_criterion_query()
        {
            var byName = Criterions.Field<Person, string>(x => x.Name.First, Criterions.Equals("Alexey"));
            var byAge = Criterions.Field<Person, int>(x => x.Age, Criterions.Equals(61));

            var andCritertion = Criterions.Or(byName, byAge);

            var builder = new QueryBuilder(andCritertion);

            AssertFoundBoth(builder);
        }
    }
}