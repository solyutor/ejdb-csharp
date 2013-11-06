using System;

namespace Ejdb.DB
{
	[Flags]
	public enum QueryMode : int
	{
		Normal = 0,
		
		/// <summary>
		/// Query only count(*)
		/// </summary>
		Count  = 1,

		/// <summary>
		/// Fetch first record only.
		/// </summary>
		FindOne = 1 << 1,

		/// <summary>
		/// Explain query execution and store query execution log into <see cref="Ejdb.DB.EJDBQCursor#Log"/>
		/// </summary>
		Explain = 1 << 16
	}
}