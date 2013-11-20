using Newtonsoft.Json;

namespace Nejdb.Queries
{
    public class AndCriterion : ICriterion
    {
        private readonly ICriterion[] _criterions;

        public AndCriterion(ICriterion[] criterions)
        {
            _criterions = criterions;
        }

        public void WriteTo(JsonWriter writer)
        {
            //Example: {z : 33, $and : [ {$or : [{a : 1}, {b : 2}]}, {$or : [{c : 5}, {d : 7}]} ] }
            //{..., $and : [subq1, subq2, ...] }
            writer.WriteCriterionArray("$and", _criterions);
        }
    }
}