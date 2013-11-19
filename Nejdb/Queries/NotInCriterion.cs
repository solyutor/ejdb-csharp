using Newtonsoft.Json;

namespace Nejdb.Queries
{
    public class NotInCriterion<TValue> : ICriterion
    {
        private readonly TValue[] _values;

        public NotInCriterion(params TValue[] values)
        {
            _values = values;
        }

        public void WriteTo(JsonWriter writer)
        {
            //$nin - Not IN String OR Number OR Array val matches to value in specified array:
            //{'fpath' : {'$nin' : [val1, val2, val3]}}
            writer.WriteArrayBaseCritertion("$nin", _values);
        }
    }
}