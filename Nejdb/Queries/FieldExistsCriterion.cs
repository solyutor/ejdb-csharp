using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Match presence of absence of any field
    /// </summary>
    public class FieldExistsCriterion : ICriterion
    {
        private readonly bool _shouldExists;

        /// <summary>
        /// Creates new instance of <see cref="FieldExistsCriterion"/>
        /// </summary>
        /// <param name="shouldExists">If true matches presence of field, matches absense otherwise</param>
        public FieldExistsCriterion(bool shouldExists)
        {
            _shouldExists = shouldExists;
        }

        public void WriteTo(JsonWriter writer)
        {
            //* - $exists Field existence matching:
            //* - {'fpath' : {'$exists' : true|false}}
            writer.WriteStartObject();
            writer.WritePropertyName("$exists");
            writer.WriteValue(_shouldExists);
            writer.WriteEndObject();
        }
    }
}