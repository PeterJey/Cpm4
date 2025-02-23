using System;
using System.Globalization;

namespace Cpm4.UnitTests.FullMonthCalculatorTests
{
    public static class Helpers
    {
        public static DateTime ToDate(string dateString)
        {
            return DateTime.ParseExact(dateString, "yyyy-MM-dd", new CultureInfo("en-GB"));
        }
    }
}
