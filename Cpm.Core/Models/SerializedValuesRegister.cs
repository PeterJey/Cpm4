using System;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    public abstract class SerializedValuesRegister
    {
        [Required]
        public string FieldId { get; set; }

        public DateTime FirstDay { get; set; }

        [Required]
        public string SerializedValues { get; set; }

        // Navigation

        public Field Field { get; set; }
    }
}