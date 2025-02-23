using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;
using Cpm.Core.ViewModels;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cpm.Web.Pages.Admin.Profiles
{
    public class IndexModel : PageModel
    {
        private readonly IProfileRepository _profileRepository;

        public IndexModel(
            IProfileRepository profileRepository
            )
        {
            _profileRepository = profileRepository;
        }

        public async Task<IActionResult> OnGetAsync(string pattern = null)
        {
            if (!User.CanManageProfiles())
            {
                return Forbid();
            }

            var profiles = await _profileRepository.GetByNamePattern(pattern);

            Profiles = profiles
                .GroupBy(x => x.Name)
                .ToDictionary(k => k.Key, v => new ProfileVm(v));


            PossibleSeasonScores = Enum.GetValues(typeof(Season))
                .OfType<Season>()
                .ToDictionary(k => k.ToString(), SeasonScore.PossibleScoresFor);

            PossibleQuality = Enum.GetValues(typeof(ForecastQuality))
                .OfType<ForecastQuality>()
                .Select(x => x.ToString());

            return Page();
        }

        public IEnumerable<string> PossibleQuality { get; set; }

        public Dictionary<string, IEnumerable<string>> PossibleSeasonScores { get; set; }

        public Dictionary<string, ProfileVm> Profiles { get; set; }
    }
}