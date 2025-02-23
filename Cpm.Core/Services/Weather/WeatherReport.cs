using System.Collections.Generic;

namespace Cpm.Core.Services.Weather
{
    public class WeatherReport
    {
        public ICollection<WeatherPeriodicSummary> Periodic { get; set; }
        public ICollection<WeatherDailySummary> Daily { get; set; }
    }
}