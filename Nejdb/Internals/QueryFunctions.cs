using System;
using System.Runtime.InteropServices;
using Codestellation.Quarks.Native;

namespace Nejdb.Internals
{
    internal class QueryFunctions
    {
        internal readonly CreateQueryDelegate Create;
        internal readonly ExecuteQueryDelegate Execute;
        internal readonly SetHintsDelegate SetHints;
        internal readonly DeleteCursorDelegate DeleteCursor;
        internal readonly AddOrDelegate AddOr;
        internal readonly DeleteQueryDelegate Delete;
        internal readonly CursorResultDelegate CursorResult;

        internal readonly NewBufferDelegate NewBuffer;
        internal readonly DeleteBufferDelegate DeleteBuffer;
        internal readonly GetSizeDelegate BufferSize;
        internal readonly ToStringDelegate BufferToString;

        public unsafe QueryFunctions(LibraryHandle handle)
        {
            Create = handle.GetUnmanagedDelegate<CreateQueryDelegate>();

            Delete = handle.GetUnmanagedDelegate<DeleteQueryDelegate>();

            Execute = handle.GetUnmanagedDelegate<ExecuteQueryDelegate>();
            DeleteCursor = handle.GetUnmanagedDelegate<DeleteCursorDelegate>();

            SetHints = handle.GetUnmanagedDelegate<SetHintsDelegate>();
            AddOr = handle.GetUnmanagedDelegate<AddOrDelegate>();

            CursorResult = handle.GetUnmanagedDelegate<CursorResultDelegate>();

            NewBuffer = handle.GetUnmanagedDelegate<QueryFunctions.NewBufferDelegate>();
            DeleteBuffer = handle.GetUnmanagedDelegate<QueryFunctions.DeleteBufferDelegate>();
            BufferSize = handle.GetUnmanagedDelegate<GetSizeDelegate>();
            BufferToString = handle.GetUnmanagedDelegate<ToStringDelegate>();
        }


        //EJDB_EXPORT EJQ* ejdbcreatequery2(EJDB *jb, void *qbsdata);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbcreatequery2")]
        internal delegate IntPtr CreateQueryDelegate([In] DatabaseHandle database, [In] byte[] queryAsBson);

        //EJDB_EXPORT void ejdbquerydel(EJQ *q);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbquerydel")]
        internal delegate void DeleteQueryDelegate(IntPtr query);

        //EJDB_EXPORT EJQ* ejdbqueryhints(EJDB *jb, EJQ *q, void *hintsbsdata)
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqueryhints")]
        internal delegate IntPtr SetHintsDelegate([In] DatabaseHandle database, [In] QueryHandle query, [In] byte[] hintsAsBson);

        //EJDB_EXPORT EJQ* ejdbqueryaddor(EJDB *jb, EJQ *q, void *orbsdata)
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqueryaddor")]
        internal delegate IntPtr AddOrDelegate([In] DatabaseHandle database, [In] QueryHandle query, [In] byte[] addOrAsBson);

        //EJDB_EXPORT EJQRESULT ejdbqryexecute(EJCOLL *jcoll, const EJQ *q, uint32_t *count, int qflags, TCXSTR *log)
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqryexecute")]
        internal delegate IntPtr ExecuteQueryDelegate([In] CollectionHandle collection, [In] QueryHandle query, out int count, [In] QueryMode flags, [In] IntPtr logxstr);

        //EJDB_EXPORT void ejdbqresultdispose(EJQRESULT qr);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqresultdispose")]
        internal delegate void DeleteCursorDelegate([In] IntPtr cursor);

        ////const void* ejdbqresultBsondata(EJQRESULT qr, int pos, int *size)
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("ejdbqresultbsondata")]
        internal unsafe delegate byte* CursorResultDelegate([In] CursorHandle cursor, [In] int position, out int size);

        //EJDB_EXPORT TCXSTR *tcxstrnew(void)
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("tcxstrnew")]
        internal delegate IntPtr NewBufferDelegate();

        //EJDB_EXPORT void tcxstrdel(TCXSTR *xstr);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("tcxstrdel")]
        internal delegate IntPtr DeleteBufferDelegate(IntPtr query);

        //EJDB_EXPORT int tcxstrsize(const TCXSTR *xstr);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("tcxstrsize")]
        internal delegate int GetSizeDelegate(IntPtr query);

        //EJDB_EXPORT int tcxstrptr(const TCXSTR *xstr);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl), UnmanagedProcedure("tcxstrptr")]
        internal unsafe delegate sbyte* ToStringDelegate(IntPtr query);
    }
}