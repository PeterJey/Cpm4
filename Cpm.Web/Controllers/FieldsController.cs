using System.Threading.Tasks;
using Cpm.Core.Services;
using Cpm.Core.Services.Forecast;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class FieldsController : Controller
    {
        private readonly IFieldManager _fieldManager;
        private readonly IUserPreferences _userPreferences;

        public FieldsController(
            IFieldManager fieldManager,
            IUserPreferences userPreferences
            )
        {
            _fieldManager = fieldManager;
            _userPreferences = userPreferences;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActiveScenario(string fieldId = null, string scenarioId = null)
        {
            if (string.IsNullOrEmpty(fieldId))
            {
                return BadRequest();
            }

            if (!User.CanChangeActiveScenarioForField(fieldId))
            {
                return Forbid();
            }

            if (!await _fieldManager.UpdateActiveScenario(fieldId, scenarioId))
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Description(string fieldId, string description)
        {
            if (string.IsNullOrEmpty(fieldId))
            {
                return BadRequest();
            }

            if (!User.CanChangeDescriptionForField(fieldId))
            {
                return Forbid();
            }

            if (!await _fieldManager.UpdateDescription(fieldId, description))
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Budget(string fieldId, string ypp, string ppa, string ypa)
        {
            if (string.IsNullOrEmpty(fieldId))
            {
                return BadRequest();
            }

            if (!User.CanChangeBudgetForField(fieldId))
            {
                return Forbid();
            }

            if (!YieldFactory.TryCreate(_userPreferences, ypp, ppa, ypa, out var yield))
            {
                return BadRequest();
            }

            if (!await _fieldManager.UpdateBudget(fieldId, yield))
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(string fieldId, string profileName)
        {
            if (string.IsNullOrEmpty(fieldId))
            {
                return BadRequest();
            }

            if (!User.CanManageApplication())
            {
                return Forbid();
            }

            if (!await _fieldManager.UpdateProfile(fieldId, profileName))
            {
                return NotFound();
            }

            return Ok();
        }
    }
}