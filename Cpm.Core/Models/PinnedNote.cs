using System;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    public class PinnedNote : IVersionable
    {
        [Required]
        public string FieldId { get; set; }

        [Required]
        public int Version { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Text { get; set; }

        public string SerializedPictureMetadata { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        // Navigation

        public Field Field { get; set; }
    }
}