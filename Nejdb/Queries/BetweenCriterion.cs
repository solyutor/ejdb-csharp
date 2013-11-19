using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Inclusive between for numeric values
    /// </summary>
    /// <typeparam name="TValue">Any numeric type</typeparam>
    public class BetweenCriterion<TValue> : ICriterion
    {
        private readonly TValue _lower;
        private readonly TValue _upper;
        /// <summary>
        /// Creates new instance of <see cref="BetweenCriterion{TValue}"/>
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public BetweenCriterion(TValue lower, TValue upper)
        {
            _lower = lower;
            _upper = upper;
        }

        /// <summary>
        /// Writes criterion to specified writer.
        /// </summary>
        public void WriteTo(JsonWriter writer)
        {
            //$bt Between for number types:
            //{'fpath' : {'$bt' : [num1, num2]}}
            writer.WriteArrayBaseCritertion("$bt",_lower, _upper);
        }
    }
}