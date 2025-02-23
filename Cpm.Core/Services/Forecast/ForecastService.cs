using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Fields;

namespace Cpm.Core.Services.Forecast
{
    public class ForecastService : IForecastService
    {
        private readonly IAlgorithmProvider _algorithmProvider;

        public ForecastService(IAlgorithmProvider algorithmProvider)
        {
            _algorithmProvider = algorithmProvider;
        }

        public async Task<ForecastResult> Calculate(FieldDetails field, ForecastParameters parameters,
            Func<string, ForecastResult, Task> recordFunc, CancellationToken cancellationToken)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            if (parameters == null)
            {
                return null;
            }

            var match = _algorithmProvider.GetAlgorithmByName(parameters.AlgorithmName);

            var input = new AlgorithmInput(field, parameters);

            var output = await match.Algorithm.Calculate(input, cancellationToken);

            var values = output.Data
                .GetWeeks()
                .ToArray();

            var startingWeek = output.Data.GetStartingWeek().ValueOr(0);

            var result = new ForecastResult(
                parameters.AlgorithmName,
                values
                    .Select((value, relativeWeek) => new ForecastWeek
                    {
                        Week = startingWeek + relativeWeek,
                        Value = value,
                    }),
                output.Quality,
                CalculateDifference(input, values.Sum(x => x.Weight)),
                output.AffectingSeasons,
                output.Comments
            );

            if (recordFunc != null)
            {
                await recordFunc(field.FieldId, result);
            }

            return result;
        }

        private decimal CalculateDifference(AlgorithmInput input, decimal total)
        {
            var baseline = input.Budget.KgPerHectare * input.AreaInHectares;
            return baseline == 0 ? 0 : total / baseline - 1;
        }

        public IEnumerable<string> GetAvailableAlgorithms() =>
            _algorithmProvider.GetAvailableAlgorithms().Select(x => x.Name);
    }
}
