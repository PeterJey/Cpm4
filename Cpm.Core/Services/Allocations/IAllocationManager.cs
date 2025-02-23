using System.Threading.Tasks;
using Cpm.Core.ViewModels;

namespace Cpm.Core.Services.Allocations
{
    public interface IAllocationManager
    {
        Task<AllocationVm> GetForSite(string siteId);
        Task<AllocationWeekVm> GetWeekVm(string siteId, int? week);
    }
}