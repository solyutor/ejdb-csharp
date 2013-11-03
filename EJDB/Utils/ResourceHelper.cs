using System;
using System.IO;
using System.Reflection;

namespace Ejdb.Utils
{
	internal static class ResourceHelper
	{
		static ResourceHelper()
		{
			DllResourceName = string.Format("{0}.tcejdb{1}.dll", typeof(ResourceHelper).Namespace, Environment.Is64BitProcess ? "64" : "32");
		}

		private static readonly string DllResourceName;

		public static string ExportLibrary()
		{
			var fileName = Path.GetTempFileName();

			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}

			using (var reader = Assembly.GetExecutingAssembly().GetManifestResourceStream(DllResourceName))
			using (var writer = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
			{
				reader.CopyTo(writer);
				writer.Flush(true);
			}

			return fileName;
		}
	}

}