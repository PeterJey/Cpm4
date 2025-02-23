using System.Threading.Tasks;
using Cpm.Core.Services;
using Cpm.Core.Services.Allocations;
using Cpm.Core.ViewModels;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cpm.Web.Pages.Allocations
{
    public class IndexModel : PageModel
    {
        private readonly IAllocationManager _allocationManager;
        private readonly IUserPreferences _userPreferences;

        public IndexModel(
            IAllocationManager allocationManager,
            IUserPreferences userPreferences
            )
        {
            _allocationManager = allocationManager;
            _userPreferences = userPreferences;
        }

        public async Task<IActionResult> OnGetAsync(string siteId, int? position = null)
        {
            if (!User.CanViewAllocationsForSite(siteId))
            {
                return Forbid();
            }

            ViewModel = await _allocationManager.GetForSite(siteId);

            Position = position;

            AllocationUnit = _userPreferences.AllocationUnitName;

            return Page();
        }

        public string AllocationUnit { get; set; }

        public int? Position { get; set; }

        public AllocationVm ViewModel { get; set; }
    }
}