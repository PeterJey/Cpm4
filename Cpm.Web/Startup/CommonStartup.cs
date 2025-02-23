using System;
using System.IO;
using System.Linq;
using Cpm.AwsS3;
using Cpm.Core.Farms;
using Cpm.Core.Services;
using Cpm.Core.Services.Allocations;
using Cpm.Core.Services.Context;
using Cpm.Core.Services.Diary;
using Cpm.Core.Services.Fields;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Mailing;
using Cpm.Core.Services.Notes;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;
using Cpm.Core.Services.Serialization;
using Cpm.Infrastructure;
using Cpm.Infrastructure.AjaxHttpStatusCodes;
using Cpm.Infrastructure.Data;
using Cpm.Infrastructure.New;
using Cpm.Web.Security;
using Cpm.Web.Services.Mailing;
using FluentEmail.Core.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cpm.Web.Startup
{
    public abstract class CommonStartup
    {
        protected CommonStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Default")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            });

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ClaimsPrincipalFactory>();

            ConfigureAuthentication(services);

            services.AddMemoryCache();

            services.AddMvc(options =>
                {
                })
                .AddRazorPagesOptions(options =>
                {
                    // Authorize pages
                    PagePolicies.Apply(options);

                    options.Conventions.AddPageRoute("/Errors/NotFound", "Status/404");
                    options.Conventions.AddPageRoute("/Errors/Error", "Status/{status:int}");
                });

            ConfigureUrlHelper(services);

            ConfigureEmails(services);

            services.AddSingleton(AwsHelper.CreateAwsCredentialsFromConfig(Configuration));

            // NEW 
            services.AddScoped<IForecastService, ForecastService>();
            services.AddScoped<IScenarioRepository, EfScenarioRepository>();
            services.AddScoped<IFieldRepository, EfFieldRepository>();
            // END OF NEW 
            services.AddScoped<IPictureRepository, S3PictureRepository>();
            services.AddScoped<IPictureMetadataSerializer, JsonPictureMetadataSerializer>();
            services.Configure<S3PictureRepoOptions>(Configuration.GetSection("S3PictureRepo"));

            services.AddScoped<IFarmManager, EfFarmManager>();
            services.AddScoped<IHarvestManager, EfHarvestManager>();
            services.AddScoped<IFieldManager, EfFieldManager>();
            services.AddScoped<IDiaryManager, DiaryManager>();
            services.AddScoped<IAllocationRepository, EfAllocationRepository>();
            services.AddScoped<IAuditDataProvider, AuditDataProvider>();
            services.AddScoped<ISitePermissionManager, SitePermissionManager>();
            services.AddScoped<IForecastManager, ForecastManager>();
            services.AddScoped<IYieldSerializer, JsonYieldSerializer>();
            services.AddScoped<IWeightsSerializer, JsonWeightsSerializer>();
            services.AddScoped<IAlgorithmSettingsSerializer, JsonAlgorithmSettingsSerializer>();
            services.AddScoped<IScenarioWorkspaceManager, EfScenarioWorkspaceManager>();
            services.AddScoped<IWeatherEvaluator, AvgTempWeatherEvaluator>();
            services.AddScoped<GdhDifferenceStrategy>();
            services.AddScoped<IWeeklyWeatherProvider, WeeklyWeatherProvider>();
            services.AddScoped<ITemperatureProfileRepository, EfTemperatureProfileRepository>();
            services.AddScoped<IAllocationManager, AllocationManager>();
            services.AddScoped<IUserPreferencesFactory, EfUserPreferencesFactory>();
            services.AddScoped<IUserPreferences>(p => 
                ((IUserPreferencesFactory)p.GetService(typeof(IUserPreferencesFactory))).Create()
                );

            services.AddScoped<IForecastStatsServiceProvider, EfForecastStatsServiceProvider>();

            services.AddSingleton<IProfileRepository, EfCachedProfileRepository>();
            services.AddSingleton<IAlgorithmProvider, HardcodedAlgorithmProvider>();

            services.Configure<ScenarioManagerOptions>(Configuration.GetSection("ScenarioManager"));
            services.Configure<ProfileRepositoryOptions>(Configuration.GetSection("ProfileProvider"));

            ConfigureWeather(services);

            ConfigureDataProtection(services);
        }

        private void ConfigureDataProtection(IServiceCollection services)
        {
            var keysDirectory = Configuration["DataProtectionKeysLocation"];

            if (!string.IsNullOrEmpty(keysDirectory))
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(
                        new DirectoryInfo(keysDirectory)
                    );
            }
        }

        protected abstract void ConfigureWeather(IServiceCollection services);

        private static void ConfigureUrlHelper(IServiceCollection services)
        {
            services
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped<IUrlHelper>(x => x
                    .GetRequiredService<IUrlHelperFactory>()
                    .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext))
                .AddSingleton<Func<IUrlHelper>>(s => s.GetService<IUrlHelper>)
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Errors/Error");
            }

            app.UseStatusCodePagesWithRedirects("/Status/{0}");
            app.DisableStatusCodesForAjax();

            ConfigureProxyIfNeeded(app);

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(Configuration["Culture"])
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            ConfigureExtra(provider);
        }

        protected virtual void ConfigureExtra(IServiceProvider provider)
        {
        }

        protected virtual void ConfigureAuthentication(IServiceCollection services)
        {
        }

        private void ConfigureEmails(IServiceCollection services)
        {
            var rawSender = CreateSenderService();

            var compositeSender = new LoggingSender(
                new TitleToSubjectCopyingSender(
                   rawSender
                    )
                );

            var categoriesSetup = Configuration
                .GetSection("Mailing:Categories")
                .GetChildren()
                .Where(x => Enum.TryParse<EmailCategory>(x.Key, out var _))
                .ToDictionary(k => Enum.Parse<EmailCategory>(k.Key), v => v.Get<FluentEmailEnvelope>());

            var factory = new FluentEmailFactory(categoriesSetup, compositeSender);

            services.AddScoped<IFluentEmailFactory>(s => factory);
            services.AddScoped<IEmailSender, FluentEmailSender>();
            services.AddSingleton<ISender>(rawSender);
        }

        protected abstract ISender CreateSenderService();

        private void ConfigureProxyIfNeeded(IApplicationBuilder app)
        {
            if ((Configuration["IsUsingProxy"] ?? "") == "1")
            {
                var forwardedHeadersOptions = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                    RequireHeaderSymmetry = false
                };
                forwardedHeadersOptions.KnownNetworks.Clear();
                forwardedHeadersOptions.KnownProxies.Clear();

                app.UseForwardedHeaders(forwardedHeadersOptions);
            }
        }
    }
}
