using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Nejdb.Internals
{
    internal static class ResourceHelper
    {
        static ResourceHelper()
        {
            var dllFileName = string.Format("tcejdb{0}.dll", Environment.Is64BitProcess ? "64" : "32");

            Assembly = Assembly
                .GetExecutingAssembly();

            DllResourceName = Assembly
                .GetManifestResourceNames()
                .Single(x => x.EndsWith(dllFileName));
        }

        private static readonly string DllResourceName;
        private static readonly Assembly Assembly;

        public static string ExportLibrary()
        {
            var fileName = Path.GetTempFileName();

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (var reader = Assembly.GetManifestResourceStream(DllResourceName))
            using (var writer = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            {
                reader.CopyTo(writer);
                writer.Flush(true);
            }

            return fileName;
        }
    }
}