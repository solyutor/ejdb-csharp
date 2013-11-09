using System;

namespace Nejdb
{
	//.//////////////////////////////////////////////////////////////////
	// 						Native open modes
	//.//////////////////////////////////////////////////////////////////
	[Flags]
	public enum OpenMode : int
	{
		/// <summary>
		/// Open as a reader.
		/// </summary>
		Reader = 1 << 0,

		/// <summary>
		/// Open as a writer.
		/// </summary>
		Writer = 1 << 1,

		/// <summary>
		/// Create if db file not exists. 
		/// </summary>
		CreateIfNotExists = 1 << 2,

		/// <summary>
		/// Truncate db on open.
		/// </summary>
		TruncateOnOpen = 1 << 3,

		/// <summary>
		/// Open without locking. 
		/// </summary>
		NoLock = 1 << 4,

		/// <summary>
		/// Lock without blocking.
		/// </summary>
		LockWithoutBlocking = 1 << 5,

		/// <summary>
		/// Synchronize every transaction with storage.
		/// </summary>
		SyncTransactionToStorage = 1 << 6
	}
}