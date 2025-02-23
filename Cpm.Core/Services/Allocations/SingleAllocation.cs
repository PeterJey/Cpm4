using System;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Services.Allocations
{
    public class SingleAllocation
    {
        [Required]
        public string FieldId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public Product Product { get; set; }
        [Required]
        [Range(0, 999999999)]
        public decimal Weight { get; set; }
    }
}