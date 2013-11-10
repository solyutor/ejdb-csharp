using System;
using System.Runtime.InteropServices;
using System.Threading;
using Nejdb.Bson;
using Nejdb.Internals;

namespace Nejdb
{
	public class Query : IDisposable
	{
		private readonly Collection _collection;
		private QueryHandle _handle;
		private ExecuteQueryDelegate _execute;
		private QueryHints _queryHints;
		private SetHintsDelegate _setHints;
		private DeleteCursorDelegate _deleteCursor;
		private AddOrDelegate _addOr;
		////EJDB_EXPORT EJQ* ejdbqueryhints(EJDB *jb, EJQ *q, void *hintsbsdata)
		//[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbqueryhints", CallingConvention = CallingConvention.Cdecl)]
		//static extern IntPtr _ejdbqueryhints([In] IntPtr jb, [In] IntPtr qptr, [In] byte[] bsdata);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqueryhints")]
		private delegate IntPtr SetHintsDelegate([In] DatabaseHandle database, [In] QueryHandle query, [In] byte[] hintsAsBson);

		////EJDB_EXPORT EJQ* ejdbqueryaddor(EJDB *jb, EJQ *q, void *orbsdata)
		//[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbqueryaddor", CallingConvention = CallingConvention.Cdecl)]
		//static extern IntPtr _ejdbqueryaddor([In] IntPtr jb, [In] IntPtr qptr, [In] byte[] bsdata);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqueryaddor")]
		private delegate IntPtr AddOrDelegate([In] DatabaseHandle database, [In] QueryHandle query, [In] byte[] addOrAsBson);

		////EJDB_EXPORT EJQRESULT ejdbqryexecute(EJCOLL *jcoll, const EJQ *q, uint32_t *count, int qflags, TCXSTR *log)
		//[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbqryexecute", CallingConvention = CallingConvention.Cdecl)]
		//static extern IntPtr _ejdbqryexecute([In] IntPtr jcoll, [In] IntPtr q, out int count, [In] int qflags, [In] IntPtr logxstr);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqryexecute")]
		private delegate IntPtr ExecuteQueryDelegate([In] CollectionHandle collection, [In] QueryHandle query, out int count, [In] QueryMode flags, [In] IntPtr logxstr);

		////EJDB_EXPORT void ejdbqresultdispose(EJQRESULT qr);
		//[DllImport(EJDB.EJDB_LIB_NAME, EntryPoint = "ejdbqresultdispose", CallingConvention = CallingConvention.Cdecl)]
		//static extern void _ejdbqresultdispose([In] IntPtr qres);
		[UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqresultdispose")]
		private delegate void DeleteCursorDelegate([In] IntPtr cursor);

		

		public Query(Collection collection)
		{
			_collection = collection;
			_handle = new QueryHandle(collection.Database);
			var libraryHandle = collection.Database.Library.LibraryHandle;
			_execute = libraryHandle.GetUnmanagedDelegate<ExecuteQueryDelegate>();
			_deleteCursor = libraryHandle.GetUnmanagedDelegate<DeleteCursorDelegate>();

			_setHints = libraryHandle.GetUnmanagedDelegate<SetHintsDelegate>();
			_addOr = libraryHandle.GetUnmanagedDelegate<AddOrDelegate>();

			_queryHints = new QueryHints();
		}

		public QueryHints QueryHints
		{
			get { return _queryHints; }
		}

		/// <summary>
		/// Append OR joined restriction to this query.
		/// </summary>
		/// <returns>This query object.</returns>
		/// <param name="docobj">Query document.</param>
		public Query AppendOrRestriction(object docobj)
		{
			BsonDocument doc = BsonDocument.ValueOf(docobj);
			//static extern IntPtr _ejdbqueryaddor([In] IntPtr jb, [In] IntPtr qptr, [In] byte[] bsdata);
			IntPtr qptr = _addOr(_collection.Database.DatabaseHandle, _handle, doc.ToByteArray());
			if (qptr == IntPtr.Zero)
			{
				EjdbException.FromDatabase(_collection.Database, "Failed to append Or restriction");
				//throw new EJDBQueryException(_jb);
			}
			return this;
		}

		public Cursor Execute(QueryMode flags = QueryMode.Normal)
		{
			SetHints();
				
			int count = 0;
			QueryLogBuffer logsptr = null;
			//TODO: check it more elegant way?
			if ((flags & QueryMode.Explain) != 0)
			{
				//static extern IntPtr _tcxstrnew();
				logsptr = new QueryLogBuffer(_collection.Database.Library.LibraryHandle); //Create dynamic query execution log buffer
			}
			
			Cursor cursor = null;
			
			try
			{
				//static extern IntPtr _ejdbqryexecute([In] IntPtr jcoll, [In] IntPtr q, out int count, [In] int qflags, [In] IntPtr logxstr);
				IntPtr bufferPointer = logsptr == null ? IntPtr.Zero : logsptr.DangerousGetHandle();
				var cursorHandle = new CursorHandle(() => _execute(_collection.CollectionHandle, _handle, out count, flags, bufferPointer), pointer => _deleteCursor(pointer));
				cursor = new Cursor(_collection.Database.Library.LibraryHandle,  cursorHandle, count);
			}
			finally
			{
				if (logsptr != null )
				{
					try
					{
						if (cursor != null)
						{
							//static extern IntPtr _tcxstrptr([In] IntPtr strptr);
							IntPtr sbptr = logsptr.AsString();
							
							Log = Native.StringFromNativeUtf8(sbptr); //UnixMarshal.PtrToString(sbptr, Encoding.UTF8);
						}
					}
					finally
					{
						//static extern IntPtr _tcxstrdel([In] IntPtr strptr);
						logsptr.Dispose();
					}
				}
			}
			_collection.Database.ThrowOnError();
			
			return cursor;
		}

		public BsonIterator FinOne()
		{
			using (Cursor cur = Execute(QueryMode.FindOne))
			{
				return cur.Next();
			}
		}

		public int Count()
		{
			using (Cursor cur = Execute(QueryMode.Count))
			{
				return cur.Count;
			}
		}

		public int Update()
		{
			using (Cursor cur = Execute(QueryMode.Count))
			{
				return cur.Count;
			}
		}

		public string Log { get; private set; }

		private void SetHints()
		{
			if (_queryHints.IsEmpty)
			{
				return;
			}

			IntPtr qptr = _setHints(_collection.Database.DatabaseHandle, _handle, _queryHints.ToByteArray());

			if (qptr == IntPtr.Zero)
			{
				//TODO: throw more specific exception
				//throw new EJDBQueryException(_jb);
				EjdbException.FromDatabase(_collection.Database, "Failed to set hints");
			}
		}

		public void Dispose()
		{
			var handle = _handle;
			Thread.MemoryBarrier();

			if (handle == null)
			{
				return;
			}
			handle.Dispose();
			_handle = null;
		}
	}
}