using System.Collections.Generic;
using System.Linq;

namespace Nett.Parser.Cst
{
    internal static class EnumerableEx
    {
        public static IEnumerable<T> NonNullItems<T>(params T[] items)
            where T : class
            => items.Where(i => i != null);
    }
}
