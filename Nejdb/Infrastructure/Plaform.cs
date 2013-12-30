using System;
using System.Runtime.InteropServices;
using Nejdb.Internals;

namespace Nejdb.Infrastructure
{
    internal static class Plaform
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern MethodHandle GetProcAddress(LibraryHandle library, string procedureName);

        internal static TDelegate GetUnmanagedDelegate<TDelegate>(this LibraryHandle library) where TDelegate : class
        {
            var delegateType = typeof(TDelegate);
            var attributeType = typeof(UnmanagedProcedureAttribute);
            var customAttributes = delegateType.GetCustomAttributes(attributeType, false);
            if (customAttributes.Length != 1)
            {
                throw new InvalidOperationException("Delegate " + delegateType.FullName + "should be marked with " + attributeType.FullName);
            }

            var procedureName = ((UnmanagedProcedureAttribute)customAttributes[0]).Name;
            var methodHandle = GetProcAddress(library, procedureName);
            if (methodHandle.IsInvalid)
            {
                var error = Marshal.GetLastWin32Error();
                throw new InvalidOperationException("Cannot get proc address '" + procedureName + "'. Win32 error =" + error);
            }

            return Marshal.GetDelegateForFunctionPointer(methodHandle.DangerousGetHandle(), delegateType) as TDelegate;
        }
    }
}