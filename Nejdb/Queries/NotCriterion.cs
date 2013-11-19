using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Negates another criterion
    /// </summary>
    public class NotCriterion : ICriterion
    {
        private readonly ICriterion _criterion;

        /// <summary>
        /// Creates new instance of <see cref="NotCriterion"/>
        /// </summary>
        /// <param name="criterion">A criterion to negate</param>
        public NotCriterion(ICriterion criterion)
        {
            _criterion = criterion;
        }

        /// <summary>
        /// Writes criterion to specifed <seealso cref="JsonWriter"/>
        /// </summary>
        /// <param name="writer"></param>
        public void WriteTo(JsonWriter writer)
        {
            //*$not Negate operation.
            // {'fpath' : {'$not' : val}} //Field not equal to val
            writer.WriteObjectBasedCriterion("$not", _criterion);
        }
    }
}