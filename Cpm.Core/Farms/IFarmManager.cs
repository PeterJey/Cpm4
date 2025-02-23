using System.Collections.Generic;
using System.Threading.Tasks;
using Cpm.Core.Models;

namespace Cpm.Core.Farms
{
    public interface IFarmManager
    {
        Task CreateNewFarm(FarmModel model);
        Task<ICollection<Farm>> GetAllFarms();
        Task<ICollection<Site>> GetAllSites();
    }
}