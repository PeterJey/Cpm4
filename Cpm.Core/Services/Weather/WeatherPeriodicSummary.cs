namespace Cpm.Core.Services.Weather
{
    public class WeatherPeriodicSummary
    {
        public string Title { get; set; }
        public bool IsCurrent { get; set; }
        public double? MinTemp { get; set; }
        public double? MaxTemp { get; set; }
        public double? AvgTemp { get; set; }
        public string Description { get; set; }
    }
}