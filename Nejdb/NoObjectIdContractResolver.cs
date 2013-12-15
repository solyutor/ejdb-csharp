using System;
using System.Collections.Generic;
using System.Linq;
using Nejdb.Bson;
using Nejdb.Internals;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;

namespace Nejdb
{
    internal class NoObjectIdContractResolver : DefaultContractResolver
    {
        public static readonly NoObjectIdContractResolver Instance = new NoObjectIdContractResolver();

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

    internal class ObjectIdConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var bsonWriter = writer as BsonWriter;
            if (bsonWriter != null)
            {
                bsonWriter.WriteObjectId(((ObjectId)value).ToBytes());
            }
            else
            {
                writer.WriteValue(value.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var bsonReader = reader as BsonReader;
            if (bsonReader == null)
            {
                return new ObjectId(reader.ReadAsString());
            }
            else
            {
                var bytes = bsonReader.ReadAsBytes();
                return new ObjectId(bytes);    
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (ObjectId);
        }
    }
}