using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Services.Fields;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cpm.Web.Pages.Fields
{
    public class WeeklyDiaryModel : PageModel
    {
        private readonly IFieldRepository _fieldRepository;

        public WeeklyDiaryModel(
            IFieldRepository fieldRepository
        )
        {
            _fieldRepository = fieldRepository;
        }

        public async Task<IActionResult> OnGetAsync(string siteId, int? position = null)
        {
            if (!User.CanViewDiaryForSite(siteId))
            {
                return Forbid();
            }

            // TODO: accessint site just for site name and farm name
            var site = await _fieldRepository.GetSiteById(siteId);

            if (site == null)
            {
                return NotFound();
            }

            SiteId = siteId;
            Position = position;

            FarmName = site.Fields.First().FarmName;
            SiteName = site.Fields.First().SiteName;

            return Page();
        }

        public string SiteName { get; set; }

        public string FarmName { get; set; }

        public string SiteId { get; set; }

        public int? Position { get; set; }
    }
}