using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Encapsulates logic to generate json/bson representation of criterions.
    /// </summary>
    public interface ICriterion
    {
        /// <summary>
        /// Writes criterion to specified writer.
        /// </summary>
        void WriteTo(JsonWriter writer);
    }
}