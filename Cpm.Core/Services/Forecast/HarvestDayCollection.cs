using System;
using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Extensions;
using Cpm.Core.Services.Fields;
using Optional;
using Optional.Collections;
using Optional.Unsafe;

namespace Cpm.Core.Services.Forecast
{
    public class HarvestDayCollection
    {
        public DateTime FirstWeekCommencing { get; }
        public IReadOnlyCollection<HarvestDay> Days { get; }

        public IReadOnlyCollection<HarvestWeek> Weekly { get; }

        public Option<int> StartingWeek { get; }

        public int TotalDays { get; }

        public int NumberOfPicks { get; }

        public Option<DateTime> LastDay { get; }

        public Option<DateTime> FirstDay { get; }

        public decimal TotalWeight { get; }

        public HarvestDayCollection(DateTime? firstDay, IEnumerable<decimal?> days, DateTime firstWeekCommencing)
        {
            FirstWeekCommencing = firstWeekCommencing;

            var rawDays = days.ToArray();

            Days = rawDays
                .Where(x => firstDay.HasValue)
                .Select((x, i) => x
                    .ToOption()
                    .Map(v => new HarvestDay
                        {
                            // ReSharper disable once PossibleInvalidOperationException
                            Date = firstDay.Value.Date.AddDays(i),
                            Weight = v
                        })
                    .ToReferenceOrNull()
                )
                .IgnoreNulls()
                .ToArray();

            TotalWeight = Days.Sum(x => x.Weight);

            FirstDay = firstDay
                .ToOption()
                .Filter(x => Days.Any());

            LastDay = Days
                .OrderBy(x => x.Date)
                .LastOrNone()
                .Map(x => x.Date);

            TotalDays = (int) Days
                .GetSpan(x => (long)x.Date.Subtract(firstWeekCommencing).TotalDays);

            NumberOfPicks = Days
                .Count;

            StartingWeek = FirstDay
                .Map(x => x.WeekNumber(firstWeekCommencing));

            var lastWeek = LastDay.Map(x => x.WeekNumber(firstWeekCommencing));

            var lastRecordDate = FirstDay.Map(d => d.AddDays(rawDays.Length - 1));
            var recordsInLastWeek = (int)lastRecordDate
                    .Map(ld => ld.Subtract(ld.FirstDayOfWeek(firstWeekCommencing)).TotalDays)
                    .ValueOr(0);

            var numberOfWeeks = lastWeek
                .FlatMap(lw => StartingWeek.Map(sw => lw - sw + 1))
                .ValueOr(0);

            if (numberOfWeeks <= 0)
            {
                Weekly = new HarvestWeek[0];
                return;
            }

            Weekly = Enumerable.Range(StartingWeek.ValueOrFailure(), numberOfWeeks)
                .Select(week =>
                    {
                        var dateFrom = firstWeekCommencing.AddWeeks(week - 1);
                        var dateUntil = dateFrom.AddDays(6);

                        return new HarvestWeek(
                            dateFrom,
                            week,
                            Days
                                .Where(x => x.Date.IsBetween(dateFrom, dateUntil))
                                .Sum(x => x.Weight),
                            week < lastWeek.ValueOrFailure() || recordsInLastWeek == 6
                        );
                    })
                .ToArray();
        }

        public bool CanBecomeLastHarvestDay(DateTime date)
        {
            return (Days.OrderBy(x => x.Date)
                    .LastOrDefault()
                    ?.Date
                )
                .ToOption()
                .Else(date.Some())
                .Filter(lhd => lhd <= date)
                .Map(lhd => (int)date.Subtract(FirstWeekCommencing).TotalDays % 7 != 6)
                .ValueOr(false);
        }
    }
}