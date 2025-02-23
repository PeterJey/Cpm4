using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Models;
using Cpm.Infrastructure.Data;
using Cpm.Web.PageHelpers;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cpm.Web.Pages.Admin.Users.Permissions
{
    public class EditModel : StatusAwarePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ISitePermissionManager _sitePermissionManager;
        private readonly ApplicationDbContext _dbContext;

        private readonly ILogger<EditModel> _logger;

        public Site Site { get; set; }

        public ApplicationUser SiteUser { get; set; }

        [BindProperty]
        public SitePermission Input { get; set; }

        public EditModel(UserManager<ApplicationUser> userManager,
            ISitePermissionManager sitePermissionManager,
            ApplicationDbContext dbContext,
            ILogger<EditModel> logger)
        {
            _userManager = userManager;
            _sitePermissionManager = sitePermissionManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string userId = null, string siteId = null)
        {
            if (!await ProcessParameters(userId, siteId))
            {
                return NotFound();
            }

            if (!User.CanChangePermissionsForSite(siteId))
            {
                return Forbid();
            }

            var permissions = Site.UserPermissions
                .SingleOrDefault(x => x.UserId == userId);

            if (permissions != null)
            {
                Input = SitePermission.CreateFromExisting(permissions);
            }
            else
            {
                Input = new SitePermission
                {
                    FarmName = Site.Farm.Name,
                    SiteName = Site.Name,
                    SiteId = Site.SiteId,
                };
            }

            ProtectFromCutOff = userId == _userManager.GetUserId(User) && !User.CanManageApplication();

            return Page();
        }

        public bool ProtectFromCutOff { get; set; }

        public async Task<IActionResult> OnPostAsync(string userId = null, string siteId = null)
        {
            if (!await ProcessParameters(userId, siteId))
            {
                return NotFound();
            }

            if (!User.CanChangePermissionsForSite(siteId))
            {
                return Forbid();
            }

            var permission = Site.UserPermissions.SingleOrDefault(x => x.UserId == userId)
                             ?? SiteUserPermission.Empty(Site, SiteUser.Id);

            permission.CanManagePermissions = Input.Manage;
            permission.CanForecast = Input.Forecast;
            permission.CanAllocate = Input.Allocate;
            permission.CanPlan = Input.Plan;
            permission.CanLogActualData = Input.Actual;
            permission.CanUpdateDiary = Input.Diary;
            permission.CanView = Input.View;

            await _sitePermissionManager.AddOrUpdate(permission);

            SaveStatus()
                .Success()
                .Text("Permissions to {0} were updated successfully.", Site.Name)
                .Dismissible();

            return RedirectToPage("./Index", new {id = userId});
        }

        private async Task<bool> ProcessParameters(string userId, string siteId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(siteId))
            {
                return false;
            }

            Site = await _dbContext.Sites
                .Include(x => x.Farm)
                .Include(x => x.UserPermissions)
                .ThenInclude(x => x.Site)
                .SingleAsync(x => x.SiteId == siteId);

            SiteUser = await _userManager.FindByIdAsync(userId);

            if (Site == null)
            {
                _logger.LogWarning("Site with id {SiteId} not found", siteId);
                return false;
            }

            if (SiteUser == null)
            {
                _logger.LogWarning("User with id {userId} not found", userId);
                return false;
            }

            return true;
        }
    }
}