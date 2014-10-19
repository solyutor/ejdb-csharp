using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Represents logical OR operation between any number of inner criterions
    /// </summary>
    public class OrCriterion : ICriterion
    {
        private readonly ICriterion[] _criterions;

        /// <summary>
        /// Creates new instance of <see cref="AndCriterion"/>
        /// </summary>
        public OrCriterion(ICriterion[] criterions)
        {
            _criterions = criterions;
        }

        /// <summary>
        /// Writes criterion to specified writer.
        /// </summary>
        public void WriteTo(JsonWriter writer)
        {
            //Example: {z : 33, $and : [ {$or : [{a : 1}, {b : 2}]}, {$or : [{c : 5}, {d : 7}]} ] }
            //{..., $or : [subq1, subq2, ...] }
            writer.WriteCriterionArray("$or", _criterions);
        }
    }
}