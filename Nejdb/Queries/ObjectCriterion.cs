using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Encloses criterions to json-object
    /// </summary>
    public class ObjectCriterion : ICriterion
    {
        private readonly ICriterion[] _criterions;
        /// <summary>
        /// Creates new instance of <see cref="ObjectCriterion"/>
        /// </summary>
        /// <param name="criterions"></param>
        public ObjectCriterion(ICriterion[] criterions)
        {
            _criterions = criterions;
        }

        /// <summary>
        /// Writes criterions to specified writer.
        /// </summary>
        public void WriteTo(JsonWriter writer)
        {
            writer.WriteStartObject();
            foreach (var criterion in _criterions)
            {
                criterion.WriteTo(writer);
            }
            writer.WriteEndObject();
        }
    }
}