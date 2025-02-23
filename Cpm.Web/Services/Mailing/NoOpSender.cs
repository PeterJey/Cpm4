using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace Cpm.Web.Services.Mailing
{
    public class NoOpSender : ISender
    {
        private static readonly NoOpSender _instance = new NoOpSender();

        private static readonly SendResponse _response = new SendResponse();

        public static ISender Instance => _instance;

        public SendResponse Send(Email email, CancellationToken? token = null)
        {
            return _response;
        }

        public Task<SendResponse> SendAsync(Email email, CancellationToken? token = null)
        {
            return Task.FromResult(_response);
        }
    }
}