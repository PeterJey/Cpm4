using System;
using Cpm.Core.Services.Forecast;

namespace Cpm.Core.Services.Diary
{
    public class WeeklyStats
    {
        public decimal MinWeight { get; set; }
        public decimal MaxWeight { get; set; }
        public decimal? MinManHour { get; set; }
        public decimal? MaxManHour { get; set; }

        public static WeeklyStats FromForecastWeek(ForecastWeek forecastWeek)
        {
            return new WeeklyStats
            {
                MinWeight = forecastWeek.Value.Weight,
                MaxWeight = forecastWeek.Value.Weight,
                MinManHour = forecastWeek.Value.ManHour,
                MaxManHour = forecastWeek.Value.ManHour,
            };
        }

        public void UpdateFromForecastWeek(ForecastWeek forecastWeek)
        {
            MinWeight = Math.Min(MinWeight, forecastWeek.Value.Weight);
            MaxWeight = Math.Max(MaxWeight, forecastWeek.Value.Weight);
            MinManHour = Nullable.Compare(MinManHour, forecastWeek.Value.ManHour) < 0 ? MinManHour : forecastWeek.Value.ManHour;
            MaxManHour = Nullable.Compare(MaxManHour, forecastWeek.Value.ManHour) > 0 ? MaxManHour : forecastWeek.Value.ManHour;
        }
    }
}