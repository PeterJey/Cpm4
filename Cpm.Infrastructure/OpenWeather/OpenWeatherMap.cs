using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cpm.Infrastructure.OpenWeather
{
    public class OpenWeatherMap
    {
        private readonly string _apiKey;

        private readonly string _baseUrl = "https://api.openweathermap.org/data/2.5";

        public OpenWeatherMap(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<DailyForecast> GetForecast(double latitude, double longitude, CancellationToken cancellationToken)
        {
            return await Download<DailyForecast>(latitude, longitude, cancellationToken);
        }

        private async Task<T> Download<T>(double latitude, double longitude, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(GetUrl<T>(latitude, longitude), cancellationToken);

                if (!response.IsSuccessStatusCode) throw new Exception($"Weather service failed: {response.StatusCode}");

                var text = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(text);
            }
        }

        private string GetUrl<T>(double latitude, double longitude)
        {
            var action = string.Empty;
            var extra = string.Empty;

            switch (typeof(T).Name)
            {
                case nameof(DailyForecast):
                    action = "forecast/daily";
                    extra = "&cnt=16";
                    break;
                case nameof(Current):
                    action = "weather";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(T));
            }
            return $"{_baseUrl}/{action}?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric&lang=en{extra}";
        }

        public async Task<Current> GetCurrent(double latitude, double longitude, CancellationToken cancellationToken)
        {
            return await Download<Current>(latitude, longitude, cancellationToken);
        }
    }

    #region responces 

    public class HourlyForecast
    {
        public string cod { get; set; }
        public float message { get; set; }
        public int cnt { get; set; }
        public List<HourlyFoo> list { get; set; }
        public City city { get; set; }
    }
    public class DailyForecast
    {
        public string cod { get; set; }
        public float message { get; set; }
        public int cnt { get; set; }
        public List<DailyFoo> list { get; set; }
        public City city { get; set; }
    }
    public class Current
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string _base { get; set; }
        public Main main { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
        public float pressure { get; set; }
        public float sea_level { get; set; }
        public float grnd_level { get; set; }
        public float humidity { get; set; }
        public float temp_kf { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Wind
    {
        public float speed { get; set; }
        public float deg { get; set; }
    }

    public class Snow
    {
        public float _3h { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public float message { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class HourlyFoo
    {
        public int dt { get; set; }
        public Main main { get; set; }
        public List<Weather> weather { get; set; }
        public Clouds clouds { get; set; }
        public Wind wind { get; set; }
        public Snow snow { get; set; }
        public Sys sys { get; set; }
        public string dt_txt { get; set; }
    }

    public class DailyFoo
    {
        public int dt { get; set; }
        public Temps temp { get; set; }
        public float pressure { get; set; }
        public float humidity { get; set; }
        public List<Weather> weather { get; set; }
        public float speed { get; set; }
        public float deg { get; set; }
        public float clouds { get; set; }
        public float snow { get; set; }
    }

    public class Temps
    {
        public float day { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public float night { get; set; }
        public float eve { get; set; }
        public float morn { get; set; }
    }

    public class Coord
    {
        public float lat { get; set; }
        public float lon { get; set; }
    }

    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public Coord coord { get; set; }
        public string country { get; set; }
        public int population { get; set; }
    }


    #endregion
}