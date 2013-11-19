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
        public void Starts_with_criterion_query()
        {
            var startsWith = Criterions.StartsWith("Ale");
            var builder = new QueryBuilder<Person>().Where(x => x.Name.First, startsWith);
            
            AssertFoundNavalny(builder);
        }
    }
}