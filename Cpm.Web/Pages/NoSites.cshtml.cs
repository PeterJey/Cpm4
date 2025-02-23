using Cpm.Web.PageHelpers;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Pages
{
    public class NoSitesModel : StatusAwarePageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}