using System.Collections.Generic;

namespace Cpm.Core.Services.Allocations
{
    public class DailyAllocations
    {
        public ICollection<Product> UsedProducts { get; set; }
        public ICollection<Product> AllProducts { get; set; }
        public ICollection<AllocationState> Allocations { get; set; }
    }
}