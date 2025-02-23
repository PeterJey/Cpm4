using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Infrastructure.Data;
using Cpm.Web.PageHelpers;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cpm.Web.Pages.Admin.Users
{
    public class IndexModel : StatusAwarePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IList<ApplicationUser> UsersToView { get; set; }

        public bool IsAddUserVisible { get; set; }
        public bool IsEditingUsersVisible { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!User.CanViewAdminPage())
            {
                return Forbid();
            }

            UsersToView = await GetUsersToAdminister();

            IsAddUserVisible = User.CanCreateUsers();
            IsEditingUsersVisible = User.CanManageUsers();

            return Page();
        }

        private async Task<List<ApplicationUser>> GetUsersToAdminister()
        {
            var allUsers = await _userManager
                .Users
                .Include(x => x.SitePermissions)
                .ToListAsync();

            return allUsers
                    .Where(u => User.CanManageApplication() || u.SitePermissions.Any(p => User.CanChangePermissionsForSite(p.SiteId)))
                    .ToList();
        }
    }
}