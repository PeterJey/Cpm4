using System;

namespace Cpm.Core.Services.Forecast
{
    public class HarvestWeek
    {
        public DateTime StartingDate { get; }
        public int WeekNumber { get; }
        public decimal Weight { get; }

        public bool IsCompleted { get; }

        public HarvestWeek(DateTime startingDate, int weekNumber, decimal weight, bool isCompleted)
        {
            StartingDate = startingDate;
            WeekNumber = weekNumber;
            Weight = weight;
            IsCompleted = isCompleted;
        }
    }
}