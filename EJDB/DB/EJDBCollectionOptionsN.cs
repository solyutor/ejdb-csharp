using System.Runtime.InteropServices;

namespace Ejdb.DB
{
	/// <summary>
	/// Corresponds to <c>EJCOLLOPTS</c> in ejdb.h
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct EjdbCollectionOptionsN {
		[MarshalAs(UnmanagedType.U1)]
		public bool large;

		[MarshalAs(UnmanagedType.U1)]
		public bool compressed;

		public long records;

		public int cachedrecords;
	}
}