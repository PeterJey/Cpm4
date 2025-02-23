using System.Linq;
using System.Security.Claims;

namespace Cpm.Web.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool CanManageApplication(this ClaimsPrincipal user) 
            => 
                AdminOnly(user);
        
        public static bool CanManageFarms(this ClaimsPrincipal user) 
            => 
                AdminOnly(user);

        public static bool CanCreateUsers(this ClaimsPrincipal user) 
            => 
                AdminOnly(user);

        public static bool CanManageProfiles(this ClaimsPrincipal user) 
            => 
                AdminOnly(user);

        public static bool CanManageUsers(this ClaimsPrincipal user)
            =>
                AdminOrAdministerAnySite(user);

        public static bool CanViewAdminPage(this ClaimsPrincipal user) 
            => 
                AdminOrAdministerAnySite(user);

        public static bool CanViewForecastsForSite(this ClaimsPrincipal user, string siteId) 
            => 
                AdminOrAnyClaim(user, siteId, ClaimTypes.ForecastingSite);

        public static bool CanViewForecastsForField(this ClaimsPrincipal user, string fieldId) 
            => 
                AdminOrAnyClaim(user, fieldId, ClaimTypes.ForecastingField);

        public static bool CanViewSite(this ClaimsPrincipal user, string siteId) 
            => 
                AdminOrAnyClaim(user, siteId, ClaimTypes.Viewing);

        public static bool CanChangePermissionsForSite(this ClaimsPrincipal user, string siteId) 
            => 
                AdminOrAnyClaim(user, siteId, ClaimTypes.AdministeringUsers);

        public static bool CanViewAllocationsForSite(this ClaimsPrincipal user, string siteId) 
            => 
                AdminOrAnyClaim(user, siteId, ClaimTypes.AllocatingForSite);

        public static bool CanChangeAllocationsForSite(this ClaimsPrincipal user, string siteId) 
            => 
                AdminOrAnyClaim(user, siteId, ClaimTypes.AllocatingForSite);

        public static bool CanChangeAllocationsForField(this ClaimsPrincipal user, string fieldId) 
            => 
                AdminOrAnyClaim(user, fieldId, ClaimTypes.AllocatingForField);

        public static bool CanViewDiaryForField(this ClaimsPrincipal user, string fieldId) 
            => 
                AdminOrAnyClaim(user, fieldId, ClaimTypes.UpdatingTheFieldDiary, ClaimTypes.ActualFieldDataLogging, ClaimTypes.DailyFieldPlanning);

        public static bool CanViewDiaryForSite(this ClaimsPrincipal user, string siteId)
            => 
                AdminOrAnyClaim(user, siteId, ClaimTypes.UpdatingTheSiteDiary, ClaimTypes.ActualSiteDataLogging, ClaimTypes.DailySitePlanning);

        public static bool CanChangeForecastForSite(this ClaimsPrincipal user, string siteId) 
            => 
                AdminOrAnyClaim(user, siteId, ClaimTypes.ForecastingSite);

        public static bool CanChangeActiveScenarioForField(this ClaimsPrincipal user, string fieldId) 
            => 
                AdminOrAnyClaim(user, fieldId, ClaimTypes.ForecastingField);

        public static bool CanChangeActiveScenarioForFieldsInSite(this ClaimsPrincipal user, string siteId) 
            => 
                AdminOrAnyClaim(user, siteId, ClaimTypes.ForecastingSite);

        public static bool CanChangeDescriptionForField(this ClaimsPrincipal user, string fieldId) 
            => 
                AdminOrAnyClaim(user, fieldId, ClaimTypes.ForecastingField);

        public static bool CanChangeBudgetForField(this ClaimsPrincipal user, string fieldId) 
            => 
                AdminOrAnyClaim(user, fieldId, ClaimTypes.ForecastingField);

        public static bool CanChangeActualDataForField(this ClaimsPrincipal user, string fieldId) 
            => 
                AdminOrAnyClaim(user, fieldId, ClaimTypes.ActualFieldDataLogging);

        public static bool CanChangeDailyPlanForField(this ClaimsPrincipal user, string fieldId) 
            => 
                AdminOrAnyClaim(user, fieldId, ClaimTypes.DailyFieldPlanning);

        public static bool CanChangeDiaryForField(this ClaimsPrincipal user, string fieldId) 
            => 
                AdminOrAnyClaim(user, fieldId, ClaimTypes.UpdatingTheFieldDiary);


        private static bool AdminOnly(ClaimsPrincipal user)
            => AnyClaim(user, ClaimTypes.AdministeringApplication);

        private static bool AdminOrAdministerAnySite(ClaimsPrincipal user)
            => AnyClaim(user, ClaimTypes.AdministeringApplication, ClaimTypes.AdministeringUsers);

        private static bool AnyClaim(ClaimsPrincipal user, params string[] claims)
            => claims.Any(c => user.HasClaim(claim => claim.Type == c));

        private static bool AdminOrAnyClaim(ClaimsPrincipal user, string value, params string[] claims)
        {
            return user.HasClaim(x => x.Type == ClaimTypes.AdministeringApplication)
                   || claims.Any(c => user.HasClaim(c, value));
        }
    }
}
