using System;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    public class HarvestRegister : SerializedValuesRegister, IVersionable
    {
        public int Version { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
    }
}
