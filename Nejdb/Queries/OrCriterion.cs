using System;
using Newtonsoft.Json;

namespace Nejdb.Queries
{
    public class OrCriterion : ICriterion
    {
        private readonly ICriterion[] _criterions;

        public OrCriterion(ICriterion[] criterions)
        {
            _criterions = criterions;
        }

        public void WriteTo(JsonWriter writer)
        {
            //Example: {z : 33, $and : [ {$or : [{a : 1}, {b : 2}]}, {$or : [{c : 5}, {d : 7}]} ] }
            //{..., $or : [subq1, subq2, ...] }
            writer.WriteCriterionArray("$or", _criterions);
        }
    }
}