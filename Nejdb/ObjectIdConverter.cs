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