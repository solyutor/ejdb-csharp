using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Negate matches of simple value
    /// </summary>
    /// <typeparam name="TValue">Type of value to match</typeparam>
    public class NotEqualsCriterion<TValue> : ICriterion
    {
        private readonly TValue _value;

        /// <summary>
        /// Creates new instance of <see cref="NotEqualsCriterion{TValue}"/>
        /// </summary>
        /// <param name="value">A value to negate</param>\
        public NotEqualsCriterion(TValue value)
        {
            _value = value;
        }

        /// <summary>
        /// Writes criterion to specifed <seealso cref="JsonWriter"/>
        /// </summary>
        /// <param name="writer"></param>
        public void WriteTo(JsonWriter writer)
        {
            //*$not Negate operation.
            // {'fpath' : {'$not' : val}} //Field not equal to val
            writer.WriteStartObject();
            writer.WritePropertyName("$not");
            JsonValueWriter<TValue>.Write(writer, _value);
            writer.WriteEndObject();
        }
    }
}