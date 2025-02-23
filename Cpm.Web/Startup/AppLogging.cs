using System;
using System.Globalization;
using Amazon;
using Amazon.CloudWatchLogs;
using Cpm.Web.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;

namespace Cpm.Web.Startup
{
    public static class AppLogging
    {
        public static void Configure(IConfiguration configuration)
        {
            if (!Enum.TryParse<LogEventLevel>(configuration["Serilog:ConsoleSink:MinimumLogLevel"], out var consoleLogLevel))
            {
                consoleLogLevel = LogEventLevel.Debug;
            }

            if (!Enum.TryParse<LogEventLevel>(configuration["Serilog:CloudWatchSink:MinimumLogLevel"],
                out var persistentLogLevel))
            {
                persistentLogLevel = LogEventLevel.Debug;
            }

            var options = new CloudWatchSinkOptions
            {
                LogGroupName = configuration["Serilog:CloudWatchSink:LogGroup"],
                LogStreamNameProvider = new CustomLogStreamNameProvider(configuration["DeploymentHost"]),
                LogEventRenderer = new CustomLogEventRenderer(),
                MinimumLogEventLevel = persistentLogLevel,
            };

            var awsCredentials = AwsHelper.CreateAwsCredentialsFromConfig(configuration);

            var awsConfig = new AmazonCloudWatchLogsConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(configuration["Serilog:CloudWatchSink:Region"]),
            };

            var awsClient = new AmazonCloudWatchLogsClient(awsCredentials, awsConfig);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is((LogEventLevel)Math.Min((int)consoleLogLevel, (int)persistentLogLevel))
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.AmazonCloudWatch(options, awsClient)
                .WriteTo.Console(consoleLogLevel, formatProvider: CultureInfo.CurrentCulture)
                .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(Console.Error);
        }
    }
}