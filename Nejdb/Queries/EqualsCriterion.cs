using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Matches simple value
    /// </summary>
    /// <typeparam name="TValue">Type of value to match</typeparam>
    public class EqualsCriterion<TValue> : ICriterion
    {
        private readonly TValue _value;

        /// <summary>
        /// Creates new instance of <see cref="EqualsCriterion{TValue}"/>
        /// </summary>
        /// <param name="value">A value to match</param>
        public EqualsCriterion(TValue value)
        {
            _value = value;
        }

        /// <summary>
        /// Writes criteria to specifed <seealso cref="JsonWriter"/>
        /// </summary>
        /// <param name="writer"></param>
        public void WriteTo(JsonWriter writer)
        {
            //Simple matching of String OR Number OR Array value:
            //{'fpath' : 'val', ...}
            JsonValueWriter<TValue>.Write(writer, _value);
        }
    }
}