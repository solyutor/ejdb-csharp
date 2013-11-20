using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace Nejdb.Tests
{
    public static class SerializationHelper
    {
        private static JsonSerializer _serializer;

        static SerializationHelper()
        {
            _serializer = new JsonSerializer();
        }

        public static string ToJson(this object self)
        {
            return JsonConvert.SerializeObject(self, Formatting.Indented);
        }

        public static byte[] ToBson(this object self)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BsonWriter(stream))
            {
                _serializer.Serialize(writer, self);

                var length = stream.Position + 1;

                var result = new Byte[length];
                Array.Copy(stream.GetBuffer(), result, length);
                return result;
            }
        }
    }
}