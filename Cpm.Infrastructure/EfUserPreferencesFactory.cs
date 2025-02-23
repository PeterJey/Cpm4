using System.Collections.Generic;
using Cpm.Core.Services;
using Cpm.Core.ViewModels;
using Cpm.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Cpm.Infrastructure
{
    public class EfUserPreferencesFactory : IUserPreferencesFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public EfUserPreferencesFactory(
            IHttpContextAccessor httpContextAccessor, 
            UserManager<ApplicationUser> userManager
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public UserPreferences Create()
        {
            var user = _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User).Result;

            var prefs = user.AreaUnit?.ToLowerInvariant() == "ha"
                ? GetHectares()
                : GetAcres();

            prefs.AllocationUnitName = user.AllocationUnit?.ToLowerInvariant() ?? "kg";

            return prefs;
        }

        public ICollection<OptionVm> AvailableAreaUnits()
        {
            return new[]
            {
                new OptionVm("ha", "Hectares"), 
                new OptionVm("ac", "Acres"), 
            };
        }

        public ICollection<OptionVm> AvailableAllocationUnits()
        {
            return new[]
            {
                new OptionVm("kg", "Kilograms"),
                new OptionVm("trays", "Trays/Boxes"),
                new OptionVm("punnets", "Punnets"),
            };
        }

        private static UserPreferences GetHectares()
        {
            return new UserPreferences
            {
                AreaUnit = "ha",
                AreaUnitName = "hectare",
                AreaUnitNamePlural = "hectares",
                AreaUnitFactor = 1m,
            };
        }

        private static UserPreferences GetAcres()
        {
            return new UserPreferences
            {
                AreaUnit = "ac",
                AreaUnitName = "acre",
                AreaUnitNamePlural = "acres",
                AreaUnitFactor = 0.40468564224m,
            };
        }
    }
}
