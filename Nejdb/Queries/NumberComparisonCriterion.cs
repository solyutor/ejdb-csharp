using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Implemenets criterion to compare numeric values.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class NumberComparisonCriterion<TValue> : ICriterion
    {
        private readonly TValue _value;
        private readonly Comparsion _comparsion;

        /// <summary>
        /// Creates new instance of <see cref="NumberComparisonCriterion{TValue}"/> with specified comparison mode
        /// </summary>
        /// <param name="value">Any numeric value</param>
        /// <param name="comparsion">Comparison type for value</param>
        public NumberComparisonCriterion(TValue value, Comparsion comparsion)
        {
            _value = value;
            _comparsion = comparsion;
        }

        /// <summary>
        /// Writes criterion to specifed <seealso cref="JsonWriter"/>
        /// </summary>
        /// <param name="writer"></param>
        public void WriteTo(JsonWriter writer)
        {
            // $gt, $gte (>, >=) and $lt, $lte for number types:
            // {'fpath' : {'$gt' : number}, ...}
            writer.WriteStartObject();

            switch (_comparsion)
            {
                case Comparsion.Greater:
                    writer.WritePropertyName("$gt");
                    break;
                case Comparsion.GreaterOrEqual:
                    writer.WritePropertyName("$gte");
                    break;
                case Comparsion.Lower:
                    writer.WritePropertyName("$lt");
                    break;
                case Comparsion.LowerOrEqual:
                    writer.WritePropertyName("$lte");
                    break;
            }

            JsonValueWriter<TValue>.Write(writer, _value);
            writer.WriteEndObject();
        }
    }
}