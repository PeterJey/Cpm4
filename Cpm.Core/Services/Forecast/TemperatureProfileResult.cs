using System.Collections.Generic;
using System.Linq;

namespace Cpm.Core.Services.Forecast
{
    public class TemperatureProfileResult
    {
        public decimal[] AverageTemperature { get; set; }
        public bool Success { get; set; }

        private TemperatureProfileResult()
        {
        }

        public static TemperatureProfileResult NotFound = new TemperatureProfileResult
        {
            Success = false
        };

        public static TemperatureProfileResult FromProfile(IEnumerable<decimal> values)
        {
            return new TemperatureProfileResult
            {
                Success = true,
                AverageTemperature = values.ToArray()
            };
        }
    }
}