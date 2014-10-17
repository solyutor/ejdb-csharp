using System;
using Nejdb.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Nejdb
{
    internal class ObjectIdConverter : JsonConverter
    {
        public static readonly ObjectIdConverter Instance = new ObjectIdConverter();
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var bsonWriter = writer as BsonWriter;
            if (bsonWriter != null)
            {
                var objectId = (ObjectId?)value;

                if (objectId.HasValue)
                {
                    byte[] bytes = objectId.Value.ToBytes();
                    bsonWriter.WriteObjectId(bytes);
                }
                else
                {
                    bsonWriter.WriteNull();
                }
            }
            else
            {
                writer.WriteValue(value.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException("Cannot read using ObjectIdConverter. Use conversion operator on ObjectId type.");
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (ObjectId) || objectType == typeof(ObjectId?);
        }
    }
}