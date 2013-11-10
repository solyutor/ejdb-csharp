using System;

namespace Nejdb
{
	/// <summary>
	/// Shows possible query modes
	/// </summary>
	[Flags]
	public enum QueryMode : int
	{
		/// <summary>
		/// Execute query and return result as a cursor
		/// </summary>
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
		/// Explain query execution and store query execution log
		/// </summary>
		Explain = 1 << 16
	}
}