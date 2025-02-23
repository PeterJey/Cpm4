using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class GridResultRowVm
    {
        public string Week { get; set; }
        public string Commencing { get; set; }
        public string Total { get; set; }
        public ICollection<GridResultValueVm> Values { get; set; }
        public string Labour { get; set; }
        public string StatsWeightMax { get; set; }
        public string StatsWeightMin { get; set; }
        public string StatsManHoursMax { get; set; }
        public string StatsManHoursMin { get; set; }
        public bool ShowStats { get; set; }
    }
}