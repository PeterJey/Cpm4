using Cpm.Core.Services.Diary;

namespace Cpm.Core.Services.Forecast
{
    public class ForecastStatistics
    {
        public int StartingWeek { get; set; }
        public WeeklyStats[] Weeks { get; set; }
    }
}