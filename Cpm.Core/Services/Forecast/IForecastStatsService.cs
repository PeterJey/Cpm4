using System;
using System.Threading.Tasks;
using Cpm.Core.Services.Diary;

namespace Cpm.Core.Services.Forecast
{
    public interface IForecastStatsService : IDisposable
    {
        Task Update(string scenarioId, string fieldId, ForecastResult forecastResult);

        Task Save();

        Task<ForecastStatistics> GetStats(string scenarioId, string fieldId, string algorithmName);
    }
}