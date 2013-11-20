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
                JsonValueWriter<TValue>.Write(writer, values[i]);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        public static void WriteCriterionArray(this JsonWriter writer, string arrayName, params ICriterion[] criterions)
        {
            writer.WritePropertyName(arrayName);

            writer.WriteStartArray();
            for (int i = 0; i < criterions.Length; i++)
            {
                writer.WriteStartObject();
                criterions[i].WriteTo(writer);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        public static void WriteObjectBasedCriterion(this JsonWriter writer, string criteriton, ICriterion subCriterion)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(criteriton);
            subCriterion.WriteTo(writer);
            writer.WriteEndObject();
        }
    }
}