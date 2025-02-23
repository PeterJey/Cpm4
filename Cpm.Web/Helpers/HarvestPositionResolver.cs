using System;
using Cpm.Core.Services.Diary;
using Cpm.Core.Services.Fields;
using Optional;

namespace Cpm.Web.Helpers
{
    public class HarvestPositionResolver : ICalendarPositionResolver
    {
        private readonly IDiaryRangeCalculator _calculator;
        private readonly ICalendarPositionResolver _next;

        public HarvestPositionResolver(IDiaryRangeCalculator calculator, ICalendarPositionResolver next)
        {
            _calculator = calculator;
            _next = next;
        }

        public Option<int> Resolve(FieldDetails field, string which)
        {
            return (
                    !string.IsNullOrEmpty(which)
                        ? which.Equals("firstharvest", StringComparison.OrdinalIgnoreCase)
                            ? field.HarvestHistory.FirstDay
                            : which.Equals("lastharvest", StringComparison.OrdinalIgnoreCase)
                                ? field.HarvestHistory.LastDay
                                : Option.None<DateTime>()
                        : Option.None<DateTime>()
                )
                .Map(date => _calculator.GetPositionForDate(field.FirstWeekCommencing, date))
                .Else(_next.Resolve(field, which));
        }
    }
}