using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Match arrays of simple values by a value or array of objects by specified criterion on for fields. 
    /// Fields for object checked using logical AND.
    /// </summary>
    public class MatchElementCriterion : ICriterion
    {
        private readonly ICriterion _criterion;

        /// <summary>
        /// Creates new instance of <see cref="MatchElementCriterion"/>
        /// </summary>
        /// <param name="criterion"></param>
        public MatchElementCriterion(ICriterion criterion)
        {
            _criterion = criterion;
        }

        /// <summary>
        /// Writes criterion to specified writer.
        /// </summary>
        public void WriteTo(JsonWriter writer)
        {
            //$elemMatch The $elemMatch operator matches more than one component within an array element.
            //{ array:    { $elemMatch:  { value1 : 1, value2 : { $gt: 1 } } } }

            writer.WriteStartObject();
            writer.WritePropertyName("$elemMatch");

            _criterion.WriteTo(writer);

            writer.WriteEndObject();
        }
    }
}