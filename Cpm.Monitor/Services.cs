using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Cpm.Monitor
{
    public static class Services
    {
        public static void AddApixuMonitor(this IServiceCollection services, IConfigurationSection configSection)
        {
            services.Configure<ApixuProbeOptions>(configSection.GetSection("Probe"));
            services.AddSingleton<ApixuProbe>();

            services.Configure<ApixuMonitorOptions>(configSection.GetSection("Monitor"));
            services.AddSingleton<IHostedService, ApixuMonitorService>();
        }
    }
}
