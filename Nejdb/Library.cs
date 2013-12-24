using System;
using System.IO;
using System.Runtime.InteropServices;
using Nejdb.Bson;
using Nejdb.Internals;

namespace Nejdb
{
    /// <summary>
    /// Loads tcejdb.dll and 
    /// </summary>
    public class Library : IDisposable
    {
        internal readonly LibraryHandle LibraryHandle;

        //[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdbversion", CallingConvention = CallingConvention.Cdecl)]
        //EJDB_EXPORT const char *ejdbversion();
        //internal static extern IntPtr _ejdbversion();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbversion")]
        private delegate IntPtr GetVersion(LibraryHandle handle);

        //[DllImport(EJDB_LIB_NAME, EntryPoint = "ejdberrmsg", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern IntPtr _ejdberrmsg(int ecode)
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdberrmsg")]
        private delegate IntPtr GetErrorMessage(int errorCode);


        //EJDB_EXPORT void Bson_del(Bson *b);
        //[DllImport(EJDB_LIB_NAME, EntryPoint = "bson_del", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern void _Bson_del([In] IntPtr bsptr);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("bson_del")]
        private delegate void FreeBsonDelegate(IntPtr Bson);

        //EJDB_EXPORT const char* bson_data2(const Bson *b, int *bsize);
        //[DllImport(EJDB_LIB_NAME, EntryPoint = "bson_data2", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern IntPtr _bson_data2([In] IntPtr bsptr, out int size);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("bson_data2")]
        private delegate IntPtr BsonToStringDelegate(BsonHandle Bson, out int size);


        //EJDB_EXPORT Bson* json2bson(const char *jsonstr);
        //[DllImport(EJDB_LIB_NAME, EntryPoint = "json2Bson", CallingConvention = CallingConvention.Cdecl)]
        //internal static extern IntPtr _json2bson([In] IntPtr jsonstr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("json2bson")]
        private delegate IntPtr JsonToBsonDelegate([In]IntPtr json);

        /// <summary>
        /// The EJDB library version hex code.
        /// </summary>
        private readonly long _hexVersion;

        /// <summary>
        /// The EJDB library version
        /// </summary>
        private readonly string _version;

        private readonly GetErrorMessage _getErrorMessage;
        private readonly FreeBsonDelegate _freeBson;
        private readonly BsonToStringDelegate _bsonToString;
        private JsonToBsonDelegate _jsonToBson;

        private static readonly Lazy<Library> _instance;

        internal readonly Functions Functions;

        /// <summary>
        /// Returns an instance of <see cref="Library"/> class.
        /// </summary>
        public static Library Instance
        {
            get { return _instance.Value; }
        }

        static Library()
        {
            _instance = new Lazy<Library>(Create);
        }

        private Library(LibraryHandle libraryHandle)
        {
            LibraryHandle = libraryHandle;

            Functions = new Functions(libraryHandle);

            _getErrorMessage = LibraryHandle.GetUnmanagedDelegate<GetErrorMessage>();

            _freeBson = libraryHandle.GetUnmanagedDelegate<FreeBsonDelegate>();
            _bsonToString = libraryHandle.GetUnmanagedDelegate<BsonToStringDelegate>();
            //_jsonToBson = libraryHandle.GetUnmanagedDelegate<JsonToBsonDelegate>();

            var getVersion = LibraryHandle.GetUnmanagedDelegate<GetVersion>();

            IntPtr version = getVersion(LibraryHandle);

            if (version == IntPtr.Zero)
            {
                throw new Exception("Unable to get ejdb library version");
            }

            _version = Native.StringFromNativeUtf8(version); //UnixMarshal.PtrToString(vres, Encoding.UTF8);
            _hexVersion = Convert.ToInt64("0x" + Version.Replace(".", ""), 16);
        }


        /// <summary>
        /// Gets the EJDB library hex encoded version.
        /// </summary>
        /// <remarks>
        /// E.g: for the version "1.1.13" return value will be: 0x1113
        /// </remarks>
        /// <value>The lib hex version.</value>
        public long HexVersion
        {
            get { return _hexVersion; }
        }

        /// <summary>
        /// Gets the EJDB library version.
        /// </summary>
        /// <value>The LIB version.</value>
        public string Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Creates new instance of a <see cref="Library"/> class.
        /// <remarks>It's better to use the only instance of library for process. You can manage it yourself or use <see cref="Library#Instance"/> property.</remarks>
        /// </summary>
        /// <returns></returns>
        public static Library Create()
        {
            var libraryHandle = LibraryHandle.Load();

            var result = new Library(libraryHandle);

            return result;
        }

        /// <summary>
        /// Creates new EJDB database instance.
        /// </summary>
        /// <returns></returns>
        public Database CreateDatabase()
        {
            return new Database(this);
        }

        public string GetLastErrorMessage(int errorCode)
        {
            return Native.StringFromNativeUtf8(_getErrorMessage(errorCode)); //UnixMarshal.PtrToString(_ejdberrmsg((int) ecode), Encoding.UTF8);
        }

        //Used internally by BsonHandle
        internal void FreeBson(IntPtr Bson)
        {
            _freeBson(Bson);
        }

        internal Stream ConvertToStream(BsonHandle bson, StreamPool streamPool)
        {
            int size;
            IntPtr bsonPointer = _bsonToString(bson, out size);

            MemoryStream stream = streamPool.GetStream();

            if (stream.Capacity < size)
            {
                stream.Capacity = size;
            }

            byte[] buffer = stream.GetBuffer();
            Marshal.Copy(bsonPointer, buffer, 0, size);
            TypeExtension.SetLength(stream, size);
            return stream;
        }

        ///// <summary>
        ///// Convert JSON string into BsonDocument.
        ///// Returns `null` if conversion failed.
        ///// </summary>
        ///// <returns>The BsonDocument instance on success.</returns>
        ///// <param name="json">JSON string</param>
        //public BsonDocument Json2Bson(string json)
        //{
        //	IntPtr jsonptr = Native.NativeUtf8FromString(json);
        //	try
        //	{
        //		using (var Bson = new BsonHandle())_jsonToBson(jsonptr))
        //		{
        //			return ConvertToBsonDocument(Bson);
        //		}
        //	}
        //	finally
        //	{
        //		Marshal.FreeHGlobal(jsonptr); //UnixMarshal.FreeHeap(jsonptr);
        //	}
        //}

        public void Dispose()
        {
            LibraryHandle.Dispose();
        }
    }
}