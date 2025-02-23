using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Weather;
using Cpm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Cpm.Infrastructure.WeatherStore
{
    public class EfWeatherHistoryStore : IWeatherHistoryStore, IDisposable
    {
        public IMemoryCache Cache { get; }
        private readonly IServiceScope _scope;

        public EfWeatherHistoryStore(
            IServiceScopeFactory scopeFactory,
            IMemoryCache cache
            )
        {
            Cache = cache;
            _scope = scopeFactory.CreateScope();
        }

        public async Task AddSample(string location, WeatherNow sample, CancellationToken cancellationToken)
        {
            var day = sample.When.Date;

            var db = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var lastStat = await db.WeatherStats
                .AsTracking()
                .Where(x => x.Location == location)
                .OrderByDescending(x => x.When)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastStat != null && lastStat.When == day)
            {
                lastStat.TempMin = Math.Min(lastStat.TempMin, (decimal) sample.TempMin);
                lastStat.TempMax = Math.Max(lastStat.TempMax, (decimal) sample.TempMax);
                lastStat.SampleCount++;
                AddToLog(lastStat, sample);
            }
            else
            {
                var newStat = new WeatherStat
                {
                    When = day,
                    Location = location,
                    TempMin = (decimal) sample.TempMin,
                    TempMax = (decimal) sample.TempMax,
                    SampleCount = 1,
                };
                AddToLog(newStat, sample);
                db.WeatherStats.Add(newStat);
            }

            var key = HybridWeatherProvider.GetCurrentCacheKey(location);

            var item = Cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60);
                return sample;
            });

            await db.SaveChangesAsync(cancellationToken);

            if (lastStat != null && lastStat.When < day.Date.AddDays(-1))
            {
                throw new WeatherStoreDateGapException(location, lastStat.When.AddDays(1),
                    DateTime.UtcNow.Date.AddDays(-1));
            }
        }

        private void AddToLog(WeatherStat stat, WeatherNow sample)
        {
            var items = JsonConvert.DeserializeObject<StatLogItem[]>(stat.Log ?? "[]");
            stat.Log = JsonConvert.SerializeObject(
                items.Append(
                    new StatLogItem
                    {
                        When = sample.When,
                        TempMin = sample.TempMin,
                        TempMax = sample.TempMax,
                    })
                );
        }

        public class StatLogItem
        {
            public DateTime When { get; set; }
            public double TempMin { get; set; }
            public double TempMax { get; set; }
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}