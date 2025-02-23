using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

namespace Cpm.Web.Security
{
    public class PagePolicies
    {
        public static void Apply(RazorPagesOptions options)
        {
            // Authorize access to everything
            options.Conventions.AuthorizeFolder("/");

            // Allow unauthorized exceptions
            options.Conventions.AllowAnonymousToFolder("/Errors");

            options.Conventions.AllowAnonymousToPage("/About");
            options.Conventions.AllowAnonymousToPage("/Contact");
            options.Conventions.AllowAnonymousToPage("/Index");

            options.Conventions.AllowAnonymousToPage("/Account/AccesDenied");
            options.Conventions.AllowAnonymousToPage("/Account/ConfirmEmail");
            options.Conventions.AllowAnonymousToPage("/Account/ExternalLogin");
            options.Conventions.AllowAnonymousToPage("/Account/ForgotPassword");
            options.Conventions.AllowAnonymousToPage("/Account/ForgotPasswordConfirmation");
            options.Conventions.AllowAnonymousToPage("/Account/Lockout");
            options.Conventions.AllowAnonymousToPage("/Account/Login");
            options.Conventions.AllowAnonymousToPage("/Account/LoginWith2fa");
            options.Conventions.AllowAnonymousToPage("/Account/LoginWithRecoveryCode");
            options.Conventions.AllowAnonymousToPage("/Account/ResetPassword");
            options.Conventions.AllowAnonymousToPage("/Account/SignedOut");
        }
    }
}