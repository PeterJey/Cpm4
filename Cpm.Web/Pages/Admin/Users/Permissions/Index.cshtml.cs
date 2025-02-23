using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Farms;
using Cpm.Core.Models;
using Cpm.Infrastructure.Data;
using Cpm.Web.PageHelpers;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Pages.Admin.Users.Permissions
{
    public class IndexModel : StatusAwarePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFarmManager _farmManager;
        private readonly ISitePermissionManager _sitePermissionManager;

        public ApplicationUser SelectedUser { get; set; }

        public List<SitePermission> Permissions { get; set; }
        public bool ShowClear { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager, 
            IFarmManager farmManager,
            ISitePermissionManager sitePermissionManager)
        {
            _userManager = userManager;
            _farmManager = farmManager;
            _sitePermissionManager = sitePermissionManager;
        }

        public async Task<IActionResult> OnGetAsync(string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToPage("../Index");
            }
            var selectedUser = await _userManager.FindByIdAsync(id);
            if (selectedUser == null)
            {
                return NotFound();
            }

            SelectedUser = selectedUser;

            Permissions = (await GetSitePermissions(await GetSitesToAdmin(), selectedUser))
                .ToList();

            if (!User.CanViewAdminPage())
            {
                return Forbid();
            }

            ShowClear = User.CanManageApplication();

            return Page();
        }

        private async Task<ICollection<Site>> GetSitesToAdmin()
        {
            return (await _farmManager.GetAllSites())
                .Where(s => User.CanChangePermissionsForSite(s.SiteId))
                .ToArray();
        }

        private async Task<IEnumerable<SitePermission>> GetSitePermissions(ICollection<Site> sites, ApplicationUser user)
        {
            var userPermissions = await _sitePermissionManager.GetForUserAsync(user.Id);
            var groupJoin = sites
                .GroupJoin(userPermissions,
                    site => site.SiteId, 
                    siteUserPermission => siteUserPermission.SiteId, 
                    (s, p) => new { Site = s, Permissions = p }).ToList();
            var siteUserPermissions = groupJoin
                .SelectMany(x => x.Permissions.Select(SitePermission.CreateFromExisting).DefaultIfEmpty(SitePermission.CreateDefault(x.Site, user.Id)))
                .ToList();
            return siteUserPermissions;
        }
    }
}