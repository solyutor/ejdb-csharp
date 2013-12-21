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
        public static readonly ObjectIdContractResolver Instance = new ObjectIdContractResolver();

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var jsonProperties = base.CreateProperties(type, memberSerialization);
            var idProperty = jsonProperties.SingleOrDefault(
                x => x.PropertyType == typeof(ObjectId) &&
                     (x.PropertyName.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                      (x.PropertyName.Equals("_Id", StringComparison.OrdinalIgnoreCase))));

            if (idProperty != null)
            {
                var originalName = idProperty.PropertyName;
                idProperty.PropertyName = "_id";
                idProperty.ShouldSerialize = value => !TypeExtension.IdIsEmpty(value, originalName);
            }

            return jsonProperties;
        }
    }
}