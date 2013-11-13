using System;
using Microsoft.Win32.SafeHandles;

namespace Nejdb.Internals
{
	internal class BsonHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private readonly Action<IntPtr> _delete;

		public BsonHandle(Func<IntPtr> create, Action<IntPtr> delete) : base(true)
		{
			handle = create();
			_delete = delete;
		}

		protected override bool ReleaseHandle()
		{
			_delete(handle);
			return true;
		}
	}
}