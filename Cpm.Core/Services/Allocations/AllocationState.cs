using System;
using Cpm.Core.Models;

namespace Cpm.Core.Services.Allocations
{
    public class AllocationState
    {
        public AllocationState(DateTime date, Product product, Allocation allocation)
        {
            Field = allocation.Field;
            Product = product;
            Date = date;
            Weight = allocation.Weight;
        }

        public decimal Weight { get; set; }

        public DateTime Date { get; set; }
        public Product Product { get; set; }
        public Field Field { get; set; }
    }
}