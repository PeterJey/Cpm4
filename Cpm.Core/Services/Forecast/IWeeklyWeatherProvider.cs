using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cpm.Core.Services.Forecast
{
    public interface IWeeklyWeatherProvider
    {
        Task<WeeklyWeatherResult> GetForLocation(string location, DateTime currentWeekStart, CancellationToken cancellationToken);
    }
}