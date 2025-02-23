using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class WeeklyOverviewVm
    {
        public string Title { get; set; }
        public int Position { get; set; }
        public IReadOnlyCollection<DiaryWeekFieldVm> Fields { get; set; }
        public IReadOnlyCollection<DayHeaderItem> DayHeaderWeek1 { get; set; }
        public IReadOnlyCollection<DayHeaderItem> DayHeaderWeek2 { get; set; }
        public decimal[] Week1Totals { get; set; }
        public decimal[] Week2Totals { get; set; }
    }
}