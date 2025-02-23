using System;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    public class Scenario : IVersionable
    {
        [Required]
        public string ScenarioId { get; set; }

        [Required]
        public string SiteId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Version { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public string SerializedSettings { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        // Navigation

        public Site Site { get; set; }
    }
}