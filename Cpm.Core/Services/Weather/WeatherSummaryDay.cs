namespace Cpm.Core.Services.Weather
{
    public class WeatherSummaryDay
    {
        private WeatherSummaryDay()
        {
        }

        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }

        public static WeatherSummaryDay Create(WeatherDay summary, WeatherNow now)
        {
            return summary == null
               ? null
                : new WeatherSummaryDay
                {
                    MaxTemp = summary.MaxTemp,
                    MinTemp = summary.MinTemp,
                    Icon = now != null ? now.Icon : summary.Icon,
                    Description = now != null ? now.Description : summary.Description,
                };
        }
    }
}