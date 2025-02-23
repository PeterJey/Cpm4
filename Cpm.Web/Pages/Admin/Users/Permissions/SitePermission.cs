using System.ComponentModel;
using Cpm.Core.Models;

namespace Cpm.Web.Pages.Admin.Users.Permissions
{
    public class SitePermission
    {
        public string SiteId { get; set; }

        [DisplayName("Site")]
        public string SiteName { get; set; }

        [DisplayName("Farm")]
        public string FarmName { get; set; }

        [DisplayName("Manage permissions")]
        public bool Manage { get; set; }

        [DisplayName("Create and modify scenarios")]
        public bool Forecast { get; set; }

        [DisplayName("Allocate products")]
        public bool Allocate { get; set; }

        [DisplayName("Daily planning")]
        public bool Plan { get; set; }

        [DisplayName("Record actual yield")]
        public bool Actual { get; set; }

        [DisplayName("Update the diary")]
        public bool Diary { get; set; }

        [DisplayName("See the site")]
        public bool View { get; set; }

        public bool IsDefault { get; set; }

        public static SitePermission CreateFromExisting(SiteUserPermission permission)
        {
            return new SitePermission
            {
                UserId = permission.UserId,
                SiteId = permission.SiteId,
                SiteName = permission.Site.Name,
                FarmName = permission.Site.Farm.Name,
                Manage = permission.CanManagePermissions,
                Forecast = permission.CanForecast,
                Allocate = permission.CanAllocate,
                Plan = permission.CanPlan,
                Actual = permission.CanLogActualData,
                Diary = permission.CanUpdateDiary,
                View = permission.CanView,
            };
        }

        public string UserId { get; set; }

        public static SitePermission CreateDefault(Site site, string userId)
        {
            return new SitePermission
            {
                UserId = userId,
                SiteId = site.SiteId,
                SiteName = site.Name,
                FarmName = site.Farm.Name,
                IsDefault = true,
            };
        }
    }
}