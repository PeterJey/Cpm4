using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core;
using Cpm.Core.Models;
using Cpm.Core.Services;
using Cpm.Core.Services.Context;
using Cpm.Core.ViewModels;
using Cpm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cpm.Infrastructure
{
    public class EfScenarioWorkspaceManager : IScenarioWorkspaceManager
    {
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuditDataProvider _auditDataProvider;
        private readonly IOptions<ScenarioManagerOptions> _options;
        private readonly ILogger<EfScenarioWorkspaceManager> _logger;

        public EfScenarioWorkspaceManager(
            IMemoryCache cache,
            ApplicationDbContext dbContext,
            IAuditDataProvider auditDataProvider,
            IOptions<ScenarioManagerOptions> options,
            ILogger<EfScenarioWorkspaceManager> logger
            )
        {
            _cache = cache;
            _dbContext = dbContext;
            _auditDataProvider = auditDataProvider;
            _options = options;
            _logger = logger;
        }

        public Task Discard(string contextId)
        {
            _cache.Remove(contextId);
            return Task.CompletedTask;
        }

        public async Task<ScenarioContext> CreateScenario(string siteId)
        {
            var site = await _dbContext.Sites
                .Include(x => x.Farm)
                .Include(x => x.Fields)
                .SingleAsync(x => x.SiteId == siteId);

            var context = ScenarioContext.CreateForSite(site);

            AddOrUpdate(context);

            return context;
        }

        public async Task<ScenarioContext> LoadScenario(string scenarioId)
        {
            var scenario = await GetScenario(scenarioId);

            if (scenario == null)
            {
                return null;
            }

            var context = ScenarioContext.FromScenario(scenario);

            AddOrUpdate(context);

            return context;
        }

        public bool TryGet(string contextId, out ScenarioContext context)
        {
            return _cache.TryGetValue(contextId, out context);
        }

        public async Task<bool> Save(string contextId)
        {
            if (!TryGet(contextId, out var context))
            {
                _logger.LogWarning("Trying to save, but context {contextId} not found", contextId);
                return false;
            }

            return await Persist(context);
        }

        public async Task Delete(string contextId)
        {
            if (!TryGet(contextId, out var context))
            {
                _logger.LogWarning("Trying to delete, but context {contextId} not found", contextId);
                return;
            }

            await Persist(context.MarkDeleted());

            await Discard(contextId);
        }

        public async Task<ICollection<ScenarioVm>> GetAllScenariosForSite(string siteId)
        {
            var scenarios = await _dbContext.Scenarios
                .Where(x => x.SiteId == siteId)
                .ToListAsync();

            return scenarios
                .GroupBy(x => x.ScenarioId)
                .Select(x => x.OrderByDescending(s => s.Version).First())
                .OrderBy(x => x.Name)
                .Select(x => new ScenarioVm
                {
                    ScenarioId = x.ScenarioId,
                    Name = x.Name,
                    IsDeleted = x.IsDeleted,
                })
                .ToList();
        }

        public void Update(ScenarioContext context)
        {
            AddOrUpdate(context);
        }

        public Task<ScenarioContext> Duplicate(string contextId)
        {
            if (!TryGet(contextId, out var context))
            {
                _logger.LogWarning("Trying to duplicate, but context {contextId} not found", contextId);
                return null;
            }

            var newContext = context.CreateDuplicate();

            AddOrUpdate(newContext);

            return Task.FromResult(newContext);
        }

        private void AddOrUpdate(ScenarioContext context)
        {
            _cache.Set(context.Id, context, new MemoryCacheEntryOptions
            {
                SlidingExpiration = _options.Value.Expiration,
            });
        }

        private async Task<bool> Persist(ScenarioContext context)
        {
            Scenario scenario;

            if (string.IsNullOrEmpty(context.ScenarioId))
            {
                scenario = new Scenario
                {
                    Version = 1,
                    ScenarioId = IdHelper.NewId(),
                };
            }
            else
            {
                scenario = await GetScenario(context.ScenarioId);

                if (scenario == null)
                {
                    _logger.LogWarning("Could not get nor create scenario for context {context}", context);
                    return false;
                }

                ++scenario.Version;
            }

            context.UpdateScenario(scenario);

            scenario.CreatedBy = _auditDataProvider.GetUserField();
            scenario.CreatedOn = _auditDataProvider.GetTimestampField();

            _dbContext.Entry(scenario).State = EntityState.Added;

            await _dbContext.SaveChangesAsync();

            var newContext = context.MarkSaved();

            AddOrUpdate(newContext);

            return true;
        }

        private Task<Scenario> GetScenario(string scenarioId)
        {
            return _dbContext.Scenarios
                .Include(x => x.Site)
                .ThenInclude(x => x.Farm)
                .Include(x => x.Site)
                .ThenInclude(x => x.Fields)
                .Where(x => x.ScenarioId == scenarioId)
                .OrderByDescending(x => x.Version)
                .FirstOrDefaultAsync();
        }
    }
}