using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Farms;
using Cpm.Core.Models;
using Cpm.Web.PageHelpers;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Pages.Admin.Farms
{
    public class IndexModel : StatusAwarePageModel
    {
        private readonly IFarmManager _farmManager;
        public IList<Farm> AllFarms { get; set; }

        public IndexModel(IFarmManager farmManager)
        {
            _farmManager = farmManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.CanManageFarms())
            {
                return Forbid();
            }

            AllFarms = (await _farmManager.GetAllFarms())
                .ToList();

            return Page();
        }
    }
}