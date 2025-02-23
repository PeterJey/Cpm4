using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class FieldAllocationVm
    {
        public decimal? ToAllocate { get; set; }
        public decimal? Total { get; set; }
        public ICollection<ProductAllocationVm> Products { get; set; }
        public FieldVm Field { get; set; }
    }
}