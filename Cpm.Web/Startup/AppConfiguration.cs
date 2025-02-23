using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Cpm.Web.Startup
{
    public static class AppConfiguration
    {
        public static IConfiguration Create(string[] args)
        {
            const string enviromentVariablePrefix = "ASPNETCORE_";

            var builder = new ConfigurationBuilder();

            var environmentName = Environment.GetEnvironmentVariable(enviromentVariablePrefix + "ENVIRONMENT") ?? string.Empty;

            builder.SetBasePath(Directory.GetCurrentDirectory());

            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json",
                    optional: true, reloadOnChange: true);

            if (string.Compare(environmentName, "Development", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                var appAssembly = Assembly.Load(
                    new AssemblyName(Assembly.GetEntryAssembly()?.GetName().Name));
                if (appAssembly != null)
                {
                    builder.AddUserSecrets(appAssembly, optional: true);
                }
            }

            builder.AddEnvironmentVariables(enviromentVariablePrefix);

            if (args != null)
            {
                builder.AddCommandLine(args);
            }

            builder.AddJsonFile("buildinfo.json", optional: true);

            builder.AddJsonFile("deployment.json", optional: true);

            return builder.Build();
        }
    }
}