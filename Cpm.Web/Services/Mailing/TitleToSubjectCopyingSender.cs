using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace Cpm.Web.Services.Mailing
{
    internal class TitleToSubjectCopyingSender : ISender
    {
        private readonly ISender _next;

        public TitleToSubjectCopyingSender(ISender next)
        {
            _next = next;
        }

        public SendResponse Send(Email email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public Task<SendResponse> SendAsync(Email email, CancellationToken? token = null)
        {
            var match = Regex.Match(
                    email.Data.Body,
                    @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\<\/title\>", 
                    RegexOptions.IgnoreCase);

            if (match.Groups.Count == 2)
            {
                email.Data.Subject = match.Groups["Title"].Value;
            }
            else
            {
                email.Data.Subject = email.Data.Subject ?? "";
            }

            return _next.SendAsync(email, token);
        }
    }
}