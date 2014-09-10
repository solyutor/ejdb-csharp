using System.Collections.Generic;
using System.Linq;

namespace Codestellation.Quarks.Collections
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> self)
        {
            return self ?? Enumerable.Empty<T>();
        }
    }
}