using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Extensions;
using Cpm.Core.Services.Profiles;
using Microsoft.Extensions.Logging;

namespace Cpm.Core.Services.Forecast
{
    public class GdhDifferenceStrategy : IFactorStrategy
    {
        private readonly ITemperatureProfileRepository _profileRepository;
        private readonly ILogger<GdhDifferenceStrategy> _logger;

        public GdhDifferenceStrategy(
            ITemperatureProfileRepository profileRepository,
            ILogger<GdhDifferenceStrategy> logger)
        {
            _profileRepository = profileRepository;
            _logger = logger;
        }

        public async Task<FactorsResult> CalculateFactors(
            string location,
            SeasonsProfile seasonsProfile,
            DateTime weekStarting, 
            IEnumerable<decimal> averageTemperatures
            )
        {
            var result = await _profileRepository.FindProfile(location, seasonsProfile);

            if (!result.Success)
            {
                return Fail("Missing temperature profile", "Could not find matching temperature profile");
            }

            if (result.AverageTemperature.Length != 12)
            {
                return Fail("Missing temperature profile", "Temperature profile has wrong length");
            }

            return new FactorsResult
            {
                Factors = averageTemperatures
                    .Skip(1) // past week, we adjust only from current onwards
                    .Select((av, i) => CalculateFactorFor(
                        weekStarting.AddWeeks(i),
                        av,
                        result.AverageTemperature)
                    )
                    .ToArray(),
            };
        }

        private decimal CalculateFactorFor(DateTime firstDay, decimal actual, decimal[] profile)
        {
            var expected = Lookup(firstDay, profile);

            var actualGdh = GdhPerWeek(actual);
            var expectedGdh = GdhPerWeek(expected);

            _logger.LogDebug("Factors: actual {actual} C, {actualGdh} GDH/w expected {expected} C, {expectedGdh} GDH/w",
                actual, actualGdh, expected, expectedGdh);

            return Proportion(actualGdh, expectedGdh);
        }

        private decimal Proportion(decimal value, decimal baseline)
        {
            return baseline == 0
                ? 1
                : value / baseline;
        }

        private decimal GdhPerWeek(decimal temperature)
        {
            return Math.Max(0, temperature - 4.5m)*24*7;
        }

        private decimal Lookup(DateTime firstDay, decimal[] profile)
        {
            return profile[firstDay.Month - 1];
        }

        private FactorsResult Fail(string comment, string message)
        {
            _logger.LogWarning(message);
            return new FactorsResult
            {
                Factors = new decimal[0],
                Comment = comment
            };
        }
    }
}