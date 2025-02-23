using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Extensions;

namespace Cpm.Core.ViewModels
{
    public class DiaryWeekVm
    {
        public string FieldId { get; set; }
        public string FieldName { get; set; }

        public int WeekNumber { get; set; }

        public IEnumerable<DiaryDayVm> Days { get; set; }

        public decimal? ForecastedWeight { get; set; }
        public bool IsForecastActualValue { get; set; }

        public decimal? TotalWeight => Days
            .Select(x => x.Weight)
            .NullableSum();

        public decimal? Total => Days
            .Select(x => IsCompleted ? x.Weight : (x.Weight ?? x.Planned))
            .NullableSum();

        public bool IsMixed => TotalWeight != Total;

        public bool IsCompleted { get; set; }

        public bool IsForecastInferred { get; set; }
        public string StatsWeightMin { get; set; }
        public string StatsWeightMax { get; set; }
        public string StatsManHoursMin { get; set; }
        public string StatsManHoursMax { get; set; }
        public bool ShowStats { get; set; }
    }
}