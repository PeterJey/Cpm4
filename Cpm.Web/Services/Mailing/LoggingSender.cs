using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Cpm.Web.Services.Mailing
{
    public class LoggingSender : ISender
    {
        private readonly ISender _next;
        private readonly ILogger _logger;

        public LoggingSender(ISender next)
        {
            _next = next;
            _logger = Log.Logger.ForContext<LoggingSender>();
        }

        public SendResponse Send(Email email, CancellationToken? token = null)
        {
            return SendAsync(email, token).GetAwaiter().GetResult();
        }

        public async Task<SendResponse> SendAsync(Email email, CancellationToken? token = null)
        {
            var result = await _next.SendAsync(email, token);
            if (result.Successful)
            {
                _logger.Information("Sent email to {ToAddress} with subject {Subject}", email.Data.ToAddresses, email.Data.Subject);
            }
            else
            {
                _logger.Error("Encountered {Count} errors while sending email to {ToAddresses} with subject {Subject}:", 
                    new object []
                    {
                        result.ErrorMessages.Count,
                        email.Data.ToAddresses,
                        email.Data.Subject,
                    }.Concat(result.ErrorMessages
                        .Select((msg, i) => $"Error {i}: {msg}")
                        )
                    );
            }
            return result;
        }
    }
}