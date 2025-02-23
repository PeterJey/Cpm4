using System.Threading.Tasks;
using Cpm.Core.Services.Context;
using Cpm.Infrastructure.Data;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cpm.Web.Pages.Scenarios
{
    public class ControlModel : PageModel
    {
        private readonly IScenarioWorkspaceManager _scenarioWorkspaceManager;

        public ControlModel(
            IScenarioWorkspaceManager scenarioWorkspaceManager
            )
        {
            _scenarioWorkspaceManager = scenarioWorkspaceManager;
        }

        public IActionResult OnGet(string contextid = null)
        {
            if (!_scenarioWorkspaceManager.TryGet(contextid, out var context))
            {
                return new NotFoundResult();
            }

            Context = context;

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            return Page();
        }

        public ScenarioContext Context { get; set; }
    }

}