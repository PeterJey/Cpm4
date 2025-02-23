using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Weather;
using Cpm.Infrastructure.Apixu;
using Microsoft.Extensions.Options;

namespace Cpm.Monitor
{
    public class ApixuProbe : IMonitorProbe
    {
        private readonly ApixuWeatherProvider _provider;
        private readonly ApixuProbeOptions _options;

        public ApixuProbe(
            ApixuWeatherProvider provider,
            IOptions<ApixuProbeOptions> options)
        {
            _provider = provider;
            _options = options.Value;
        }

        public async Task<ProbeStatus> Check(CancellationToken cancellationToken)
        {
            var historicTask = _provider.GetHistoric(
                _options.Location, 
                DateTime.Today.AddDays(_options.HistoricOffsetDays),
                cancellationToken
                );

            var currentTask = _provider.GetCurrent(
                _options.Location,
                cancellationToken
                );

            var forecastTask = _provider.GetForecast(
                _options.Location,
                cancellationToken
                );

            await Task.WhenAll(historicTask, currentTask, forecastTask);

            var errors = new List<string>();

            if (historicTask.IsFaulted)
            {
                AddException(errors, historicTask, "historic");
            }
            else
            {
                if (historicTask.Result == null)
                {
                    errors.Add("Invalid or missing historic data");
                }
            }

            if (currentTask.IsFaulted)
            {
                AddException(errors, currentTask, "current");
            }
            else
            {
                if (currentTask.Result == null)
                {
                    errors.Add("Invalid or missing current data");
                }
            }

            if (forecastTask.IsFaulted)
            {
                AddException(errors, forecastTask, "forecast");
            }
            else
            {
                if (forecastTask.Result == null)
                {
                    errors.Add("Invalid or missing forecast data");
                }
                else
                {
                    if (forecastTask.Result.Count != _options.ExpectedForecastDays)
                    {
                        errors.Add($"Expected the forecast for {_options.ExpectedForecastDays} days but got {forecastTask.Result.Count}");
                    }
                }
            }

            return errors.Any()
                ? ProbeStatus.Fail(string.Join("\n", errors)) 
                : ProbeStatus.Ok;
        }

        private static void AddException(List<string> errors, Task task, string type)
        {
            errors.Add($"Request for {type} data failed: {task.Exception.InnerException.Message}");
        }
    }
}