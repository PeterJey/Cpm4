using System;

namespace Cpm.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FirstOfJanuary(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        public static DateTime FirstOfMonth(this DateTime date)
        {
            return date.Date.AddDays(-date.Day + 1);
        }

        public static DateTime LastOfMonth(this DateTime date)
        {
            return date.FirstOfMonth().AddMonths(1).AddDays(- 1);
        }

        public static DateTime FirstDayOfWeek(this DateTime me, DateTime firstWeekCommencing)
        {
            return me.Date.AddDays(-((me.Date - firstWeekCommencing.Date).TotalDays % 7));
        }

        public static int WeekNumber(this DateTime me, DateTime firstWeekCommencing)
        {
            return (int)Math.Floor(me.Date.Subtract(firstWeekCommencing.Date).TotalDays / 7) + 1;
        }

        public static DateTime AddWeeks(this DateTime date, int weeks)
        {
            return date.Date.AddDays(weeks * 7);
        }
    }
}
