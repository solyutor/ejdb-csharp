using System;
using Microsoft.Win32.SafeHandles;

namespace Ejdb.DB
{
	internal class CursorHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private Action<IntPtr> _release;

		public CursorHandle(Func<IntPtr> create, Action<IntPtr> release) : base(true)
		{
			this.handle = create();
			_release = release;

		}

		protected override bool ReleaseHandle()
		{
			_release(handle);
			return true;
		}
	}
}