using System;
using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Extensions;
using Optional;
using Optional.Collections;

namespace Cpm.Core.Services.Forecast
{
    public class ForecastData
    {
        private readonly Dictionary<int, ForecastValue> _elements;

        public ForecastData()
        {
            _elements = new Dictionary<int, ForecastValue>();
        }

        public ForecastData(int startingWeek, IEnumerable<ForecastValue> values)
        {
            _elements = values
                .Select((v, i) => new { key = startingWeek + i, val = v })
                .ToDictionary(x => x.key, x => x.val);
        }

        public decimal Override(
            int firstWeek,
            IReadOnlyCollection<decimal> factors,
            Func<OverridingForecastContext, ForecastValue> func
        )
        {
            var firstElement = _elements
                .Where(x => x.Value.PerHour.HasValue)
                .OrderBy(x => x.Key)
                .FirstOrNone();

            var lastPerHour = _elements
                .Where(x => x.Value.PerHour.HasValue)
                .OrderBy(x => x.Key)
                .LastOrDefault()
                .Value?.PerHour;

            var lastWeek = firstWeek + factors.Count - 1;

            var overriden = _elements
                .Where(x => x.Key.IsBetween(firstWeek, lastWeek))
                .Sum(x => x.Value.Weight);

            factors
                .ForEach((v, i) =>
                {
                    var week = firstWeek + i;

                    var existing = _elements.GetOrNone(week);

                    var weightBefore = existing
                        .Map(x => x.Weight)
                        .ValueOr(0);

                    var newValue = func(new OverridingForecastContext
                    {
                        Existing = existing,
                        NearestPerHour = existing
                            .Map(x => x.PerHour)
                            .ValueOr(() => firstElement
                                .Filter(fe => fe.Key > week)
                                .Map(x => x.Value.PerHour)
                                .ValueOr(() => lastPerHour)
                            ),
                        Factor = v
                    });

                    _elements[week] = newValue;

                    Delta += newValue.Weight - weightBefore;
                });

            Enumerable.Range(lastWeek + 1, Math.Max(0, firstElement.Map(x => x.Key).ValueOr(0) - lastWeek - 1))
                .ForEach(x => _elements[x] = ForecastValue.Inferred());

            return overriden;
        }

        public void RemoveWeeksBefore(int firstWeekToKeep)
        {
            var toRemove = _elements
                .Where(x => x.Key < firstWeekToKeep)
                .ToArray();

            Delta -= toRemove.Sum(x => x.Value.Weight);

            toRemove.ForEach(x => _elements.Remove(x.Key));
        }

        public decimal Delta { get; private set; }

        public Option<int> GetStartingWeek()
        {
            return _elements
                .OrderBy(x => x.Key)
                .SkipWhile(x => x.Value.Weight == 0)
                .Select(x => x.Key)
                .FirstOrNone();
        }

        public decimal TotalWeight => _elements.Sum(x => x.Value.Weight);

        public IEnumerable<HarvestValue> GetWeeks()
        {
            var firstWeek = GetStartingWeek().ValueOr(0);

            var lastWeek = _elements
                .OrderBy(x => x.Key)
                .Reverse()
                .SkipWhile(x => x.Value.Weight == 0)
                .Select(x => (int?)x.Key)
                .FirstOrDefault() ?? firstWeek - 1;

            return Enumerable.Range(firstWeek, lastWeek - firstWeek + 1)
                .Select(w => _elements
                    .GetValueOrNone(w)
                    .Map(f => new HarvestValue
                    {
                        Type = f.Type,
                        Weight = f.Weight,
                        ManHour = f.PerHour
                            .ToOption()
                            .Filter(x => x > 0)
                            .Map(x => f.Weight / x)
                            .ToNullable()
                    }))
                .SkipWhile(x => x.Map(v => v.Weight).ValueOr(0) == 0)
                .Select(x => x.ValueOr(new HarvestValue{ Type = HarvestValueType.Inferred }));
        }

        public class OverridingForecastContext
        {
            public Option<ForecastValue> Existing { get; set; }
            public decimal? NearestPerHour { get; set; }
            public decimal Factor { get; set; }
        }
    }
}