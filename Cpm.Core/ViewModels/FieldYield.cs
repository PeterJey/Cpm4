using System;
using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Extensions;

namespace Cpm.Core.ViewModels
{
    public class FieldYield
    {
        public DateTime FirstDay { get; set; }
        public IEnumerable<decimal?> Values { get; set; }

        public DateTime? LastDay
        {
            get
            {
                var count = Values
                    .SkipNullsFromEnd()
                    .Count();

                return count > 0
                    ? FirstDay.Date.AddDays(count - 1)
                    : (DateTime?) null;
            }
        }
    }
}