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
            var criterion = Criterions.Field<Person, int>(x => x.Age, Criterions.In(36, 52));

            var builder = new QueryBuilder(criterion);

            AssertFoundNavalny(builder);
        }

        [Test]
        public void Not_in_criterion_query()
        {
            var criterion = Criterions.Field<Person, int>(x => x.Age, Criterions.NotIn(36, 52));

            var builder = new QueryBuilder(criterion);

            AssertFoundPutin(builder);
        }
    }
}