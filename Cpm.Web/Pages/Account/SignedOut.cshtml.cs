using Cpm.Web.PageHelpers;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Pages.Account
{
    public class SignedOutModel : StatusAwarePageModel
    {
        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToPage("/Index");
            }

            return Page();
        }
    }
}