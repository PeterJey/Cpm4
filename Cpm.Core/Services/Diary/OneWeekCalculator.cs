using System;
using Cpm.Core.Extensions;

namespace Cpm.Core.Services.Diary
{
    public class OneWeekCalculator : IDiaryRangeCalculator
    {
        public DiaryRange GetForPosition(DateTime firstDayOfYear, int position)
        {
            return new DiaryRange
            {
                FirstDay = firstDayOfYear.AddWeeks(position - 1),
                NumberOfWeeks = 1,
                Position = position,
            };
        }

        public int GetPositionForDate(DateTime firstDayOfYear, DateTime requestedDate)
        {
            return requestedDate.WeekNumber(firstDayOfYear);
        }
    }
}