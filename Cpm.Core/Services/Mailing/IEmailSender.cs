using System.Threading.Tasks;

namespace Cpm.Core.Services.Mailing
{
    public interface IEmailSender
    {
        Task SendInvitationEmail(EmailRecipient recipient, string userId, string code);
        Task SendResetPasswordAsync(EmailRecipient recipient, string userId, string code);
        Task SendWelcomeEmail(EmailRecipient recipient);
    }
}