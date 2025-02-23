using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Farms;
using Cpm.Core.Models;
using Cpm.Core.Services.Context;
using Cpm.Core.ViewModels;
using Cpm.Web.PageHelpers;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Pages
{
    public class DashboardModel : StatusAwarePageModel
    {
        private readonly IFarmManager _farmManager;
        private readonly IScenarioWorkspaceManager _scenarioWorkspaceManager;

        public ICollection<Farm> VisibleFarms { get; set; }
        
        public ICollection<ScenarioVm> Scenarios { get; set; }

        public DashboardModel(
            IFarmManager farmManager,
            IScenarioWorkspaceManager scenarioWorkspaceManager
            )
        {
            _farmManager = farmManager;
            _scenarioWorkspaceManager = scenarioWorkspaceManager;
        }

        public Site Site { get; set; }

        public ICollection<FieldSummaryVm> Fields { get; set; }

        public async Task<IActionResult> OnGet(string siteId = null)
        {
            var visibleSites = await GetVisibleSites();

            VisibleFarms = GroupSitesByFarms(visibleSites);

            if (!VisibleFarms.Any())
            {
                return RedirectToPage("/NoSites");
            }

            if (!TrySelectSite(siteId, visibleSites))
            {
                return NotFound();
            }

            ShowForecasting = User.CanViewForecastsForSite(Site?.SiteId);

            if (ShowForecasting)
            {
                Scenarios = (await _scenarioWorkspaceManager.GetAllScenariosForSite(Site.SiteId))
                    .Where(x => !x.IsDeleted)
                    .ToList();
            }

            return Page();
        }

        public bool ShowForecasting { get; set; }

        private async Task<List<Site>> GetVisibleSites()
        {
            var allSites = await _farmManager.GetAllSites();

            return allSites
                .Where(x => User.CanViewSite(x.SiteId))
                .OrderBy(x => x.Farm.Name)
                .ThenBy(x => x.Order)
                .ToList();
        }

        private static List<Farm> GroupSitesByFarms(List<Site> visibleSites)
        {
            return visibleSites
                .GroupBy(x => x.Farm)
                .Select(f =>
                {
                    var farm = f.Key;
                    farm.Sites = f.OrderBy(x => x.Order).ToList();
                    return farm;
                })
                .OrderBy(x => x.Name)
                .ToList();
        }

        private bool TrySelectSite(string siteId, List<Site> visibleSites)
        {
            Site = string.IsNullOrEmpty(siteId) 
                ? visibleSites.FirstOrDefault() 
                : visibleSites.FirstOrDefault(x => x.SiteId == siteId);

            FullSiteName = Site != null ? $"{Site.Farm.Name} / {Site.Name}" : string.Empty;

            return Site != null;
        }

        public string FullSiteName { get; set; }
    }
}