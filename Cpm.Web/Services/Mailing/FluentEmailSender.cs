using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Cpm.Core.Services.Mailing;
using Cpm.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Services.Mailing
{
    public class FluentEmailSender : IEmailSender
    {
        private readonly IFluentEmailFactory _emailFactory;
        private readonly IUrlHelper _urlHelper;

        public FluentEmailSender(IFluentEmailFactory emailFactory, IUrlHelper urlHelper)
        {
            _emailFactory = emailFactory;
            _urlHelper = urlHelper;
        }

        public Task SendInvitationEmail(EmailRecipient recipient, string userId, string code)
        {
            return _emailFactory
                .Create(EmailCategory.Default, recipient)
                .UsingEmbeddedTemplate("Invitation",
                    new
                    {
                        InvitationUrl = HtmlEncoder.Default.Encode(_urlHelper.ResetPasswordCallbackUrl(userId, code)),
                        ApplicationUrl = HtmlEncoder.Default.Encode(_urlHelper.ApplicationUrl()),
                    })
                .SendAsync();
        }
        
        public Task SendResetPasswordAsync(EmailRecipient recipient, string userId, string code)
        {
            return _emailFactory
                .Create(EmailCategory.Default, recipient)
                .UsingEmbeddedTemplate("PasswordReset", 
                new
                {
                    ResetUrl = HtmlEncoder.Default.Encode(_urlHelper.ResetPasswordCallbackUrl(userId, code)),
                    ApplicationUrl = HtmlEncoder.Default.Encode(_urlHelper.ApplicationUrl()),
                })
                .SendAsync();
        }

        public Task SendWelcomeEmail(EmailRecipient recipient)
        {
            return _emailFactory
                .Create(EmailCategory.Default, recipient)
                .UsingEmbeddedTemplate("Welcome",
                    new
                    {
                        DashboardUrl = HtmlEncoder.Default.Encode(_urlHelper.DashboardUrl()),
                        ApplicationUrl = HtmlEncoder.Default.Encode(_urlHelper.ApplicationUrl()),
                    })
                .SendAsync();
        }
    }

}