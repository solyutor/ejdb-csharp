using Microsoft.Win32.SafeHandles;

namespace Ejdb.Utils
{
	public class SafeMethodHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeMethodHandle() : base(false)
		{
		}

		protected override bool ReleaseHandle()
		{
			return false;
		}
	}
}