using System;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    [Obsolete]
    public class DailyAllocation : IVersionable
    {
        [Required]
        public string SiteId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int Version { get; set; }

        public string Serialized { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        // Navigation

        public Site Site { get; set; }
    }
}
