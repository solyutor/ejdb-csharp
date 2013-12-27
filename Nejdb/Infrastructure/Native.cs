// ============================================================================================
//   .NET API for EJDB database library http://ejdb.org
//   Copyright (C) 2012-2013 Softmotions Ltd <info@softmotions.com>
//
//   This file is part of EJDB.
//   EJDB is free software; you can redistribute it and/or modify it under the terms of
//   the GNU Lesser General Public License as published by the Free Software Foundation; either
//   version 2.1 of the License or any later version.  EJDB is distributed in the hope
//   that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
//   License for more details.
//   You should have received a copy of the GNU Lesser General Public License along with EJDB;
//   if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330,
//   Boston, MA 02111-1307 USA.
// ============================================================================================

using System;
using System.Runtime.InteropServices;
using System.Text;
using Nejdb.Internals;

namespace Nejdb.Infrastructure
{
    public static class Native
    {
        public static string StringFromNativeUtf8(IntPtr nativeUtf8)
        {
            int len = 0;
            for (; Marshal.ReadByte(nativeUtf8, len) != 0; ++len)
            {
            }
            if (len == 0)
            {
                return String.Empty;
            }
            byte[] buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

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