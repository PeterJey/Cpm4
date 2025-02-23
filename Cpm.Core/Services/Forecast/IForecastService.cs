using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Fields;

namespace Cpm.Core.Services.Forecast
{
    public interface IForecastService
    {
        Task<ForecastResult> Calculate(
            FieldDetails field, 
            ForecastParameters argParameters, 
            Func<string, ForecastResult, Task> recordFunc, 
            CancellationToken cancellationToken
            );
        IEnumerable<string> GetAvailableAlgorithms();
    }
}