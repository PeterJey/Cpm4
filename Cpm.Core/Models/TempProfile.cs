using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cpm.Core.Models
{
    public class TempProfile
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string SerializedCriteria { get; set; }

        [Required]
        public string SerializedPoints { get; set; }

        public string Description { get; set; }

        public string Comment { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
    }
}
