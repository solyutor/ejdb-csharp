using System.IO;
using Nejdb.Bson;
using Nejdb.Infrastructure.Streams;

namespace Nejdb.Infrastructure
{
    internal unsafe struct UnsafeBuffer
    {
        public fixed byte Buffer [1024];

        public static void FillFromString(UnsafeBuffer* buffer, string value)
        {
            using (var stream = new UnsafeStream((byte*) buffer))
            using (var writer = new StreamWriter(stream, BsonConstants.Encoding))
            {
                foreach (var symbol in value)
                {
                    writer.Write(symbol);
                }
            }
        }
    }
}