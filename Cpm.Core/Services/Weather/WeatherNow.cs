using System;

namespace Cpm.Core.Services.Weather
{
    public class WeatherNow
    {
        public string Icon { get; set; }
        public string Description { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public DateTime When { get; set; }
    }
}