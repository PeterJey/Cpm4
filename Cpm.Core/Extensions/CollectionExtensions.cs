using System.Collections.Generic;

namespace Cpm.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddIfNotEmpty(this ICollection<string> collection, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                collection.Add(text);
            }
        }
    }
}
