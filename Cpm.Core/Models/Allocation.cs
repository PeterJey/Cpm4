using System;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    public class Allocation : IVersionable
    {
        [Required]
        public string FieldId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string ProductType { get; set; }

        [Required]
        public int PerTray { get; set; }

        [Required]
        public int PerPunnet { get; set; }

        [Required]
        public decimal Weight { get; set; }

        public int Version { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        // Navigation

        public Field Field { get; set; }
    }
}
