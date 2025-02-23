using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cpm.Core.Services.Weather
{
    public interface IWeatherProvider
    {
        Task<ICollection<WeatherDay>> GetForecast(string postcode, CancellationToken cancellationToken);
        Task<WeatherDay> GetHistoric(string postcode, DateTime day, CancellationToken cancellationToken);
        Task<WeatherNow> GetCurrent(string postcode, CancellationToken cancellationToken);
    }
}