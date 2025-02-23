using System.Threading.Tasks;
using Cpm.Core.Services;
using Cpm.Core.Services.Allocations;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class AllocationsController : Controller
    {
        private readonly IAllocationManager _allocationManager;
        private readonly IUserPreferences _userPreferences;
        private readonly IUserPreferencesFactory _userPreferencesFactory;
        private readonly IAllocationRepository _allocationRepository;

        public AllocationsController(
            IAllocationManager allocationManager,
            IUserPreferences userPreferences,
            IUserPreferencesFactory userPreferencesFactory,
            IAllocationRepository allocationRepository
            )
        {
            _allocationManager = allocationManager;
            _userPreferences = userPreferences;
            _userPreferencesFactory = userPreferencesFactory;
            _allocationRepository = allocationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Week(string siteId, string position = null)
        {
            if (string.IsNullOrEmpty(siteId))
            {
                return BadRequest();
            }

            if (!User.CanViewAllocationsForSite(siteId))
            {
                return Forbid();
            }

            var viewModel = int.TryParse(position, out var week)
                ? await _allocationManager.GetWeekVm(siteId, week)
                : await _allocationManager.GetWeekVm(siteId, null);

            return PartialView("_AllocationsWeek", viewModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Save(SingleAllocation allocation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!User.CanChangeAllocationsForField(allocation.FieldId))
            {
                return Forbid();
            }

            await _allocationRepository.SetAllocation(allocation);

            return Ok();
        }
    }
}