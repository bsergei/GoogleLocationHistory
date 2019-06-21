using System.Collections.Generic;
using System.Linq;

namespace GoogleLocationHistory
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T?> AsNullable<T>(this IEnumerable<T> src) where T : struct
        {
            return src.Cast<T?>();
        }
    }
}