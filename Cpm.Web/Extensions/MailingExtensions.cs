using System.Reflection;
using Cpm.Core.Services.Mailing;
using Cpm.Infrastructure.Data;
using FluentEmail.Core;

namespace Cpm.Web.Extensions
{
    public static class MailingExtensions
    {
        public static IFluentEmail UsingEmbeddedTemplate<T>(this IFluentEmail fluentEmail, string templateName, T model)
        {
            return fluentEmail.UsingTemplateFromEmbedded(
                $"Cpm.Web.EmailTemplates.{templateName}.html", 
                model,
                Assembly.GetExecutingAssembly(), 
                isHtml: true
                );
        }

        public static EmailRecipient AsEmailRecipient(this ApplicationUser user)
        {
            return new EmailRecipient
            {
                Name = user.GetDefaultDisplayName(),
                Email = user.Email,
            };
        }
    }
}
