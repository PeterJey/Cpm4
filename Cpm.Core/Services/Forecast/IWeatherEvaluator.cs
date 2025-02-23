using System;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Profiles;

namespace Cpm.Core.Services.Forecast
{
    public interface IWeatherEvaluator
    {
        Task<EvaluationResult> EvaluateFor(
            string location, 
            SeasonsProfile seasonsProfile, 
            DateTime weekCommencing, 
            IFactorStrategy strategy,
            CancellationToken cancellationToken
            );
    }
}