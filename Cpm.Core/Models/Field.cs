using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    public class Field
    {
        public string FieldId { get; set; }

        [Required]
        public string SiteId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Order { get; set; }

        public string Variety { get; set; }

        public string ProfileName { get; set; }

        [Required]
        public decimal AreaInHectares { get; set; }

        // Navigation
        public ICollection<PinnedNote> PinnedNotes { get; set; }

        public ICollection<FieldScore> FieldScores { get; set; }
        
        public ICollection<HarvestRegister> HarvestRegisters { get; set; }
        
        public ICollection<PickingPlan> PickingPlans { get; set; }

        public ICollection<Allocation> Allocations { get; set; }

        public Site Site { get; set; }

        public Field()
        {
            PinnedNotes = new List<PinnedNote>();
            FieldScores = new List<FieldScore>();
            HarvestRegisters = new List<HarvestRegister>();
            PickingPlans = new List<PickingPlan>();
            Allocations = new List<Allocation>();
        }
    }
}