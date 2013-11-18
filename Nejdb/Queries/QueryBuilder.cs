using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
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

* - $gt, $gte (>, >=) and $lt, $lte for number types:
* - {'fpath' : {'$gt' : number}, ...}
* - $bt Between for number types:
* - {'fpath' : {'$bt' : [num1, num2]}}
* - $in String OR Number OR Array val matches to value in specified array:
* - {'fpath' : {'$in' : [val1, val2, val3]}}
* - $nin - Not IN
* - $strand String tokens OR String array val matches all tokens in specified array:
* - {'fpath' : {'$strand' : [val1, val2, val3]}}
* - $stror String tokens OR String array val matches any token in specified array:
* - {'fpath' : {'$stror' : [val1, val2, val3]}}
* - $exists Field existence matching:
* - {'fpath' : {'$exists' : true|false}}
* - $icase Case insensitive string matching:
* - {'fpath' : {'$icase' : 'val1'}} //icase matching
* Ignore case matching with '$in' operation:
* - {'name' : {'$icase' : {'$in' : ['théâtre - театр', 'hello world']}}}
* For case insensitive matching you can create special index of type: `JBIDXISTR`
* - $elemMatch The $elemMatch operator matches more than one component within an array element.
* - { array: { $elemMatch: { value1 : 1, value2 : { $gt: 1 } } } }
* Restriction: only one $elemMatch allowed in context of one array field.
* - $and, $or joining:
* - {..., $and : [subq1, subq2, ...] }
* - {..., $or : [subq1, subq2, ...] }
* Example: {z : 33, $and : [ {$or : [{a : 1}, {b : 2}]}, {$or : [{c : 5}, {d : 7}]} ] }
*
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
    public class QueryBuilder<TDocument>
    {
        private readonly Dictionary<string, ICriterion> _criterions;

        /// <summary>
        /// Creates new instance of <see cref="QueryBuilder{TDocument}"/>
        /// </summary>
        public QueryBuilder()
        {
            _criterions = new Dictionary<string, ICriterion>();
        }

        /// <summary>
        /// Converts <see cref="QueryBuilder{TDocument}"/> to Bson represention of EJDB query 
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
        /// Adds new criterion 
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <param name="criterion"></param>
        /// <returns></returns>
        public QueryBuilder<TDocument> Where<TProperty>(Expression<Func<TDocument, TProperty>> property, ICriterion criterion)
        {
            var memberPath = BuildMemberPath(property);
            _criterions.Add(memberPath, criterion);
            return this;
        }

        private string BuildMemberPath<TProperty>(Expression<Func<TDocument, TProperty>> property)
        {
            var expression = property.Body;

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                switch (unaryExpression.NodeType)
                {
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        expression = unaryExpression.Operand;
                        break;
                }

            }
            var me = expression as MemberExpression;

            if (me == null)
                throw new InvalidOperationException("No idea how to convert " + property.Body.NodeType + ", " + property.Body + " to a member expression");

            var parts = new List<string>();
            while (me != null)
            {
                parts.Insert(0, me.Member.Name);
                me = me.Expression as MemberExpression;
            }
            return String.Join(".", parts.ToArray());
        }

        private void WriteTo(JsonWriter writer)
        {
            writer.WriteStartObject();

            foreach (var criterion in _criterions)
            {
                writer.WritePropertyName(criterion.Key);
                criterion.Value.WriteTo(writer);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Clears all added critrions.
        /// </summary>
        public void Clear()
        {
            _criterions.Clear();
        }
    }
}