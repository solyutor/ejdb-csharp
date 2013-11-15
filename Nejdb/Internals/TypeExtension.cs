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
using System.Runtime.CompilerServices;

namespace Nejdb.Internals
{

    /// <summary>
    /// Check if specified type is anonymous.
    /// </summary>
    public static class TypeExtension
    {
        public static bool IsAnonymousType(this Type type)
        {
            bool hasCompilerGeneratedAttribute = (type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length > 0);
            bool nameContainsAnonymousType = (type.FullName.Contains("AnonType") || type.FullName.Contains("AnonymousType"));
            return (hasCompilerGeneratedAttribute && nameContainsAnonymousType);
        }

        public static byte ToByteFromHex(this string self, int startIndex)
        {
            var firstSymbol = Char.ToLower(self[startIndex]);
            var secondSymbol = Char.ToLower(self[startIndex + 1]);

            byte upper = (byte)(ByteFromHex(firstSymbol) << 4);
            byte lower = ByteFromHex(secondSymbol);

            return (byte)(upper | lower);
        }

        private static byte ByteFromHex(char symbol)
        {
            switch (symbol)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'a':
                    return 10;
                case 'b':
                    return 11;
                case 'c':
                    return 12;
                case 'd':
                    return 13;
                case 'e':
                    return 14;
                case 'f':
                    return 15;
                default:
                    var errorMessage = string.Format("Expected chars [0-9] or [a-f], but '{0}'", symbol);
                    throw new ArgumentOutOfRangeException("symbol", errorMessage);
            }
        }
    }


}

