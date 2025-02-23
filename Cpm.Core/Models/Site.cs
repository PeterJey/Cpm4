using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    public class Site
    {
        public string SiteId { get; set; }

        [Required]
        public string FarmId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Postcode { get; set; }

        [Required]
        public int Order { get; set; }

        public ICollection<Field> Fields { get; set; }

        public ICollection<Scenario> Scenarios { get; set; }

        // Navigation

        public Farm Farm { get; set; }

        public ICollection<SiteUserPermission> UserPermissions { get; set; }

        public Site()
        {
            Fields = new List<Field>();
            Scenarios = new List<Scenario>();
            UserPermissions = new List<SiteUserPermission>();
        }
    }
}