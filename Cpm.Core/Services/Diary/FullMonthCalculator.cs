using System;
using Cpm.Core.Extensions;

namespace Cpm.Core.Services.Diary
{
    public class FullMonthCalculator : IDiaryRangeCalculator
    {
        public DiaryRange GetForPosition(DateTime firstDayOfYear, int position)
        {
            var firstOfMonth = GetFirstJanuaryInYearStarting(firstDayOfYear)
                .AddMonths(position - 1);

            var firstWeek = firstOfMonth.WeekNumber(firstDayOfYear);
            var lastWeek = firstOfMonth.LastOfMonth().WeekNumber(firstDayOfYear);

            return new DiaryRange
            {
                FirstDay = firstDayOfYear.AddWeeks(firstWeek - 1),
                NumberOfWeeks = lastWeek - firstWeek + 1,
                Position = position,
            };
        }

        private static DateTime GetFirstJanuaryInYearStarting(DateTime date)
        {
            return date
                .FirstOfJanuary()
                .AddYears(date.Month > 1 ? 1 : 0);
        }

        public int GetPositionForDate(DateTime firstDayOfYear, DateTime requestedDate)
        {
            var year = GetFirstJanuaryInYearStarting(firstDayOfYear).Year;
            var years = requestedDate.Year - year;
            var months = requestedDate.Month;

            return years * 12 + months;
        }
    }
}