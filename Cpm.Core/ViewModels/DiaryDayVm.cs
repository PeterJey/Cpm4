using System;
using System.Collections.Generic;
using Cpm.Core.Services.Diary;
using Cpm.Core.Services.Fields;

namespace Cpm.Core.ViewModels
{
    public class DiaryDayVm
    {
        public string FieldName { get; set; }
        public bool CanBecomeLastHarvestOfTheWeek { get; set; }
        public bool CanBePlanned { get; set; }

        public bool IsPadding { get; set; }

        public DateTime Date { get; set; }

        public decimal? Weight { get; set; }
        public decimal? Planned { get; set; }

        public NoteDetails Note { get; set; }
        public string FieldId { get; set; }
        public string SiteId { get; set; }
        public IReadOnlyCollection<PictureVm> Pictures { get; set; }
        public bool ShowChangeHarvest { get; set; }
        public bool ShowPlanning { get; set; }
        public bool ShowChangeDiary { get; set; }
    }
}