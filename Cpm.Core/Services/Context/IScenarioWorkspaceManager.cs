using System.Collections.Generic;
using System.Threading.Tasks;
using Cpm.Core.ViewModels;

namespace Cpm.Core.Services.Context
{
    public interface IScenarioWorkspaceManager
    {
        Task Discard(string contextId);
        Task<ScenarioContext> CreateScenario(string siteId);
        Task<ScenarioContext> LoadScenario(string scenarioId);
        bool TryGet(string contextId, out ScenarioContext context);
        Task<bool> Save(string contextId);
        Task Delete(string contextId);
        Task<ICollection<ScenarioVm>> GetAllScenariosForSite(string siteId);
        void Update(ScenarioContext context);
        Task<ScenarioContext> Duplicate(string contextId);
    }
}