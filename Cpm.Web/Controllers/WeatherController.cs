using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Weather;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class WeatherController : Controller
    {
        private readonly IWeatherReportProvider _weatherReportProvider;

        public WeatherController(IWeatherReportProvider weatherReportProvider)
        {
            _weatherReportProvider = weatherReportProvider;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Report(string postcode = null)
        {
            var weatherReport = await _weatherReportProvider.GetReport(postcode, CancellationToken.None);
            return PartialView("_WeatherReport", weatherReport);
        }
    }
}
