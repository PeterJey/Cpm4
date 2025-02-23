using System;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    public class FieldScore : IVersionable
    {
        [Required]
        public string FieldId { get; set; }

        [Required]
        public int Version { get; set; }

        public int GrowingDegreeHours { get; set; }

        public int ChillHours { get; set; }

        public string ActiveScenarioId { get; set; }
        
        public string Description { get; set; }

        [Required]
        public string SerializedBudget { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        // Navigation
        public Field Field { get; set; }
    }
}