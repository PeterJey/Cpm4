using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Weather;
using Cpm.Infrastructure;
using Cpm.Infrastructure.Data;
using Cpm.Infrastructure.WeatherStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cpm.Web.Services.JobScheduling
{
    public class HybridWeatherRecordingJobFactory : IJobFactory, IDisposable
    {
        public IWeatherProvider WeatherProvider { get; }
        public IWeatherHistoryStore WeatherStore { get; }
        public IMemoryCache Cache { get; }
        public ILogger<HybridWeatherRecordingJobFactory> Logger { get; }
        private readonly IServiceScope _scope;
        private bool _firstRun = true;

        public HybridWeatherRecordingJobFactory(
            IServiceScopeFactory scopeFactory,
            IWeatherProvider weatherProvider,
            IWeatherHistoryStore weatherStore,
            IMemoryCache cache,
            ILogger<HybridWeatherRecordingJobFactory> logger
            )
        {
            WeatherProvider = weatherProvider;
            WeatherStore = weatherStore;
            Cache = cache;
            Logger = logger;
            _scope = scopeFactory.CreateScope();
        }

        public async Task<IEnumerable<Job>> CreateJobs(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Creating jobs based on site locations");

            var db = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            return (await db.Sites
                .Select(x => x.Postcode)
                .Distinct()
                .ToArrayAsync(cancellationToken)
                )
                .SelectMany(CreateJobsForLocation);
        }

        private IEnumerable<Job> CreateJobsForLocation(string location)
        {
            return new[]
            {
                new Job(ct => CheckForGaps()),
                new Job(ct => UpdateCurrent(location, ct)),
                new Job(ct => UpdateForecast(location, ct)),
            };
        }

        private async Task<JobResult> CheckForGaps()
        {
            if (_firstRun)
            {
                var db = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var groups = await db.WeatherStats
                    .Where(x => x.When >= DateTime.UtcNow.Date.AddYears(-1))
                    .AsAsyncEnumerable()
                    .GroupBy(x => x.Location)
                    .Select(x => new { location = x.Key, gaps = CheckContinuity(x.ToArray().Result) })
                    .Where(x => x.gaps.Any())
                    .ToArray();

                foreach (var g in groups)
                {
                    foreach (var gap in g.gaps)
                    {
                        Logger.LogWarning(
                            WeatherStoreDateGapException.FormatMessage(g.location, gap.Item1, gap.Item2)
                            );
                    }
                }
            }

            _firstRun = false;
            return new JobResult(true);
        }

        private IReadOnlyCollection<Tuple<DateTime, DateTime>> CheckContinuity(IEnumerable<WeatherStat> stats)
        {
            using (var e = stats.OrderBy(x => x.When).GetEnumerator())
            {
                if (!e.MoveNext()) return null;

                var prev = e.Current;

                var gaps = new List<Tuple<DateTime, DateTime>>();

                while (e.MoveNext())
                {
                    var cur = e.Current;

                    var nextDay = prev.When.AddDays(1);

                    if (nextDay < cur.When)
                    {
                        gaps.Add(Tuple.Create(nextDay, cur.When.AddDays(-1)));
                    }

                    prev = cur;
                }

                return gaps;
            }
        }

        private async Task<JobResult> UpdateCurrent(string location, CancellationToken cancellationToken)
        {
            Logger.LogInformation("UpdateCurrent({0}) started", location);
            var current = await WeatherProvider.GetCurrent(location, cancellationToken);

            if (current == null)
            {
                Logger.LogInformation("UpdateCurrent({0}) failed", location);
                return new JobResult(false);
            }

            try
            {
                await WeatherStore.AddSample(location, current, cancellationToken);
            }
            catch (WeatherStoreDateGapException ex)
            {
                Logger.LogWarning(ex.Message);
            }

            Logger.LogInformation("UpdateCurrent({0}) succeeded", location);
            return new JobResult(true);
        }

        private async Task<JobResult> UpdateForecast(string location, CancellationToken cancellationToken)
        {
            Logger.LogInformation("UpdateForecast({0}) started", location);
            var forecast = await WeatherProvider.GetForecast(location, cancellationToken);

            if (forecast == null)
            {
                Logger.LogInformation("UpdateForecast({0}) failed", location);
                return new JobResult(false);
            }

            var key = HybridWeatherProvider.GetForecastCacheKey(location);

            var item = Cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpiration = GetNextMidnight();
                return forecast;
            });

            Logger.LogInformation("UpdateForecast({0}) succeeded", location);
            return new JobResult(true);
        }

        private static DateTime GetNextMidnight()
        {
            return DateTime.UtcNow.Date.AddDays(1);
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}