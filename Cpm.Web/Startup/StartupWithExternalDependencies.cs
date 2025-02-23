using System;
using Cpm.Core.Services.Weather;
using Cpm.Infrastructure;
using Cpm.Infrastructure.Apixu;
using Cpm.Infrastructure.Data;
using Cpm.Infrastructure.OpenWeather;
using Cpm.Infrastructure.WeatherStore;
using Cpm.Monitor;
using Cpm.Web.Services.JobScheduling;
using FluentEmail.Core.Interfaces;
using FluentEmail.Mailgun;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cpm.Web.Startup
{
    public class StartupWithExternalDependencies : CommonStartup
    {
        public StartupWithExternalDependencies(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureWeather(IServiceCollection services)
        {
            //ConfigureApixu(services);
            ConfigureOwm(services);

            services.AddSingleton<IWeatherReportProvider, WeatherReportProvider>();
        }

        private void ConfigureOwm(IServiceCollection services)
        {
            services.Configure<OwmWeatherProviderOptions>(Configuration.GetSection("Owm"));

            services.AddScoped<IWeatherProvider>(p => new HybridWeatherProvider(
                p.GetRequiredService<ApplicationDbContext>(),
                p.GetRequiredService<IMemoryCache>(),
                p.GetRequiredService<ILogger<HybridWeatherProvider>>()
            ));

            services.AddSingleton<OwmWeatherProvider>();
            services.AddSingleton<EfWeatherHistoryStore>();

            // non-Owm specific
            services.AddSingleton<IJobFactory>(p => new HybridWeatherRecordingJobFactory(
                p.GetRequiredService<IServiceScopeFactory>(),
                p.GetRequiredService<OwmWeatherProvider>(),
                p.GetRequiredService<EfWeatherHistoryStore>(),
                p.GetRequiredService<IMemoryCache>(),
                p.GetRequiredService<ILogger<HybridWeatherRecordingJobFactory>>()
            ));

            services.AddHostedService<JobRunnerService>();
            services.Configure<JobRunnerServiceOptions>(Configuration.GetSection("JobRunner"));
        }

        private void ConfigureApixu(IServiceCollection services)
        {
            services.AddApixuMonitor(Configuration.GetSection("Apixu"));

            services.Configure<ApixuOptions>(Configuration.GetSection("Apixu:Provider"));
            services.AddSingleton<ApixuWeatherProvider>();

            services.Configure<MemoryCachedWeatherProviderProxyOptions>(Configuration.GetSection("WeatherCache"));
            services.AddSingleton<IWeatherProvider>(p =>
                new MemoryCachedWeatherProviderProxy(
                    p.GetService<IOptions<MemoryCachedWeatherProviderProxyOptions>>(),
                    p.GetService<ApixuWeatherProvider>(),
                    p.GetService<IMemoryCache>(),
                    p.GetService<ILogger<MemoryCachedWeatherProviderProxy>>()
                )
            );
        }

        protected override void ConfigureAuthentication(IServiceCollection services)
        {
            base.ConfigureAuthentication(services);

            var ticketStore = new CacheTicketStore();

            services.AddSingleton(ticketStore);

            services.ConfigureApplicationCookie(options => { options.SessionStore = ticketStore; });

            services.AddAuthentication()
                .AddMicrosoftAccount(microsoftOptions =>
                {
                    microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ApplicationId"];
                    microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:Password"];
                });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });
        }

        protected override void ConfigureExtra(IServiceProvider provider)
        {
            base.ConfigureExtra(provider);

            var cache = provider.GetRequiredService<IMemoryCache>();
            var store = provider.GetRequiredService<CacheTicketStore>();

            store.SetCache(cache);
        }

        protected override ISender CreateSenderService()
        {
            return new MailgunSender(Configuration["Mailgun:Domain"], Configuration["Mailgun:Key"]);
        }
    }
}