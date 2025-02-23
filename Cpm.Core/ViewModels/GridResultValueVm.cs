namespace Cpm.Core.ViewModels
{
    public class GridResultValueVm
    {
        public string Weight { get; set; }
        public bool IsActual { get; set; }
        public bool IsInferred { get; set; }
        public string StatsWeightMin { get; set; }
        public string StatsWeightMax { get; set; }
        public string StatsManHoursMin { get; set; }
        public string StatsManHoursMax { get; set; }
        public bool ShowStats { get; set; }
    }
}