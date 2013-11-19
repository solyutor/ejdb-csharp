using Newtonsoft.Json;

namespace Nejdb.Queries
{
    /// <summary>
    /// Matches that field or array at lease one array element contains ALL or ANY of specified tokens
    /// </summary>
    public class StringAndOrAnyCriterion : ICriterion
    {
        /// <summary>
        /// Enumerates matching methods of 
        /// </summary>
        public enum Match
        {
            /// <summary>
            ///  Matches that field or array at lease one array element contains ALL specified tokens
            /// </summary>
            All,
            /// <summary>
            ///  Matches that field or array at lease one array element contains ANY of specified tokens
            /// </summary>
            Any
        }

        private readonly string[] _tokens;
        private readonly Match _match;

        /// <summary>
        /// Creates new instance of <see cref="StringAndOrAnyCriterion"/>
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="match"></param>
        public StringAndOrAnyCriterion(string[] tokens, Match match)
        {
            _tokens = tokens;
            _match = match;
        }

        /// <summary>
        /// Writes criterion to specified writer.
        /// </summary>
        public void WriteTo(JsonWriter writer)
        {
            // $strand String tokens OR String array val matches all tokens in specified array:
            //{'fpath' : {'$strand' : [val1, val2, val3]}}
            //$stror String tokens OR String array val matches any token in specified array:
            //{'fpath' : {'$stror' : [val1, val2, val3]}}
            var token = _match == Match.All ? "$strand" : "$stror";
            writer.WriteArrayBaseCritertion(token, _tokens);
        }
    }
}