using System;
using System.Collections.Generic;
using System.Linq;
using Nejdb.Bson;
using Nejdb.Internals;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nejdb
{
    internal class ObjectIdContractResolver : DefaultContractResolver
    {
        public const string IdPropertyName = "_id";
        public static readonly ObjectIdContractResolver Instance = new ObjectIdContractResolver();

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> orignalProperties = base.CreateProperties(type, memberSerialization);

            List<JsonProperty> jsonProperties = new List<JsonProperty>(orignalProperties.Count);
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

        private int IdMustBeFirst(JsonProperty x, JsonProperty y)
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
    }
}