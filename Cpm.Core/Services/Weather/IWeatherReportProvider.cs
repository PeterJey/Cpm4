using System.Threading;
using System.Threading.Tasks;

namespace Cpm.Core.Services.Weather
{
    public interface IWeatherReportProvider
    {
        Task<WeatherReport> GetReport(string postcode, CancellationToken cancellationToken);
    }
}