using System.ComponentModel.DataAnnotations;

namespace Cpm.Core.Models
{
    public class SiteUserPermission
    {
        [Required]
        public string SiteId { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool CanView { get; set; }

        public bool CanUpdateDiary { get; set; }

        public bool CanLogActualData { get; set; }

        public bool CanPlan { get; set; }

        public bool CanAllocate { get; set; }

        public bool CanForecast { get; set; }

        public bool CanManagePermissions { get; set; }

        // Navigation

        public Site Site { get; set; }

        public static SiteUserPermission Empty(Site site, string userId)
        {
            return new SiteUserPermission
            {
                UserId = userId,
                Site = site,
                SiteId = site.SiteId
            };
        }
    }
}