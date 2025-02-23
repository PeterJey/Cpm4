using System;
using System.Collections.Generic;

namespace Cpm.Core.Services.Allocations
{
    public class AllocationBreakdown
    {
        public IDictionary<Tuple<Product, string>, decimal> Splits { get; set; }
    }
}