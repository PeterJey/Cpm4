using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cpm.Core.Models;
using Cpm.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Cpm.Web.Security
{
    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        private readonly ISitePermissionManager _sitePermissionManager;

        public ClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, 
            IOptions<IdentityOptions> optionsAccessor,
            ISitePermissionManager sitePermissionManager) 
            : base(userManager, optionsAccessor)
        {
            _sitePermissionManager = sitePermissionManager;
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);

            ((ClaimsIdentity)principal.Identity).AddClaims(
                (await _sitePermissionManager.GetForUserAsync(user.Id))
                    .SelectMany(CreateClaimsForPermission)
                    .Concat(GetUserClaims(user))
                );

            return principal;
        }

        private IEnumerable<Claim> GetUserClaims(ApplicationUser user)
        {
            if (user.IsApplicationAdmin)
            {
                yield return new Claim(ClaimTypes.AdministeringApplication, string.Empty);
            }
            yield return new Claim(ClaimTypes.ShortName, user.GetDefaultShortName()); 
            yield return new Claim(ClaimTypes.LongName, user.GetDefaultDisplayName()); 
        }

        private IEnumerable<Claim> CreateClaimsForPermission(SiteUserPermission permission)
        {
            if (permission.CanManagePermissions) yield return new Claim(ClaimTypes.AdministeringUsers, permission.SiteId);

            if (permission.CanView) yield return new Claim(ClaimTypes.Viewing, permission.SiteId);

            if (permission.CanForecast)
            {
                yield return new Claim(ClaimTypes.ForecastingSite, permission.SiteId);

                foreach (var field in permission.Site.Fields)
                {
                    yield return new Claim(ClaimTypes.ForecastingField, field.FieldId);
                }
            }

            if (permission.CanAllocate)
            {
                yield return new Claim(ClaimTypes.AllocatingForSite, permission.SiteId);

                foreach (var field in permission.Site.Fields)
                {
                    yield return new Claim(ClaimTypes.AllocatingForField, field.FieldId);
                }
            }

            if (permission.CanUpdateDiary)
            {
                yield return new Claim(ClaimTypes.UpdatingTheSiteDiary, permission.SiteId);

                foreach (var field in permission.Site.Fields)
                {
                    yield return new Claim(ClaimTypes.UpdatingTheFieldDiary, field.FieldId);
                }
            }

            if (permission.CanLogActualData)
            {
                yield return new Claim(ClaimTypes.ActualSiteDataLogging, permission.SiteId);

                foreach (var field in permission.Site.Fields)
                {
                    yield return new Claim(ClaimTypes.ActualFieldDataLogging, field.FieldId);
                }
            }

            if (permission.CanPlan)
            {
                yield return new Claim(ClaimTypes.DailySitePlanning, permission.SiteId);

                foreach (var field in permission.Site.Fields)
                {
                    yield return new Claim(ClaimTypes.DailyFieldPlanning, field.FieldId);
                }
            }
        }
    }
}
