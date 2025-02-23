using Cpm.Core;
using Cpm.Core.Services.Diary;
using Cpm.Core.Services.Fields;
using Optional;

namespace Cpm.Web.Helpers
{
    public class UseTodayResolver : ICalendarPositionResolver
    {
        private readonly IDiaryRangeCalculator _calculator;

        public UseTodayResolver(IDiaryRangeCalculator calculator)
        {
            _calculator = calculator;
        }

        public Option<int> Resolve(FieldDetails field, string which)
        {
            return _calculator
                .GetPositionForDate(
                    field.FirstWeekCommencing, 
                    Clock.Now.Date
                )
                .Some();
        }
    }
}