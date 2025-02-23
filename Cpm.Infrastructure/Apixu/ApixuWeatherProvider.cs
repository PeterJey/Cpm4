using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core;
using Cpm.Core.Services.Weather;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Cpm.Infrastructure.Apixu
{
    public class ApixuWeatherProvider : IWeatherProvider
    {
        private readonly IOptions<ApixuOptions> _options;

        private readonly ILogger<ApixuWeatherProvider> _logger;

        public const int MaxForecastDays = 15;

        public ApixuWeatherProvider(IOptions<ApixuOptions> options, ILogger<ApixuWeatherProvider> logger)
        {
            _options = options;
            _logger = logger;
        }

        public async Task<WeatherDay> GetHistoric(string postcode, DateTime date, CancellationToken cancellationToken)
        {
            using (_logger.BeginScope((postcode, date)))
            {
                if (date >= Clock.Now.Date)
                {
                    _logger.LogWarning("Apixu history works only for past dates but {Date:d} requested", date);
                    return null;
                }

                var location = FormatLocation(postcode);

                var url = "https:" + $"//api.apixu.com/v1/history.json?key={_options.Value.ApiKey}&q={location}&dt={date:yyyy-MM-dd}";

                var weatherData = await MakeWebRequest(url, cancellationToken);

                try
                {
                    var dayData = weatherData?.forecast?.forecastday?.First().day;

                    var newValue = CreateWeatherDay(dayData);

                    return newValue;
                }
                catch (Exception)
                {
                    _logger.LogWarning("Missing fields in weather data");
                    return null;
                }
            }
        }

        public async Task<WeatherNow> GetCurrent(string postcode, CancellationToken cancellationToken)
        {
            var location = FormatLocation(postcode);

            var url = "https:" + $"//api.apixu.com/v1/current.json?key={_options.Value.ApiKey}&q={location}";

            var weatherData = await MakeWebRequest(url, cancellationToken);

            var dayData = weatherData?.current;

            return dayData != null 
                ? new WeatherNow
                    {
                        Description = dayData.condition.text,
                        Icon = dayData.condition.icon,
                        TempMin = dayData.temp_c,
                        TempMax = dayData.temp_c
                    }
                : null;
        }

        private static WeatherDay CreateWeatherDay(Day dayData)
        {
            return dayData != null
                ? new WeatherDay
                {
                    MinTemp = dayData.mintemp_c,
                    MaxTemp = dayData.maxtemp_c,
                    AvgTemp = dayData.avgtemp_c,
                    Icon = dayData.condition.icon,
                    Description = dayData.condition.text
                }
                : null;
        }

        private static string FormatLocation(string postcode)
        {
            return postcode;
        }

        private async Task<WeatherModel> MakeWebRequest(string url, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET {Url}", url);

            HttpResponseMessage response;
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                response = await client.GetAsync(url, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Apixu request takes more than {Timeout} seconds, cancelled", _options.Value.WebRequestTimeout.TotalSeconds);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Http error");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Failed to obtain weather info: {Status} {StatusCode}: {Reason}",
                    (int)response.StatusCode,
                    response.StatusCode,
                    response.ReasonPhrase);
                return null;
            }

            _logger.LogInformation("Received weather info");

            return JsonConvert.DeserializeObject<WeatherModel>(
                await response.Content.ReadAsStringAsync()
                );
        }

        public async Task<ICollection<WeatherDay>> GetForecast(string postcode, CancellationToken cancellationToken)
        {
            using (_logger.BeginScope(postcode))
            {
                _logger.LogTrace("Getting forecast for {Postcode}", postcode);

                var location = FormatLocation(postcode);

                var url = "https:" + $"//api.apixu.com/v1/forecast.json?key={_options.Value.ApiKey}&q={location}&days={MaxForecastDays}";

                var weatherData = await MakeWebRequest(url, cancellationToken);

                var daysData = weatherData?.forecast?.forecastday ?? new List<Forecastday>();

                return daysData.Select(dayData => CreateWeatherDay(dayData.day)).ToList();
            }
        }
    }
}