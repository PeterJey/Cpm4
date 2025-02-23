using System;
using System.Globalization;
using System.IO;
using Cpm.Web.Startup;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace Cpm.Web
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var configuration = AppConfiguration.Create(args);

            var cultureInfo = new CultureInfo(configuration["Culture"]);

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            AppLogging.Configure(configuration);

            Log.Information("Configured culture: {0} {1}", cultureInfo.DisplayName, cultureInfo.Name);

            try
            {
                Log.Information("Starting web host");
                new WebHostBuilder()
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseConfiguration(configuration)
                    .UseSerilog()
                    .UseStartup<StartupWithExternalDependencies>()
                    .Build()
                    .Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        // Only used by EF Tooling
        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder()
                .UseConfiguration(AppConfiguration.Create(args))
                .ConfigureLogging((ctx, logging) => { }) // No logging
                .UseStartup<StartupWithStubs>()
                .UseSetting("DesignTime", "true")
                .Build();
        }
    }
}
