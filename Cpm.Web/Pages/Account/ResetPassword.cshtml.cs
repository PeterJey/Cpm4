using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Cpm.Core;
using Cpm.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Cpm.Web.PageHelpers;
using Microsoft.Extensions.Logging;

namespace Cpm.Web.Pages.Account
{
    public class ResetPasswordModel : StatusAwarePageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(UserManager<ApplicationUser> userManager, ILogger<ResetPasswordModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = code
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                SaveStatus()
                    .Success()
                    .Text("Your password has been reset. Now you can log in.")
                    .Dismissible();

                return RedirectToPage("/Account/Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                if (user.LastLogin.HasValue)
                {
                    _logger.LogInformation("Password for user \"{0}\" was reset", user.GetDefaultDisplayName());
                }
                else
                {
                    user.Activated = Clock.Now.ToUniversalTime();
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation("Initial password for a new user \"{0}\" was set", user.GetDefaultDisplayName());
                }

                SaveStatus()
                    .Success()
                    .Text("Your password has been reset. Now you can log in.")
                    .Dismissible();

                return RedirectToPage("/Account/Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}
