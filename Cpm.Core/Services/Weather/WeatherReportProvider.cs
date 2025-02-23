using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Extensions;

namespace Cpm.Core.Services.Weather
{
    public class WeatherReportProvider : IWeatherReportProvider
    {
        private readonly IWeatherProvider _weatherProvider;

        public WeatherReportProvider(IWeatherProvider weatherProvider)
        {
            _weatherProvider = weatherProvider;
        }

        private async Task<ICollection<WeatherPeriodicSummary>> GetPeriodic(string postcode, CancellationToken cancellationToken)
        {
            const int periodLength = 5;

            var today = DateTime.Today;
            var firstDay = today.AddDays(-periodLength);

            var forecast = (await _weatherProvider.GetForecast(postcode, cancellationToken))
                .ToList();

            return new[] {
                CreatePeriod("Earlier 5 days", "From -10 to -6 days", false, await GetAllHistoric(postcode, firstDay.AddDays(-periodLength), periodLength, cancellationToken)),
                CreatePeriod("Past 5 days", "From -5 days until yesterday", false, await GetAllHistoric(postcode, firstDay, periodLength, cancellationToken)),
                CreatePeriod("Current 5 days", "Today and next 4 days", true, forecast.Take(periodLength)),
                CreatePeriod("Following 5 days", "From +5 to +9 days", false, forecast.Skip(periodLength).Take(periodLength)),
                CreatePeriod("Another 5 days", "From +10 to +14 days", false, forecast.Skip(periodLength*2)),
            };
        }

        private async Task<IEnumerable<WeatherDay>> GetAllHistoric(string postcode, DateTime firstDay, int days, CancellationToken cancellationToken)
        {
            var tasks = Enumerable.Range(0, days)
                .Select(x => _weatherProvider.GetHistoric(postcode, firstDay.AddDays(x), cancellationToken))
                .ToArray();

            await Task.WhenAll(tasks);

            return tasks
                .Select(x => x.Result);
        }

        private async Task<ICollection<WeatherDailySummary>> GetDaily(string postcode, CancellationToken cancellationToken)
        {
            const int daysInPast = 3;
            const int daysInFuture = 3;
            const int totalDays = daysInPast + 1 + daysInFuture;

            var today = DateTime.Today;
            var firstDay = today.AddDays(-daysInPast);

            var current = await _weatherProvider.GetCurrent(postcode, cancellationToken);

            return
                (await GetAllHistoric(postcode, firstDay, daysInPast, cancellationToken))
                .Concat(await _weatherProvider.GetForecast(postcode, cancellationToken))
                .LimitOrRightPad(totalDays, null)
                .Select((x, i) =>
                {
                    var isToday = i == daysInPast;
                    return new WeatherDailySummary
                    {
                        Summary = WeatherSummaryDay.Create(x, isToday ? current : null),
                        Day = firstDay.AddDays(i),
                        IsToday = isToday,
                        IsForecast = i > daysInPast,
                        Icon = x?.Icon ?? "/images/unknown-weather.png",
                        Description = x?.Description ?? "N/A",
                    };
                })
                .ToList();

        }

        private WeatherPeriodicSummary CreatePeriod(
            string title,
            string description,
            bool isCurrent, 
            IEnumerable<WeatherDay> dailySummaries
            )
        {
            var daily = dailySummaries.ToList();

            return new WeatherPeriodicSummary
            {
                Title = title,
                Description = description,
                IsCurrent = isCurrent,
                MinTemp = daily.IgnoreNulls().MinOrDefault(x => (double?)x.MinTemp),
                MaxTemp = daily.IgnoreNulls().MaxOrDefault(x => (double?)x.MaxTemp),
                AvgTemp = daily.IgnoreNulls().AverageOrDefault(x => (double?)(x.MinTemp + x.MaxTemp) / 2),
            };
        }

        public async Task<WeatherReport> GetReport(string postcode, CancellationToken cancellationToken)
        {
            return new WeatherReport
            {
                Daily = await GetDaily(postcode, cancellationToken),
                Periodic = await GetPeriodic(postcode, cancellationToken)
            };
        }
    }
}