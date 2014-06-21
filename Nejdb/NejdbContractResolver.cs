using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nejdb.Bson;
using Nejdb.Internals;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nejdb
{
    /// <summary>
    /// Default contract resolver for Nejdb <see cref="Collection"/> and <see cref="Cursor"/>
    /// </summary>
    public class NejdbContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Default name for Id property of event bson document
        /// </summary>
        public const string IdPropertyName = "_id";

        /// <summary>
        /// Default instance of <see cref="NejdbContractResolver"/>
        /// </summary>
        public static readonly NejdbContractResolver Instance = new NejdbContractResolver();

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> orignalProperties = base.CreateProperties(type, memberSerialization);

            var jsonProperties = new List<JsonProperty>(orignalProperties.Count);
            jsonProperties.AddRange(orignalProperties);

            var idProperty = jsonProperties.SingleOrDefault(
                x => x.PropertyType == typeof(ObjectId) &&
                     (x.PropertyName.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                      (x.PropertyName.Equals("_Id", StringComparison.OrdinalIgnoreCase))));


            if (idProperty != null)
            {
                var originalName = idProperty.PropertyName;
                idProperty.PropertyName = IdPropertyName;
                idProperty.ShouldSerialize = value => !TypeExtension.IdIsEmpty(value, originalName);

                jsonProperties.Sort(IdMustBeFirst);
            }

            return jsonProperties;
        }

        /// <summary>
        /// Sorts json properties in the way that Id property will be the first property in a bson document
        /// </summary>
        protected int IdMustBeFirst(JsonProperty x, JsonProperty y)
        {
            if (x.PropertyName == IdPropertyName)
            {
                return int.MinValue;
            }

            if (y.PropertyName == IdPropertyName)
            {
                return int.MaxValue;
            }

            return Comparer<string>.Default.Compare(x.PropertyName, y.PropertyName);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);

            if (!jsonProperty.Readable || jsonProperty.Writable)
            {
                return jsonProperty;
            }

            var property = member as PropertyInfo;
            if (property == null) return jsonProperty;
            var hasPrivateSetter = property.GetSetMethod(true) != null;
            jsonProperty.Writable = hasPrivateSetter;
            return jsonProperty;
        }
    }
}