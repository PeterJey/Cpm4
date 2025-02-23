using System;

namespace Cpm.Infrastructure
{
    public class MemoryCachedWeatherProviderProxyOptions
    {
        public TimeSpan MaxForecastAge { get; set; } = TimeSpan.FromHours(1);
        public TimeSpan HistoricExpiration { get; set; } = TimeSpan.FromDays(5 * 365);
        public TimeSpan MaxCurrentAge { get; set; } = TimeSpan.FromMinutes(15);
    }
}