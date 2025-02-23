namespace Cpm.Core.Services.Forecast
{
    public class WeeklyWeatherResult
    {
        public decimal CurrentWeek { get; set; }
        public decimal LastWeek { get; set; }
        public decimal NextWeek { get; set; }
        public string Comment { get; set; }
        public bool Success { get; set; }

        public static WeeklyWeatherResult Fail(string comment)
        {
            return new WeeklyWeatherResult
            {
                Success = false,
                Comment = comment
            };
        }
    }
}