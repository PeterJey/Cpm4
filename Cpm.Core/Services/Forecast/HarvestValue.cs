namespace Cpm.Core.Services.Forecast
{
    public class HarvestValue
    {
        public decimal Weight { get; set; }
        public decimal? ManHour { get; set; }
        public HarvestValueType Type { get; set; }
        public string Comment { get; set; }
    }
}