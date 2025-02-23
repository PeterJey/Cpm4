using System;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    public class HarvestProfile
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string ProfileName { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public string SerializedCriteria { get; set; }

        [Required]
        public string Quality { get; set; }

        [Required]
        public int StartingWeek { get; set; }

        [Required]
        public decimal ExtraPotential { get; set; }

        [Required]
        public string SerializedPoints { get; set; }

        public string Description { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public string Comment { get; set; }
    }
}
