using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Services;
using Cpm.Core.Services.Context;
using Cpm.Core.Services.Fields;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Profiles;
using Cpm.Core.ViewModels;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Optional;

namespace Cpm.Web.Controllers
{
    public class SitesController : Controller
    {
        private readonly IUserPreferences _userPreferences;
        private readonly IFieldRepository _fieldRepository;
        private readonly IScenarioWorkspaceManager _scenarioWorkspaceManager;
        private readonly IProfileRepository _profileRepository;

        public SitesController(
            IUserPreferences userPreferences,
            IFieldRepository fieldRepository,
            IScenarioWorkspaceManager scenarioWorkspaceManager,
            IProfileRepository profileRepository
            )
        {
            _userPreferences = userPreferences;
            _fieldRepository = fieldRepository;
            _scenarioWorkspaceManager = scenarioWorkspaceManager;
            _profileRepository = profileRepository;
        }

        public async Task<IActionResult> Summary(string siteId = null)
        {
            if (string.IsNullOrEmpty(siteId))
            {
                return BadRequest();
            }

            if (!User.CanViewSite(siteId))
            {
                return Forbid();
            }

            var summary = await GetSummary(siteId);

            return PartialView("_SiteSummary", summary);
        }

        private async Task<SiteFieldsSummaryVm> GetSummary(string siteId)
        {
            var site = await _fieldRepository.GetSiteById(siteId);

            var allExistingScenarioVms = (await _scenarioWorkspaceManager.GetAllScenariosForSite(siteId))
                .Prepend(new ScenarioVm
                {
                    ScenarioId = "",
                    Name = "(None)",
                    IsDeleted = false,
                })
                .ToList();

            return new SiteFieldsSummaryVm
            {
                AreaUnit = _userPreferences.AreaUnit,
                SiteId = siteId,
                Fields = site.Fields
                        .Select(field => CreateSummaryVm(field, allExistingScenarioVms))
                        .ToList(),
                ExistingProfiles = User.CanManageApplication() 
                    ? (await _profileRepository.GetByNamePattern(null))
                        .GroupBy(x => x.Name)
                        .OrderBy(x => x.Key)
                        .Select(ToProfileOptionVms)
                        .ToArray() 
                    : new ProfileOptionVm[0]
            };
        }

        private ProfileOptionVm ToProfileOptionVms(IGrouping<string, MatchedProfile> profiles)
        {
            return new ProfileOptionVm
            {
                Name = profiles.Key
            };
        }

        private FieldSummaryVm CreateSummaryVm(FieldDetails field, IEnumerable<ScenarioVm> scenarioVms)
        {
            var fieldSummaryVm = new FieldSummaryVm
            {
                FieldId = field.FieldId,
                Name = field.FieldName,
                Description = field.Description,
                Variety = field.Variety,
                Area = _userPreferences.FormatArea(field.AreaInHectares),
                Budget = _userPreferences.FormatYield(field.Budget),
                TonnesPerAcre = (field.Budget.KgPerHectare * 0.40468564224m / 1000).ToString("N2"),
                ProfileName = field.ProfileName,
                YieldPerArea = (field.Budget as YieldPerHectare)?.KgPerHectare,
                PlantsPerArea = (field.Budget as YieldPerPlant)?.PlantsPerHectare,
                YieldPerPlant = (field.Budget as YieldPerPlant)?.GramsPerPlant,
                WeightToDate = field.TotalWeight
                    .ToString("F0"),
                BudgetToDate = field.GetProgress()
                    .ToOption()
                    .Map(x => (x*100).ToString("F0") + " %")
                    .ValueOr("-"),
                FirstDay = field
                    .HarvestHistory
                    .FirstDay
                    .Map(x => x.Date.ToString("dd MMM"))
                    .ValueOr("-"),
                LastDay = field
                    .HarvestHistory
                    .LastDay
                    .Map(x => x.Date.ToString("dd MMM"))
                    .ValueOr("-"),
                NumberOfPicks = (int)field
                    .HarvestHistory
                    .NumberOfPicks,
                TotalDays = field
                    .HarvestHistory
                    .TotalDays,
                ActiveScenarioId = field.ActiveScenarioId,
                AvailableScenarioVms = scenarioVms
                    .Where(s => !s.IsDeleted || s.ScenarioId == field.ActiveScenarioId)
                    .ToList(),
            };
            return fieldSummaryVm;
        }
    }
}