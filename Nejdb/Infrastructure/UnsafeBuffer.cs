using System.IO;
using System.Text;

namespace Nejdb.Infrastructure
{
    internal unsafe struct UnsafeBuffer
    {
        public static readonly Encoding UTF8WithoutBom = new UTF8Encoding(false);
        
        public fixed byte Buffer [1024];

        public static void FillFromString(UnsafeBuffer* buffer, string value)
        {
            using (var stream = new UnsafeStream((byte*) buffer))
            using (var writer = new StreamWriter(stream, UTF8WithoutBom))
            {
                foreach (var symbol in value)
                {
                    writer.Write(symbol);
                }
            }
        }
    }
}