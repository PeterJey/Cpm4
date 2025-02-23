using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Models;
using Cpm.Web.PageHelpers;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cpm.Web.Pages.Admin.Users.Permissions
{
    public class ClearModel : StatusAwarePageModel
    {
        private readonly ISitePermissionManager _sitePermissionManager;
        private readonly ILogger<ClearModel> _logger;

        public ClearModel(
            ISitePermissionManager sitePermissionManager,
            ILogger<ClearModel> logger
            )
        {
            _sitePermissionManager = sitePermissionManager;
            _logger = logger;
        }

        public SiteUserPermission Permission { get; set; }

        public async Task<IActionResult> OnPostAsync(string userId = null, string siteId = null)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(siteId))
            {
                return RedirectToPage("./Index", new { id = userId });
            }

            Permission = (await _sitePermissionManager.GetForUserAsync(userId))
                .SingleOrDefault(x => x.SiteId == siteId);

            if (Permission == null)
            {
                return NotFound();
            }

            if (!User.CanManageApplication())
            {
                return Forbid();
            }

            await _sitePermissionManager.Clear(Permission);

            SaveStatus()
                .Success()
                .Text("Permissions to site ")
                .Strong(Permission.Site.Name)
                .Text(" are cleared.")
                .Dismissible();

            return RedirectToPage("./Index", new { id = userId });
        }
    }
}