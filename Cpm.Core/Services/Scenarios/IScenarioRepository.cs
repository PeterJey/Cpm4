using System.Threading.Tasks;

namespace Cpm.Core.Services.Scenarios
{
    public interface IScenarioRepository
    {
        Task<ScenarioState> GetScenarioState(string scenarioId);
    }
}