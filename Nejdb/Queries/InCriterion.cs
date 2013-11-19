using Newtonsoft.Json;

namespace Nejdb.Queries
{
    public class InCriterion<TValue> : ICriterion
    {
        private readonly TValue[] _values;

        public InCriterion(params TValue[] values)
        {
            _values = values;
        }

        public void WriteTo(JsonWriter writer)
        {
            //$in String OR Number OR Array val matches to value in specified array:
            //{'fpath' : {'$in' : [val1, val2, val3]}}

            writer.WriteArrayBaseCritertion("$in", _values);
        }
    }
}