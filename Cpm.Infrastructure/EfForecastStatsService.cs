using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Extensions;
using Cpm.Core.Models;
using Cpm.Core.Services.Diary;
using Cpm.Core.Services.Forecast;
using Cpm.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Cpm.Infrastructure
{
    public class EfForecastStatsService : IForecastStatsService
    {
        private readonly ApplicationDbContext _dbContext;
        private Action _onDispose;
        private readonly IMemoryCache _cache;
        private static readonly SemaphoreSlim DbSemaphore = new SemaphoreSlim(1);

        public EfForecastStatsService(ApplicationDbContext dbContext, Action onDispose, IMemoryCache cache)
        {
            _dbContext = dbContext;
            _onDispose = onDispose;
            _cache = cache;
        }

        public async Task Update(string scenarioId, string fieldId, ForecastResult forecastResult)
        {
            await DbSemaphore.WaitAsync();

            var stats = await FetchStats(scenarioId, fieldId, forecastResult.AlgorithmName);

            var now = DateTime.UtcNow;

            var key = new {scenarioId, fieldId, forecastResult.AlgorithmName};

            if (stats == null)
            {
                stats = new ForecastStat
                {
                    ScenarioId = scenarioId,
                    FieldId = fieldId,
                    AlgorithmName = forecastResult.AlgorithmName,
                    Created = now,
                    SerializedStats = "[]"
                };

                //try
                //{

                    _dbContext.ForecastStats.Add(stats);
                //}
                //catch (DbUpdateException)
                //{
                //    //_dbContext.ForecastStats.Update(stats);
                //}
            }
            else
            {
                if (_dbContext.ChangeTracker.Entries<ForecastStat>().ToArray()
                    .All(x => x.Entity.Id != stats.Id))
                {
                    _dbContext.Entry(stats).State = EntityState.Unchanged;
                }

                //_dbContext.ForecastStats.Update(stats);
                //_dbContext.Entry(stats).State = EntityState.Modified;
            }

            var entry = _cache.CreateEntry(key);
            entry.Value = stats;

            stats.SampleCount++;
            stats.Modified = now;

            var replaceStats = forecastResult.Weeks.Any() && forecastResult.Weeks.All(x => x.Value.Type != HarvestValueType.Actual);

            var statWeeks = replaceStats
                ? new Dictionary<int, WeeklyStats>()
                : DeserializeStatsWeeks(stats);

            foreach (var forecastWeek in forecastResult.Weeks)
            {
                if (forecastWeek.Value.Type == HarvestValueType.Actual) continue;

                if (statWeeks.ContainsKey(forecastWeek.Week))
                {
                    statWeeks[forecastWeek.Week].UpdateFromForecastWeek(forecastWeek);
                }
                else
                {
                    statWeeks.Add(
                        forecastWeek.Week,
                        WeeklyStats.FromForecastWeek(forecastWeek)
                    );
                }
            }

            if (statWeeks.Any())
            {
                var firstWeek = statWeeks.Keys.Min();
                var weeks = statWeeks.Keys.Max() - firstWeek + 1;

                stats.SerializedStats = JsonConvert.SerializeObject(
                    Enumerable.Range(firstWeek, weeks)
                        .Select(week => statWeeks.GetValueOrDefault(week, null))
                        .ToArray()
                );
                stats.StartingWeek = firstWeek;
            }
            else
            {
                stats.SerializedStats = "[]";
                stats.StartingWeek = 0;
            }

            DbSemaphore.Release();
        }

        private async Task<ForecastStat> FetchStats(string scenarioId, string fieldId, string algorithmName)
        {
            var key = new { scenarioId, fieldId, algorithmName };

            var stats = await _cache.GetOrCreateAsync(
                key,
                async entry =>
                {
                    var fromDb = await EagerlyFetchStats(scenarioId, fieldId, algorithmName);
                    entry.Value = fromDb;
                    return fromDb;
                });

            return stats;
        }

        private async Task<ForecastStat> EagerlyFetchStats(string scenarioId, string fieldId, string algorithmName)
        {
            var fromDb = await _dbContext.ForecastStats
                .AsNoTracking()
                .Where(x => x.ScenarioId == scenarioId)
                .ToArrayAsync();

            foreach (var forecastStat in fromDb)
            {
                var key = new { forecastStat.ScenarioId, forecastStat.FieldId, forecastStat.AlgorithmName };
                var entry = _cache.CreateEntry(key);
                entry.Value = forecastStat;
            }

            //var toAttach = fromDb
            //    .GroupJoin(
            //        _dbContext.ChangeTracker.Entries<ForecastStat>().ToArray(),
            //        outer => outer.Id,
            //        inner => inner.Entity.Id,
            //        (stat, entries) => entries.Any() ? null : stat
            //    );

            //foreach (var forecastStat in toAttach)
            //{
            //    _dbContext.Entry(forecastStat).State = EntityState.Unchanged;
            //}

            return fromDb.SingleOrDefault(x =>
                    x.FieldId == fieldId && 
                    x.ScenarioId == scenarioId &&
                    x.AlgorithmName == algorithmName);
        }

        private static Dictionary<int, WeeklyStats> DeserializeStatsWeeks(ForecastStat stats)
        {
            return JsonConvert.DeserializeObject<WeeklyStats[]>(stats.SerializedStats)
                .Select((v, i) => new {v, i})
                .Where(x => x.v != null)
                .ToDictionary(k => k.i + stats.StartingWeek, v => v.v);
        }

        public async Task Save()
        {
            await DbSemaphore.WaitAsync();

            try
            {
                await _dbContext.SaveChangesAsync();

            }
            catch (DbUpdateException)
            {
                //ignore
            }

            foreach (var entityEntry in _dbContext.ChangeTracker.Entries().ToArray())
            {
                entityEntry.State = EntityState.Detached;
            }

            DbSemaphore.Release();

            //    foreach (var o in _toSave)
            //    {
            //        _cache.Remove(o);
            //    }
            //}
            //finally
            //{
            //    _toSave.Clear();
            //}

            //return Task.CompletedTask;
        }

        public async Task<ForecastStatistics> GetStats(string scenarioId, string fieldId, string algorithmName)
        {
            await DbSemaphore.WaitAsync();

            var stats = await FetchStats(scenarioId, fieldId, algorithmName);

            DbSemaphore.Release();


            if (stats == null) return null;
                
            var weeks = DeserializeStatsWeeks(stats) ;
                
            return weeks.Any() 
                ? new ForecastStatistics
                {
                    StartingWeek = stats.StartingWeek,
                    Weeks = weeks
                        .Select(x => x.Value)
                        .ToArray()
                }
                :null;
        }

        public void Dispose()
        {
            var action = _onDispose;
            _onDispose = null;
            action?.Invoke();
        }
    }
}