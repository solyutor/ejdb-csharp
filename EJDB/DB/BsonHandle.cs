using System;
using Microsoft.Win32.SafeHandles;

namespace Ejdb.DB
{
	public class BsonHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private readonly Action<IntPtr> _delete;

		public BsonHandle(Database database, Func<IntPtr> create, Action<IntPtr> delete) : base(true)
		{
			handle = create();
			if (IsInvalid)
			{
				throw EJDBException.FromDatabase(database, "Failed to get database metadata");
			}

			_delete = delete;
		}


		protected override bool ReleaseHandle()
		{
			_delete(handle);
			return true;
		}
	}
}