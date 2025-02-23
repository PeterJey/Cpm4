using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class DiaryVm
    {
        public string SiteId { get; set; }

        public int Position { get; set; }
        public string Title { get; set; }

        public IReadOnlyCollection<string> Weekdays { get; set; }

        public IReadOnlyCollection<DiaryWeekVm> Weeks { get; set; }
    }
}