using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Extensions;

namespace Cpm.Core.Services.Forecast
{
    public class AdjustedOnlyAlgorithm : IAlgorithm
    {
        public Task<AlgorithmOutput> Calculate(AlgorithmInput input, CancellationToken cancellationToken)
        {
            var adjusted = input.Settings.GetOrDefault("AdjustedValues", "")
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => decimal.Parse(x.Trim()))
                .ToArray();

            var weekStarting = int.Parse(input.Settings.GetOrDefault("AdjustedStartingWeek", "0")) +
                               int.Parse(input.Settings.GetOrDefault("WeekOffset", "0"));

            return Task.FromResult(
                AlgorithmOutput.FromValues(weekStarting, adjusted)
                );
        }
    }
}