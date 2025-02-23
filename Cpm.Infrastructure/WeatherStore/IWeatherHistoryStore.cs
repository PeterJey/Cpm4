using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Services.Weather;

namespace Cpm.Infrastructure.WeatherStore
{
    public interface IWeatherHistoryStore
    {
        Task AddSample(string location, WeatherNow sample, CancellationToken cancellationToken);
    }
}