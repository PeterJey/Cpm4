using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Services.Forecast;

namespace Cpm.Core.Services.Scenarios
{
    public class ScenarioState
    {
        public IReadOnlyCollection<ForecastParameters> Parameters { get; }
        public IReadOnlyCollection<bool> Visibility { get; }

        public ScenarioState(IEnumerable<ForecastParameters> parameters, IEnumerable<bool> visibility)
        {
            Parameters = parameters.ToArray();
            Visibility = visibility.ToArray();
        }
    }
}