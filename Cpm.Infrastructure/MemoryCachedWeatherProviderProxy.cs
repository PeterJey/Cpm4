using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Weather;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cpm.Infrastructure
{
    public class MemoryCachedWeatherProviderProxy : IWeatherProvider
    {
        private readonly IOptions<MemoryCachedWeatherProviderProxyOptions> _options;
        private readonly IWeatherProvider _provider;
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCachedWeatherProviderProxy> _logger;

        public MemoryCachedWeatherProviderProxy(
            IOptions<MemoryCachedWeatherProviderProxyOptions> options, 
            IWeatherProvider provider,
            IMemoryCache cache,
            ILogger<MemoryCachedWeatherProviderProxy> logger)
        {
            _options = options;
            _provider = provider;
            _cache = cache;
            _logger = logger;
        }

        public Task<ICollection<WeatherDay>> GetForecast(string postcode, CancellationToken cancellationToken)
        {
            var key = $"forecast:{postcode}";

            if (_cache.TryGetValue(key, out var value))
            {
                _logger.LogInformation("Weather cache hit for: {key}", key);
                return value as Task<ICollection<WeatherDay>>;
            }
            _logger.LogInformation("Weather cache miss for: {key}", key);

            var task = _provider.GetForecast(postcode, cancellationToken)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.Result == null)
                    {
                        _logger.LogInformation("Weather cache lazy fetch failed, removing entry for: {key}", key);
                        _cache.Remove(key);
                        return new List<WeatherDay>();
                    }

                    return t.Result;
                }, cancellationToken);

            var expiration = TimeSpan.FromSeconds(
                Math.Min(
                    _options.Value.MaxForecastAge.TotalSeconds,
                    DateTime.Today.AddDays(1).Subtract(DateTime.Now).TotalSeconds
                ));

            _logger.LogInformation("Creating cache entry for: {key} expires in {expiration}", key, expiration);
            _cache.Set(key, task, expiration);

            return task;
        }

        public Task<WeatherDay> GetHistoric(string postcode, DateTime day, CancellationToken cancellationToken)
        {
            var key = $"historic:{postcode}:{day.Date:yyyy-MM-dd}";

            if (_cache.TryGetValue(key, out var value))
            {
                _logger.LogInformation("Weather cache hit for: {key}", key);
                return value as Task<WeatherDay>;
            }
            _logger.LogInformation("Weather cache miss for: {key}", key);

            var task = _provider.GetHistoric(postcode, day, cancellationToken)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.Result == null)
                    {
                        _logger.LogInformation("Weather cache lazy fetch failed, removing entry for: {key}", key);
                        _cache.Remove(key);
                        return null;
                    }

                    return t.Result;
                }, cancellationToken);

            _logger.LogInformation("Creating cache entry for: {key} expires in {expiration}", key, _options.Value.HistoricExpiration);
            _cache.Set(key, task, _options.Value.HistoricExpiration);

            return task;
        }

        public Task<WeatherNow> GetCurrent(string postcode, CancellationToken cancellationToken)
        {
            var key = $"current:{postcode}";

            if (_cache.TryGetValue(key, out var value))
            {
                _logger.LogInformation("Weather cache hit for: {key}", key);
                return value as Task<WeatherNow>;
            }
            _logger.LogInformation("Weather cache miss for: {key}", key);

            var task = _provider.GetCurrent(postcode, cancellationToken)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted || t.Result == null)
                    {
                        _logger.LogInformation("Weather cache lazy fetch failed, removing entry for: {key}", key);
                        _cache.Remove(key);
                        return null;
                    }

                    return t.Result;
                }, cancellationToken);

            var expiration = _options.Value.MaxCurrentAge;

            _logger.LogInformation("Creating cache entry for: {key} expires in {expiration}", key, expiration);
            _cache.Set(key, task, expiration);

            return task;
        }
    }
}
