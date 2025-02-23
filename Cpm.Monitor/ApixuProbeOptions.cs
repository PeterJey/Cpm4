using Microsoft.Extensions.Options;

namespace Cpm.Monitor
{
    public class ApixuProbeOptions
    {
        public string Location { get; set; } = "ME13 9PU";
        public double HistoricOffsetDays { get; set; } = -1;
        public int ExpectedForecastDays { get; set; } = 14;
    }
}