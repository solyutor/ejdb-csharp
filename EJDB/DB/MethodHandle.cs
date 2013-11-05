using Microsoft.Win32.SafeHandles;

namespace Ejdb.DB
{
	public class MethodHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public MethodHandle() : base(false)
		{
		}

		protected override bool ReleaseHandle()
		{
			return false;
		}
	}
}