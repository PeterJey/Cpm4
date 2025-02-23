using System.Collections.Generic;
using System.Linq;

namespace Cpm.Core.Farms
{
    public abstract class ModelBase
    {
        public abstract ICollection<string> GetErrors();

        protected void CheckCollection<T>(List<string> errors, ICollection<T> collection, string elementName, string emptyMessage)
            where T : ModelBase
        {
            if (collection?.Any() ?? false)
            {
                errors.AddRange(
                    collection
                        .SelectMany(
                            (s, i) => s?.GetErrors().Select(e => $"{elementName} #{i + 1}: {e}") ?? new[] { $"{elementName} #{i + 1} is null." })
                );
            }
            else
            {
                errors.Add(emptyMessage);
            }
        }
    }
}