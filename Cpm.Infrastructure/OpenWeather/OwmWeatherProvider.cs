using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Weather;
using Microsoft.Extensions.Options;

namespace Cpm.Infrastructure.OpenWeather
{
    public class OwmWeatherProvider : IWeatherProvider
    {
        private readonly OpenWeatherMap _service;
        public IOptions<OwmWeatherProviderOptions> Options { get; }

        public OwmWeatherProvider(IOptions<OwmWeatherProviderOptions> options)
        {
            Options = options;
            _service = new OpenWeatherMap(Options.Value.ApiKey);
        }

        public async Task<ICollection<WeatherDay>> GetForecast(string postcode, CancellationToken cancellationToken)
        {
            if (!TrySplitLatLon(postcode, out var lat, out var lon))
            {
                throw new ArgumentException("Expected lat,lon");
            }

            var data = await _service.GetForecast(lat, lon, cancellationToken);

            return data.list
                .Select(day => new WeatherDay
                {
                    Description = CollateDescriptions(day.weather),
                    Icon = null,
                    MinTemp = day.temp.min,
                    MaxTemp = day.temp.max,
                    AvgTemp = (day.temp.min + day.temp.max)/2,
                })
                .ToArray();
        }

        public Task<WeatherDay> GetHistoric(string postcode, DateTime day, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<WeatherNow> GetCurrent(string postcode, CancellationToken cancellationToken)
        {
            if (!TrySplitLatLon(postcode, out var lat, out var lon))
            {
                throw new ArgumentException("Expected lat,lon");
            }

            var data = await _service.GetCurrent(lat, lon, cancellationToken);

            return new WeatherNow
            {
                Description = CollateDescriptions(data.weather),
                Icon = null,
                TempMin = data.main.temp_min,
                TempMax = data.main.temp_max,
                When = DateTime.UtcNow
            };
        }

        private static string CollateDescriptions(IEnumerable<Weather> conditions)
        {
            return string.Join(", ", conditions.Select(x => x.description));
        }

        private bool TrySplitLatLon(string postcode, out double lat, out double lon)
        {
            lat = 0;
            lon = 0;

            var parts = postcode.Split(",");

            if (parts.Length != 2) return false;

            if (!double.TryParse(parts[0], out lat)) return false;

            if (!double.TryParse(parts[1], out lon)) return false;

            return true;
        }
    }
}
