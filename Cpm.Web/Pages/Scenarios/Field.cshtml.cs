using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Services.Context;
using Cpm.Core.Services.Fields;
using Cpm.Core.Services.Forecast;
using Cpm.Infrastructure.Data;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cpm.Web.Pages.Scenarios
{
    public class FieldModel : PageModel
    {
        private readonly IScenarioWorkspaceManager _scenarioWorkspaceManager;
        private readonly IForecastManager _forecastManager;
        private readonly IFieldRepository _fieldRepository;

        public FieldModel(
            IScenarioWorkspaceManager scenarioWorkspaceManager,
            IForecastManager forecastManager,
            IFieldRepository fieldRepository
            )
        {
            _scenarioWorkspaceManager = scenarioWorkspaceManager;
            _forecastManager = forecastManager;
            _fieldRepository = fieldRepository;
        }

        public async Task<IActionResult> OnGetAsync(string contextid, uint index)
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

            if (index >= Context.FieldStates.Count)
            {
                return new BadRequestResult();
            }

            ViewModel = await _forecastManager.GetAlgorithmControlVm(context, index, 2, 3);

            // TODO: Accessing the site details just for current field name!!!
            var site = await _fieldRepository.GetSiteById(context.SiteId);

            Index = index;

            FieldName = site.Fields
                .ElementAt((int) index)
                .FieldName;

            WeekOffset = int.Parse(context.FieldStates.ElementAt((int) index).Settings["WeekOffset"]);

            return Page();
        }

        public AlgorithmControlVm ViewModel { get; set; }

        public uint Index { get; set; }

        public int WeekOffset { get; set; }

        public string FieldName { get; set; }

        public ScenarioContext Context { get; set; }
    }
}