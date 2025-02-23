using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cpm.Core.Services.Forecast
{
    public class BudgetGuardingAlgorithm : IAlgorithm
    {
        private const decimal MaxDeltaKg = 10; //kg

        private readonly CorrectingAlgorithm _algorithm;

        public BudgetGuardingAlgorithm(CorrectingAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

        public async Task<AlgorithmOutput> Calculate(AlgorithmInput input, CancellationToken cancellationToken)
        {
            var result = await _algorithm.Calculate(input, cancellationToken);

            if (Math.Abs(result.Data.Delta) > MaxDeltaKg)
            {
                result.Comments.Add("Unable to stay on budget");
            }

            return result;
        }
    }
}