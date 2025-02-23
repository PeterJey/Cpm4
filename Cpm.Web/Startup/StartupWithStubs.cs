using Cpm.Web.Services.Mailing;
using FluentEmail.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cpm.Web.Startup
{
    public class StartupWithStubs : CommonStartup
    {
        public StartupWithStubs(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureWeather(IServiceCollection services)
        {
        }

        protected override ISender CreateSenderService()
        {
            return NoOpSender.Instance;
        }
    }
}