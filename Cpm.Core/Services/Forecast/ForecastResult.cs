using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Cpm.Core.Services.Scenarios;

namespace Cpm.Core.Services.Forecast
{
    public class ForecastResult
    {
        public string AlgorithmName { get; }
        public IReadOnlyCollection<ForecastWeek> Weeks { get; }
        public ForecastQuality Quality { get; }
        public decimal Difference { get; }
        public ISet<Season> Seasons { get; }
        public IReadOnlyCollection<string> Comments { get; }

        public ForecastResult(string algorithmName, IEnumerable<ForecastWeek> forecast, ForecastQuality quality, decimal difference, ISet<Season> seasons, IEnumerable<string> comments)
        {
            AlgorithmName = algorithmName;
            Weeks = forecast.ToArray();
            Quality = quality;
            Difference = difference;
            Seasons = ImmutableHashSet.CreateRange(seasons);
            Comments = comments.ToArray();
        }
    }
}