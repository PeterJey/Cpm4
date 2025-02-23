using System;

namespace Cpm.Core.Services.Weather
{
    public class WeatherDailySummary
    {
        public DateTime Day { get; set; }
        public bool IsToday { get; set; }
        public bool IsForecast { get; set; }
        public WeatherSummaryDay Summary { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
    }
}