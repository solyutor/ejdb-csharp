using System;

namespace Nejdb.Infrastructure
{
    internal static class ThrowHelper
    {
        public static void ThrowNull(string argument)
        {
            throw new ArgumentNullException(argument);
        }
    }
}