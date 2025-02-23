using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Extensions;
using Cpm.Core.Services.Profiles;
using Microsoft.Extensions.Logging;

namespace Cpm.Core.Services.Forecast
{
    public class AvgTempWeatherEvaluator : IWeatherEvaluator
    {
        private readonly IWeeklyWeatherProvider _weatherProvider;
        private readonly ILogger<AvgTempWeatherEvaluator> _logger;

        public AvgTempWeatherEvaluator(
            IWeeklyWeatherProvider weatherProvider, 
            ILogger<AvgTempWeatherEvaluator> logger
            )
        {
            _weatherProvider = weatherProvider;
            _logger = logger;
        }

        public async Task<EvaluationResult> EvaluateFor(
            string location,
            SeasonsProfile seasonsProfile, 
            DateTime weekCommencing, 
            IFactorStrategy strategy,
            CancellationToken cancellationToken
            )
        {
            var today = Clock.Now.Date;

            var dayOfCurrentWeek = (int)today.Subtract(weekCommencing).TotalDays;

            var isWeekAhead = dayOfCurrentWeek.IsBetween(-7, -1);

            var weather = await _weatherProvider.GetForLocation(
                location, 
                currentWeekStart: weekCommencing.AddWeeks(isWeekAhead ? -1 : 0),
                cancellationToken: cancellationToken
                );

            if (!weather.Success)
            {
                return EvaluationResult.None(weather.Comment);
            }

            IEnumerable<decimal> averageTemperatures = new []
            {
                weather.CurrentWeek,
                weather.NextWeek
            };

            if (!isWeekAhead)
            {
                averageTemperatures = averageTemperatures.Prepend(weather.LastWeek);
            }

            var factorsResult = await strategy.CalculateFactors(
                location, 
                seasonsProfile, 
                weekCommencing.AddWeeks(isWeekAhead ? 0 : -1), 
                averageTemperatures
                );

            _logger.LogDebug(
                $"Factors: {string.Join(" / ", factorsResult.Factors.Select((x, i) => $"{{f{i+1}}}"))}",
                factorsResult.Factors.Cast<object>().ToArray()
            );

            return new EvaluationResult
            {
                Factors = factorsResult.Factors,
                Comment = factorsResult.Comment
            };
        }
    }
}