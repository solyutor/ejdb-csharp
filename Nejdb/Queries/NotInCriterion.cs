using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Checks that a value of a field is not equals to any of supplied values;
    /// </summary>
    public class NotInCriterion<TValue> : ICriterion
    {
        private readonly TValue[] _values;
        /// <summary>
        /// Creates new instance of <see cref="NotInCriterion{TValue}"/>
        /// </summary>
        /// <param name="values">Values to check</param>
        public NotInCriterion(params TValue[] values)
        {
            _values = values;
        }

        /// <summary>
        /// Writes criterion to specified writer.
        /// </summary>
        public void WriteTo(JsonWriter writer)
        {
            //$nin - Not IN String OR Number OR Array val matches to value in specified array:
            //{'fpath' : {'$nin' : [val1, val2, val3]}}
            writer.WriteArrayBaseCritertion("$nin", _values);
        }
    }
}