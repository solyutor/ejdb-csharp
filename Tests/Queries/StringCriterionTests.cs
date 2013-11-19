using System;
using System.IO;
using Nejdb.Queries;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using NUnit.Framework;

namespace Ejdb.Tests
{
    [TestFixture]
    public class StringCriterionTests : CriterionTests
    {
        [Test]
        public void String_all_query()
        {
            var criterion = Criterions.All("Patriotic", "speech");

            var builder = new QueryBuilder(Criterions.Field<Person, string[]>(x => x.Hobbies, criterion));

            AssertFoundPutin(builder);
        }

        [Test]
        public void String_any_query()
        {
            var criterion = Criterions.Any("Patriotic");

            var builder = new QueryBuilder(Criterions.Field<Person, string[]>(x => x.Hobbies, criterion));

            AssertFoundPutin(builder);
        }

        [Test]
        public void String_ignore_case_query()
        {
            var criterion = Criterions.IgnoreCase(Criterions.Any("PUTIN"));

            var builder = new QueryBuilder(Criterions.Field<Person, string>(x => x.Name.Surname, criterion));

            AssertFoundPutin(builder);
        }

        [Test]
        public void String_any_with_ignore_case_query()
        {
            var criterion = Criterions.IgnoreCase(Criterions.Any("patriotic"));

            var builder = new QueryBuilder(Criterions.Field<Person, string[]>(x => x.Hobbies, criterion));

            AssertFoundPutin(builder);
        }

        [Test]
        public void Starts_with_criterion_query()
        {
            var criterion = Criterions.StartsWith("Ale");
            var builder = new QueryBuilder(Criterions.Field<Person, string>(x => x.Name.First, criterion));

            AssertFoundNavalny(builder);
        }
    }
}