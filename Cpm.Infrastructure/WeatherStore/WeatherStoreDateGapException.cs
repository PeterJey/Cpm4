using System;

namespace Cpm.Infrastructure.WeatherStore
{
    public class WeatherStoreDateGapException : Exception
    {
        public WeatherStoreDateGapException(string location, DateTime dateFrom, DateTime dateUntil) 
            : base(FormatMessage(location, dateFrom, dateUntil))
        {
            Location = location;
            DateFrom = dateFrom;
            DateUntil = dateUntil;
        }

        public static string FormatMessage(string location, DateTime dateFrom, DateTime dateUntil)
        {
            return dateFrom == dateUntil 
                ? $"Found a gap in weather stats for location \"{location}\" on {dateFrom:d}" 
                : $"Found a gap in weather stats for location \"{location}\" between {dateFrom:d} and {dateUntil:d}";
        }

        public string Location { get; }
        public DateTime DateFrom { get; }
        public DateTime DateUntil { get; }
    }
}