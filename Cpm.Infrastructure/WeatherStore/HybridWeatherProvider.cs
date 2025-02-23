using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Weather;
using Cpm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Cpm.Infrastructure.WeatherStore
{
    public class HybridWeatherProvider : IWeatherProvider
    {
        public ApplicationDbContext DbContext { get; }
        public IMemoryCache Cache { get; }
        public ILogger<HybridWeatherProvider> Logger { get; }

        public HybridWeatherProvider(
            ApplicationDbContext dbContext,
            IMemoryCache cache,
            ILogger<HybridWeatherProvider> logger
            )
        {
            DbContext = dbContext;
            Cache = cache;
            Logger = logger;
        }

        public static string GetForecastCacheKey(string location)
        {
            return $"forecast-{location}";
        }

        public static string GetCurrentCacheKey(string location)
        {
            return $"current-{location}";
        }

        public static string GetHistoricCacheKey(string location, DateTime date)
        {
            return $"historic-{location}-{date:yyyy-MM-dd}";
        }

        public Task<ICollection<WeatherDay>> GetForecast(string postcode, CancellationToken cancellationToken)
        {
            Logger.LogInformation("GetForecast: {0}", postcode);
            return Task.FromResult(
                (Cache.TryGetValue(GetForecastCacheKey(postcode), out var forecast)
                    ? forecast
                    : new WeatherDay[] {}
                ) as ICollection<WeatherDay>
                );
        }

        public Task<WeatherDay> GetHistoric(string postcode, DateTime day, CancellationToken cancellationToken)
        {
            Logger.LogInformation("GetHistoric: {0} - {1}", postcode, day);

            return Cache.GetOrCreate(
                GetHistoricCacheKey(postcode, day), 
                entry =>
                {
                    Logger.LogInformation("GetHistoric: {0} - {1} Cache missed", postcode, day);
                    return DbContext.WeatherStats
                        .AsNoTracking()
                        .Where(x => x.When == day.Date && x.Location == postcode)
                        .Select(x => ToWeatherDay(x))
                        .SingleOrDefaultAsync(cancellationToken);
                });
        }

        private static WeatherDay ToWeatherDay(WeatherStat stat)
        {
            // using convert instead of casting as otherwise EF throws an exception
            // https://github.com/aspnet/EntityFrameworkCore/issues/13587

            return new WeatherDay
            {
                MinTemp = Convert.ToDouble(stat.TempMin),
                MaxTemp = Convert.ToDouble(stat.TempMax),
                AvgTemp = Convert.ToDouble((stat.TempMin + stat.TempMax)/2)
            };
        }

        public Task<WeatherNow> GetCurrent(string postcode, CancellationToken cancellationToken)
        {
            Logger.LogInformation("GetCurrent: {0}", postcode);

            return Task.FromResult(
                Cache.TryGetValue(GetCurrentCacheKey(postcode), out WeatherNow current) 
                ? current 
                : null
                );
        }
    }
}