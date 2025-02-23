using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Extensions;

namespace Cpm.Core.Services.Forecast
{
    public class CorrectingAlgorithm : IAlgorithm
    {
        private readonly IAlgorithm _inputAlgorithm;
        private readonly IWeatherEvaluator _weatherEvaluator;
        private readonly IFactorStrategy _factorStrategy;
        private readonly ICorrectingStrategy _correctingStrategy;

        public CorrectingAlgorithm(IAlgorithm inputAlgorithm,
            IWeatherEvaluator weatherEvaluator,
            IFactorStrategy factorStrategy,
            ICorrectingStrategy correctingStrategy)
        {
            _inputAlgorithm = inputAlgorithm;
            _weatherEvaluator = weatherEvaluator;
            _factorStrategy = factorStrategy;
            _correctingStrategy = correctingStrategy;
        }

        public async Task<AlgorithmOutput> Calculate(AlgorithmInput input, CancellationToken cancellationToken)
        {
            var result = await _inputAlgorithm.Calculate(input, cancellationToken);

            var currentWeek = Clock.Now.Date.WeekNumber(input.FirstWeekCommencing);

            var lastHarvestWeek = input.FirstWeekOfHarvest
                    .Map(fw => fw + input.HarvestWeights.Count - 1);

            var projectionWeek = lastHarvestWeek
                .Map(lhw => lhw + 1)
                .ValueOr(currentWeek);

            lastHarvestWeek
                .Filter(lhw => currentWeek < lhw)
                .MatchSome(_ => result.Comments.Add("Some harvest data was recorded in the future"));

            var evaluation = await _weatherEvaluator.EvaluateFor(
                location: input.Postcode,
                seasonsProfile: input.SeasonsProfile,
                weekCommencing: input.FirstWeekCommencing.AddWeeks(projectionWeek - 1),
                strategy: _factorStrategy,
                cancellationToken: cancellationToken
            );

            var initialDelta = result.Data.Delta;

            result.Data.Override(
                projectionWeek,
                evaluation.Factors,
                context =>
                {
                    var hv = context.Existing.ValueOr(new ForecastValue
                    {
                        Type = HarvestValueType.Forecasted,
                        PerHour = context.NearestPerHour
                    });
                    hv.Weight = Math.Max(0, _correctingStrategy.Correct(hv.Weight, context.Factor, initialDelta));
                    return hv;
                });

            result.Comments.AddIfNotEmpty(evaluation.Comment);

            return result;
        }
    }
}