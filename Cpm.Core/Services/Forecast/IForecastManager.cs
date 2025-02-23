using System.Threading.Tasks;
using Cpm.Core.Services.Context;
using Cpm.Core.ViewModels;

namespace Cpm.Core.Services.Forecast
{
    public interface IForecastManager
    {
        Task<ScenarioControlVm> GetScenarioControlVmForContext(ScenarioContext context);
        Task<GridResultsVm> GetResultsForContext(ScenarioContext context);
        Task<AlgorithmControlVm> GetAlgorithmControlVm(ScenarioContext context, uint index, uint extraWeeksBefore, uint extraWeeksAfter);
    }
}