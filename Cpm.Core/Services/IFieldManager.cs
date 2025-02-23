using System.Collections.Generic;
using System.Threading.Tasks;
using Cpm.Core.Models;
using Cpm.Core.Services.Fields;
using Cpm.Core.Services.Forecast;

namespace Cpm.Core.Services
{
    public interface IFieldManager
    {
        Task<Field> GetField(string fieldId);
        Task<ICollection<HarvestDay>> GetHarvestedWeight(string fieldId);
        Task<bool> UpdateActiveScenario(string fieldId, string scenarioId);
        Task<bool> UpdateDescription(string fieldId, string description);
        Task<bool> UpdateBudget(string fieldId, Yield yield);
        Task<bool> UpdateProfile(string fieldId, string profileName);
    }
}