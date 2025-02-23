using System.Collections.Generic;

namespace Cpm.Core.Extensions
{
    public static class GeneralExtensions
    {
        public static bool IsBetween<T>(this T item, T start, T end)
        {
            return Comparer<T>.Default.Compare(item, start) >= 0
                   && Comparer<T>.Default.Compare(item, end) <= 0;
        }
    }
}
