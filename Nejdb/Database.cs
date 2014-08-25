using System;
using System.Runtime.InteropServices;
using Nejdb.Bson;
using Nejdb.Infrastructure;
using Nejdb.Internals;
using Newtonsoft.Json;

namespace Nejdb
{
    /// <summary>
    /// Provides operation over ejdb database.
    /// </summary>
    public class Database : IDisposable
    {
        /// <summary>
        /// The default open mode (OpenMode.Writer | OpenMode.CreateIfNotExists) <c>(JBOWRITER | JBOCREAT)</c>
        /// </summary>
        public const OpenMode DefaultOpenMode = (OpenMode.Writer | OpenMode.CreateIfNotExists);

        internal readonly Library Library;
        internal DatabaseHandle DatabaseHandle;
        private DatabaseFunctions _functions;
        internal readonly StreamPool StreamPool;
        private JsonSerializer _serializer;

        /// <summary>
        /// Creates instance of EJDB. 
        /// </summary>
        /// <param name="library"></param>
        public Database(Library library)
        {
            _functions = library.Functions.Database;
            DatabaseHandle = new DatabaseHandle(library);

            Library = library;
            StreamPool = new StreamPool();

            Serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = NejdbContractResolver.Instance,
            };
            Serializer.Converters.Add(ObjectIdConverter.Instance);
        }

        /// <summary>
        /// Gets the last DB error code or <c>null</c> if underlying native database object does not exist.
        /// </summary>
        /// <value>The last DB error code.</value>
        public int LastErrorCode
        {
            get { return _functions.GetErrorCode(DatabaseHandle); }
        }

        /// <summary>
        /// Gets a value indicating whether this EJDB databse is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public bool IsOpen
        {
            get { return _functions.IsOpen(DatabaseHandle); }
        }

        /// <summary>
        /// Automatically creates new collection if it does't exists.
        /// </summary>
        /// <remarks>
        /// Collection options <c>options</c> are applied only for newly created collection.
        /// For existing collections <c>options</c> has no effect.
        /// </remarks>
        /// <returns><c>false</c> error ocurried.</returns>
        /// <param name="name">Name of collection.</param>
        /// <param name="options">Collection options.</param>
        public Collection CreateCollection(string name, CollectionOptions options)
        {
            return new Collection(this, name, options);
        }

        /// <summary>
        /// Gets existing collection and throws if in does not exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Collection GetCollection(string name)
        {
            return new Collection(this, name);
        }

        /// <summary>
        /// Synchronize database state with disk
        /// </summary>
        public void Synchronize()
        {
            if (_functions.Sync(DatabaseHandle))
            {
                return;
            }
            throw EjdbException.FromDatabase(this, "Failed to synchronize database");
        }


        /// <summary>
        /// Gets info of EJDB database itself and its collections.
        /// </summary>
        /// <value>The DB meta.</value>
        public BsonDocument DatabaseMetadata
        {
            get
            {
                using (var bson = new BsonHandle(() => _functions.GetMetadata(DatabaseHandle), Library))
                {
                    if (bson.IsInvalid)
                    {
                        EjdbException.FromDatabase(this, "Could not get database metadata");
                    }

                    using (var stream = Library.ConvertToStream(bson))
                    {
                        return new BsonDocument(stream);
                    }
                }
            }
        }

        /// <summary>
        /// Default serialezer used by <see cref="Collection"/> and <see cref="Cursor"/> objects those belongs to <see cref="Database"/> instance.
        /// </summary>
        public JsonSerializer Serializer
        {
            get { return _serializer; }
            set
            {
                if (value == null)
                {
                    ThrowHelper.ThrowNull("value");
                }
                _serializer = value;
            }
        }


        ///// <summary>
        ///// Executes EJDB command.
        ///// </summary>
        ///// <remarks>
        ///// Supported commands:
        /////
        ///// 1) Exports database collections data. See ejdbexport() method.
        ///// 
        ///// 	"export" : {
        ///// 	"path" : string,                    //Exports database collections data
        ///// 	"cnames" : [string array]|null,     //List of collection names to export
        ///// 	"mode" : int|null                   //Values: null|`JBJSONEXPORT` See ejdb.h#ejdbexport() method
        ///// }
        ///// 
        ///// Command response:
        ///// {
        ///// 	"log" : string,        //Diagnostic log about executing this command
        ///// 	"error" : string|null, //ejdb error message
        ///// 	"errorCode" : int|0,   //ejdb error code
        ///// }
        ///// 
        ///// 2) Imports previously exported collections data into ejdb.
        ///// 
        ///// 	"import" : {
        ///// 	"path" : string                     //The directory path in which data resides
        ///// 		"cnames" : [string array]|null,     //List of collection names to import
        ///// 		"mode" : int|null                //Values: null|`JBIMPORTUPDATE`|`JBIMPORTREPLACE` See ejdb.h#ejdbimport() method
        ///// }
        ///// 
        ///// Command response:
        ///// {
        ///// 	"log" : string,        //Diagnostic log about executing this command
        ///// 	"error" : string|null, //ejdb error message
        ///// 	"errorCode" : int|0,   //ejdb error code
        ///// }
        ///// </remarks>
        ///// <param name="cmd">Command object</param>
        ///// <returns>Command response.</returns>
        ////public BsonDocument Command(BsonDocument cmd)
        ////{
        ////	CheckDisposed();
        ////	byte[] cmdata = cmd.ToByteArray();
        ////	//internal static extern IntPtr _ejdbcommand([In] IntPtr db, [In] byte[] cmd);
        ////	IntPtr cmdret = _command(this, cmdata);
        ////	if (cmdret == IntPtr.Zero)
        ////	{
        ////		return null;
        ////	}
        ////	byte[] bsdata = BsonPtrIntoByteArray(cmdret);
        ////	if (bsdata.Count == 0)
        ////	{
        ////		return null;
        ////	}
        ////	BsonIterator it = new BsonIterator(bsdata);
        ////	return it.ToBsonDocument();
        ////}

        /// <summary>
        /// Opens or creates database file depending on <see cref="OpenMode"/>
        /// </summary>
        /// <param name="dbFilePath">Database filename</param>
        /// <param name="openMode"></param>
        public unsafe void Open(string dbFilePath, OpenMode openMode = DefaultOpenMode)
        {
            UnsafeBuffer buffer;
            UnsafeBuffer.FillFromString(&buffer, dbFilePath);

            bool result = _functions.OpenDatabase(DatabaseHandle, &buffer, openMode);
            if (!result)
            {
                throw EjdbException.FromDatabase(this, "Error on open database");
            }

        }

        /// <summary>
        /// Close database and free all resources
        /// </summary>
        public void Dispose()
        {
            Close();
            DatabaseHandle.Dispose();
            DatabaseHandle = null;
            _functions = null;
        }
        /// <summary>
        /// Closes current database
        /// </summary>
        public void Close()
        {
            if (IsOpen)
            {
                _functions.CloseDatabase(DatabaseHandle);
            }
        }

        internal void ThrowOnError()
        {
            var errorCode = LastErrorCode;

            if (errorCode != 0)
            {
                EjdbException.FromDatabase(this, string.Empty);
            }
        }
    }
}