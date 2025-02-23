using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Extensions;
using Cpm.Core.Services.Weather;
using Microsoft.Extensions.Logging;

namespace Cpm.Core.Services.Forecast
{
    public class WeeklyWeatherProvider : IWeeklyWeatherProvider
    {
        private readonly IWeatherProvider _weatherProvider;
        private readonly ILogger<WeeklyWeatherProvider> _logger;

        public WeeklyWeatherProvider(
            IWeatherProvider weatherProvider,
            ILogger<WeeklyWeatherProvider> logger
        )
        {
            _weatherProvider = weatherProvider;
            _logger = logger;
        }

        public async Task<WeeklyWeatherResult> GetForLocation(string location, DateTime currentWeekStart, CancellationToken cancellationToken)
        {
            // based on :
            // past week 0 (from historical 1-7 days to historical 7-13 days)
            // current week 1 (from 1-7 forecast to 6 historical and 1 forecast days)
            // following week 2 (between 8-15 forecast days to 2-9 forecast days)

            var today = Clock.Now.Date;

            var relativeSinceStart = (int)today.Subtract(currentWeekStart).TotalDays;

            var daysFromThePast = Math.Clamp(relativeSinceStart, 0, 3*7);
            var daysCurrentAndFromTheFuture = Math.Max(0, 3*7 - relativeSinceStart);

            if (relativeSinceStart < -6)
            {
                return Fail(
                    "Could not obtain the weather data - too far in the future",
                    "RelativeSinceStart is {relativeSinceStart} so no forecast data for next week", relativeSinceStart
                );
            }

            var forecastTask = _weatherProvider.GetForecast(location, cancellationToken);

            var historicTasks = Enumerable.Range(-relativeSinceStart, daysFromThePast)
                .Select(i => _weatherProvider
                    .GetHistoric(
                        location,
                        today.AddDays(i),
                        cancellationToken
                    )
                )
                .ToArray();

            await Task.WhenAll(historicTasks.Cast<Task>().Append(forecastTask));

            if (historicTasks.Any(t => !t.IsCompletedSuccessfully || t.Result == null))
            {
                return Fail(
                    "Could not obtain the weather data",
                    "Unable to obtain some historic data"
                );
            }
            if (forecastTask.Result.Count < 14)
            {
                return Fail(
                    "Could not obtain the weather data",
                    "Forecast days requested: 14, received: {noOfDays}", forecastTask.Result.Count
                );
            }
            if (forecastTask.Result.Any(x => x == null))
            {
                return Fail(
                    "Could not obtain the weather data",
                    "Some forecast days are null: {days}", forecastTask.Result
                );
            }

            var lastWeeksAvg = historicTasks
                .Select(x => x.Result)
                .Concat(forecastTask.Result)
                .Take(7)
                .AverageOrDefault(t => t?.AvgTemp);

            var currentWeeksAvg = historicTasks
                .Select(x => x.Result)
                .Concat(forecastTask.Result)
                .Skip(7)
                .Take(7)
                .AverageOrDefault(t => t?.AvgTemp);

            var nextWeeksAvg = historicTasks
                .Select(x => x.Result)
                .Concat(forecastTask.Result)
                .Skip(14)
                .Take(7)
                .AverageOrDefault(t => t?.AvgTemp);

            _logger.LogDebug(
                "Averages (past, current, next): {past} / {current} / {next}",
                lastWeeksAvg,
                currentWeeksAvg,
                nextWeeksAvg
            );

            var success = lastWeeksAvg.HasValue && currentWeeksAvg.HasValue && nextWeeksAvg.HasValue;

            return new WeeklyWeatherResult
            {
                // ReSharper disable PossibleInvalidOperationException
                LastWeek = (decimal)lastWeeksAvg.GetValueOrDefault(),
                CurrentWeek = (decimal)currentWeeksAvg.GetValueOrDefault(),
                NextWeek = (decimal)nextWeeksAvg.GetValueOrDefault(),
                // ReSharper restore PossibleInvalidOperationException
                Success = success,
            };
        }

        private WeeklyWeatherResult Fail(string comment, string message = null, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                _logger.LogWarning(message, args);
            }
            return WeeklyWeatherResult.Fail(comment);
        }
    }
}