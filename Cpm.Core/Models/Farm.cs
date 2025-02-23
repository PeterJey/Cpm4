using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    public class Farm
    {
        public string FarmId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime FirstDayOfYear { get; set; }

        public ICollection<Site> Sites { get; set; }

        public Farm()
        {
            Sites = new List<Site>();
        }
    }
}