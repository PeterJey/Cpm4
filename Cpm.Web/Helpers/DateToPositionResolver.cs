using System;
using System.Globalization;
using Cpm.Core.Services.Diary;
using Cpm.Core.Services.Fields;
using Optional;

namespace Cpm.Web.Helpers
{
    public class DateToPositionResolver : ICalendarPositionResolver
    {
        private readonly IDiaryRangeCalculator _calculator;
        private readonly ICalendarPositionResolver _next;

        public DateToPositionResolver(IDiaryRangeCalculator calculator, ICalendarPositionResolver next)
        {
            _calculator = calculator;
            _next = next;
        }

        public Option<int> Resolve(FieldDetails field, string which)
        {
            if (DateTime.TryParseExact(which, "yyyy-MM-dd", null, DateTimeStyles.None,
                out var requestedDay))
            {
                return _calculator
                    .GetPositionForDate(field.FirstWeekCommencing, requestedDay)
                    .Some();
            }

            return _next.Resolve(field, which);
        }
    }
}