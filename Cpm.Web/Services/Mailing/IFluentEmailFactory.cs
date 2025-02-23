using Cpm.Core.Services.Mailing;
using FluentEmail.Core;

namespace Cpm.Web.Services.Mailing
{
    public interface IFluentEmailFactory
    {
        IFluentEmail Create(EmailCategory category);
        IFluentEmail Create(EmailCategory category, EmailRecipient recipient);
    }
}