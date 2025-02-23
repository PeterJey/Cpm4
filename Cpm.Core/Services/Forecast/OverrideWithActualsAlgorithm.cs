using System.Threading;
using System.Threading.Tasks;

namespace Cpm.Core.Services.Forecast
{
    public class OverrideWithActualsAlgorithm : IAlgorithm
    {
        private readonly IAlgorithm _original;

        public OverrideWithActualsAlgorithm(IAlgorithm original)
        {
            _original = original;
        }

        public async Task<AlgorithmOutput> Calculate(AlgorithmInput input, CancellationToken cancellationToken)
        {
            var orig = await _original.Calculate(input, cancellationToken);

            input.FirstWeekOfHarvest
                .MatchSome(fw =>
                {
                    orig.Data.RemoveWeeksBefore(fw);
                    orig.Data.Override(
                        fw,
                        input.HarvestWeights,
                        context =>
                        {
                            var hv = context.Existing.ValueOr(new ForecastValue
                            {
                                PerHour = context.NearestPerHour
                            });
                            hv.Type = HarvestValueType.Actual;
                            hv.Weight = context.Factor;
                            return hv;
                        });
                });

            return orig;
        }
    }
}