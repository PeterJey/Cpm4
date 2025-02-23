using System;

namespace Cpm.Core.Services.Forecast
{
    public class ScalingStrategy : ICorrectingStrategy
    {
        public static ScalingStrategy Instance = new ScalingStrategy();

        private ScalingStrategy()
        {
        }

        public decimal Correct(decimal value, decimal factor, decimal totalDelta)
        {
            return value * factor;
        }
    }
}