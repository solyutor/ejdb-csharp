using Newtonsoft.Json;

namespace Nejdb.Queries
{
    internal static class JsonWriterExtensions
    {
        public static void WriteArrayBaseCritertion<TValue>(this JsonWriter writer, string criterion, params TValue[] values)
        {
            writer.WriteStartObject();


            writer.WritePropertyName(criterion);

            writer.WriteStartArray();
            for (int i = 0; i < values.Length; i++)
            {
                writer.WriteValue(values[i]);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}