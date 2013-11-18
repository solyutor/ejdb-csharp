using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Implementes criterion that matches a beggining of string
    /// </summary>
    public class StartsWithCriterion : ICriterion
    {
        private readonly string _value;

        /// <summary>
        /// Creates new instance of <see cref="StartsWithCriterion"/>
        /// </summary>
        /// <param name="value">Prefix of the a string to search</param>
        public StartsWithCriterion(string value)
        {
            _value = value;
        }

        public void WriteTo(JsonWriter writer)
        {
            //$begin String starts with prefix
            //{'fpath' : {'$begin' : prefix}}
            writer.WriteStartObject();
            writer.WritePropertyName("$begin");
            JsonValueWriter<string>.Write(writer, _value);
            writer.WriteEndObject();
        }
    }
}