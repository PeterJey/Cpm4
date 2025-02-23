using System;
using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Services.Profiles;
using Microsoft.Extensions.Logging;

namespace Cpm.Core.Services.Forecast
{
    public class HardcodedAlgorithmProvider : IAlgorithmProvider
    {
        private readonly AlgorithmDetails[] _algorithms;

        public HardcodedAlgorithmProvider(
            IProfileRepository profileRepository,
            IWeatherEvaluator weatherEvaluator,
            GdhDifferenceStrategy gdhStrategy
        )
        {
            var originalAlgorithm = new OriginalAlgorithm(profileRepository);

            var forecastingAlgorithm = new OverrideWithActualsAlgorithm(
                new OriginalAlgorithm(profileRepository)
                );

            var projectingAlgorithm = new BudgetGuardingAlgorithm(
                new CorrectingAlgorithm(
                    forecastingAlgorithm, 
                    weatherEvaluator,
                    LookupStrategy.Instance,
                    DeltaDistributionStrategy.Instance
                )
            );

            var reactiveAlgorithm = new CorrectingAlgorithm(
                forecastingAlgorithm, 
                weatherEvaluator,
                gdhStrategy,
                ScalingStrategy.Instance
            );

            var adjustedAlgorithm = new OverrideWithActualsAlgorithm(new AdjustedOnlyAlgorithm());

            _algorithms = new[]
            {
                new AlgorithmDetails("Original", "", originalAlgorithm),
                new AlgorithmDetails("Static", "", forecastingAlgorithm),
                new AlgorithmDetails("Projecting", "", projectingAlgorithm),
                new AlgorithmDetails("Reactive", "", reactiveAlgorithm),
                new AlgorithmDetails("Adjusted", "", adjustedAlgorithm),
            };
        }

        public IEnumerable<AlgorithmDetails> GetAvailableAlgorithms()
        {
            return _algorithms;
        }

        public AlgorithmDetails GetAlgorithmByName(string name)
        {
            return _algorithms.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}