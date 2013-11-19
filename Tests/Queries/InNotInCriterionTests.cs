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

            AssertFoundNavalny(builder);
        }

        [Test]
        public void Not_in_criterion_query()
        {
            var criterion = Criterions.NotIn(36, 52);

            var builder = new QueryBuilder<Person>().Where(x => x.Age, criterion);

            AssertFoundPutin(builder);
        }
    }
}