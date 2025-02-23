namespace Cpm.Core.Services.Forecast
{
    public interface IForecastStatsServiceProvider
    {
        IForecastStatsService GetInstance();
    }
}