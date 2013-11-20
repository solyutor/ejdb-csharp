using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Checks that a value of a field is equals to any of supplied values;
    /// </summary>
    public class InCriterion<TValue> : ICriterion
    {
        private readonly TValue[] _values;

        /// <summary>
        /// Creates new instance of <see cref="InCriterion{TValue}"/>
        /// </summary>
        /// <param name="values">Values to check</param>
        public InCriterion(params TValue[] values)
        {
            _values = values;
        }

        /// <summary>
        /// Writes criterion to specified writer.
        /// </summary>
        public void WriteTo(JsonWriter writer)
        {
            //$in String OR Number OR Array val matches to value in specified array:
            //{'fpath' : {'$in' : [val1, val2, val3]}}

            writer.WriteArrayBaseCritertion("$in", _values);
        }
    }
}