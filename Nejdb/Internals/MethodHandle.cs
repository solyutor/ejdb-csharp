using Microsoft.Win32.SafeHandles;

namespace Nejdb.Internals
{
    internal class MethodHandle : SafeHandleZeroOrMinusOneIsInvalid
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