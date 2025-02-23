namespace Cpm.Core.Services.Forecast
{
    public class ForecastValue
    {
        public decimal Weight { get; set; }
        public decimal? PerHour { get; set; }
        public HarvestValueType Type { get; set; }

        public static ForecastValue Inferred()
        {
            return new ForecastValue
            {
                Type = HarvestValueType.Inferred
            };
        }
    }
}