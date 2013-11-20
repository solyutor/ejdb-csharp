using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Nejdb.Queries
{
    /**
* Create query object.
* Sucessfully created queries must be destroyed with ejdbquerydel().
*
* EJDB queries inspired by MongoDB (mongodb.org) and follows same philosophy.
*
* - Supported queries:


* - $elemMatch The $elemMatch operator matches more than one component within an array element.
* - { array: { $elemMatch: { value1 : 1, value2 : { $gt: 1 } } } }
* Restriction: only one $elemMatch allowed in context of one array field.

* - Mongodb $(projection) operator supported. (http://docs.mongodb.org/manual/reference/projection/positional/#proj._S_)
* - Mongodb positional $ update operator supported. (http://docs.mongodb.org/manual/reference/operator/positional/)
*
* - Queries can be used to update records:
*
* $set Field set operation.
* - {.., '$set' : {'fpath1' : val1, 'fpathN' : valN}}
* $upsert Atomic upsert. If matching records are found it will be '$set' operation,
* otherwise new record will be inserted
* with fields specified by argment object.
* - {.., '$upsert' : {'fpath1' : val1, 'fpathN' : valN}}
* $inc Increment operation. Only number types are supported.
* - {.., '$inc' : {'fpath1' : number, ..., 'fpath2' : number}
* $dropall In-place record removal operation.
* - {.., '$dropall' : true}
* $addToSet Atomically adds value to the array only if its not in the array already.
* If containing array is missing it will be created.
* - {.., '$addToSet' : {'fpath' : val1, 'fpathN' : valN, ...}}
* $addToSetAll Batch version if $addToSet
* - {.., '$addToSetAll' : {'fpath' : [array of values to add], ...}}
* $pull Atomically removes all occurrences of value from field, if field is an array.
* - {.., '$pull' : {'fpath' : val1, 'fpathN' : valN, ...}}
* $pullAll Batch version of $pull
* - {.., '$pullAll' : {'fpath' : [array of values to remove], ...}}
*
* - Collection joins supported in the following form:
*
* {..., $do : {fpath : {$join : 'collectionname'}} }
* Where 'fpath' value points to object's OIDs from 'collectionname'. Its value
* can be OID, string representation of OID or array of this pointers.
*
* NOTE: It is better to execute update queries with `JBQRYCOUNT`
* control flag to avoid unnecessarily data fetching.
*
* NOTE: Negate operations: $not and $nin not using indexes
* so they can be slow in comparison to other matching operations.
*
* NOTE: Only one index can be used in search query operation.
*
* QUERY HINTS (specified by `hints` argument):
* - $max Maximum number in the result set
* - $skip Number of skipped results in the result set
* - $orderby Sorting order of query fields.
* - $fields Set subset of fetched fields
* If a field presented in $orderby clause it will be forced to include in resulting records.
* Example:
* hints: {
* "$orderby" : { //ORDER BY field1 ASC, field2 DESC
* "field1" : 1,
* "field2" : -1
* },
* "$fields" : { //SELECT ONLY {_id, field1, field2}
* "field1" : 1,
* "field2" : 1
* }
* }
*
* Many query examples can be found in `testejdb/t2.c` test case.
*
* @param jb EJDB database handle.
* @param qobj Main BSON query object.
* @param orqobjs Array of additional OR query objects (joined with OR predicate).
* @param orqobjsnum Number of OR query objects.
* @param hints BSON object with query hints.
* @return On success return query handle. On error returns NULL.
*/

    ///<summary>
    /// Encapsulates logic to crate queryies
    /// </summary>
    public class QueryBuilder
    {
        private readonly ICriterion _criterion;

        /// <summary>
        /// Creates new instance of <see cref="QueryBuilder"/>
        /// </summary>
        public QueryBuilder(ICriterion criterion)
        {
            _criterion = criterion;
        }

        /// <summary>
        /// Converts <see cref="QueryBuilder"/> to Bson represention of EJDB query 
        /// </summary>
        public byte[] ToBsonBytes()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BsonWriter(stream))
            {
                WriteTo(writer);
                var length = stream.Position + 1;
                var result = new byte[length];
                Array.Copy(stream.GetBuffer(), result, length);
                return result;
            }
        }

        /// <summary>
        /// Writes EJDB query to specified <see cref="JsonWriter"/>
        /// </summary>
        public void WriteTo(JsonWriter writer)
        {
            writer.WriteStartObject();

            _criterion.WriteTo(writer);

            writer.WriteEndObject();
        }
    }
}