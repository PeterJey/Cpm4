using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Services.Profiles;

namespace Cpm.Core.Services.Forecast
{
    public class LookupStrategy : IFactorStrategy
    {
        public static readonly LookupStrategy Instance = new LookupStrategy();

        private LookupStrategy()
        {
        }

        public Task<FactorsResult> CalculateFactors(
            string location,
            SeasonsProfile seasonsProfile,
            DateTime weekStarting, 
            IEnumerable<decimal> averageTemperatures
            )
        {
            return Task.FromResult(
                new FactorsResult {
                    Factors = DoLookups(
                        CalculateDeltas(averageTemperatures)
                                )
                            .ToArray()
                });
        }

        private IEnumerable<decimal> DoLookups(IEnumerable<decimal> deltas)
        {
            var remaining = 1m;

            using (var enumerator = deltas.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var factor = remaining*Lookup(enumerator.Current);
                    remaining -= factor;
                    yield return factor;
                }
            }
            yield return remaining;
        }

        private IEnumerable<decimal> CalculateDeltas(IEnumerable<decimal> temperatures)
        {
            using (var enumerator = temperatures.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    yield break;
                }
                var previous = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current - previous;
                    previous = enumerator.Current;
                }
            }
        }

        private static readonly IReadOnlyDictionary<decimal, decimal> Factors = new Dictionary<decimal, decimal>
        {
            { -20, 0 },
            { -15, 0.2m },
            { -10, 0.3m },
            { -5, 0.45m },
            { 0, 0.5m },
            { 5, 0.55m },
            { 10, 0.7m },
            { 15, 0.8m },
            { 20, 1 }
        };

        private static decimal Lookup(decimal delta)
        {
            var lowerKey = Factors.Keys
                .Cast<decimal?>()
                .LastOrDefault(x => x < delta);

            var upperKey = Factors.Keys
                .Cast<decimal?>()
                .FirstOrDefault(x => x < delta);

            if (!lowerKey.HasValue) return Factors[upperKey.Value];
            if (!upperKey.HasValue) return Factors[lowerKey.Value];

            var lowerValue = Factors[lowerKey.Value];
            var upperValue = Factors[upperKey.Value];

            return Interpolate(delta, lowerKey.Value, upperKey.Value, lowerValue, upperValue);
        }

        public static decimal Interpolate(decimal x, decimal x0, decimal x1, decimal y0, decimal y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }
    }
}