using System;

namespace Cpm.Core.Services.Diary
{
    public interface IDiaryRangeCalculator
    {
        DiaryRange GetForPosition(DateTime firstDayOfYear, int position);
        int GetPositionForDate(DateTime firstDayOfYear, DateTime requestedDate);
    }
}