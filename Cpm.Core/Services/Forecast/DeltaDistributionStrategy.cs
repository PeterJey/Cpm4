using System;

namespace Cpm.Core.Services.Forecast
{
    public class DeltaDistributionStrategy : ICorrectingStrategy
    {
        public static DeltaDistributionStrategy Instance = new DeltaDistributionStrategy();

        private DeltaDistributionStrategy()
        {
        }

        public decimal Correct(decimal value, decimal factor, decimal totalDelta)
        {
            return value - factor * totalDelta;
        }
    }
}