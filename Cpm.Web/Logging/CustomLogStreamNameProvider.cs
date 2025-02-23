using System;
using Cpm.Core;
using Serilog.Sinks.AwsCloudWatch;

namespace Cpm.Web.Logging
{
    public class CustomLogStreamNameProvider : ILogStreamNameProvider
    {
        private readonly string _deploymentHost;

        public CustomLogStreamNameProvider(string deploymentHost)
        {
            _deploymentHost = deploymentHost;
        }

        public string GetLogStreamName()
        {
            return $"{Clock.Now.ToUniversalTime().ToString("yyyy-MM-dd_HH-mm-ss")}_{_deploymentHost}_{Environment.MachineName}";
        }
    }
}