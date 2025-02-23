using System;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core;
using Cpm.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Cpm.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IdentityResult Result { get; set; }

        public IndexModel(IConfiguration configuration, UserManager<ApplicationUser> userManager, ILogger<IndexModel> logger)
        {
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            if (_configuration["Environment"] == "Development")
            {
                if (!_userManager.Users.Any())
                {
                    _logger.LogInformation("No users and in Development mode, creating initial user");
                    // create initial admin user
                    var user = _configuration
                        .GetSection("DevelopmentAdmin:User")
                        .Get<ApplicationUser>();

                    user.Created = Clock.Now.ToUniversalTime();
                    user.UserName = user.Email;

                    var password = _configuration["DevelopmentAdmin:Password"];

                    Result = await _userManager.CreateAsync(user, password);

                    if (Result.Succeeded)
                    {
                        _logger.LogInformation("User \"{0}\" created successfully", user.GetDefaultDisplayName());
                    }
                    else
                    {
                        _logger.LogWarning("Could not create user \"{0}\": {1}", 
                            user.GetDefaultDisplayName(),
                            string.Join(Environment.NewLine, Result.Errors
                                .Select((error, i) => $"Error #{i}: {error.Description}"))
                            );
                    }
                }
            }
        }
    }
}
