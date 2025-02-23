using Cpm.Web.PageHelpers;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Pages.Admin
{
    public class IndexModel : StatusAwarePageModel
    {
        public IActionResult OnGet()
        {
            if (!User.CanViewAdminPage())
            {
                return Forbid();
            }

            IsUsersLinkVisible = User.CanManageUsers();
            IsFarmsLinkVisible = User.CanManageFarms();
            IsProfilesLinkVisible = User.CanManageProfiles();

            return Page();
        }

        public bool IsProfilesLinkVisible { get; set; }

        public bool IsUsersLinkVisible { get; set; }

        public bool IsFarmsLinkVisible { get; set; }

    }
}