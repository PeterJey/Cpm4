using Cpm.Core.Services.Forecast;
using Cpm.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Cpm.Infrastructure
{
    public class EfForecastStatsServiceProvider : IForecastStatsServiceProvider
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EfForecastStatsServiceProvider(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IForecastStatsService GetInstance()
        {
            var scope = _serviceScopeFactory.CreateScope();
            return new EfForecastStatsService(
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
                () => scope.Dispose(),
                scope.ServiceProvider.GetRequiredService<IMemoryCache>()
                );
        }
    }
}