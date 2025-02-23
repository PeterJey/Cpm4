using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;
using Cpm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cpm.Infrastructure.New
{
    public class EfScenarioRepository : IScenarioRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EfScenarioRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ScenarioState> GetScenarioState(string scenarioId)
        {
            var scenario = (await _dbContext.Scenarios
                    .Where(x => x.ScenarioId == scenarioId)
                    .ToArrayAsync()
                )
                .OrderBy(x => x.Version)
                .LastOrDefault();

            if (scenario == null)
            {
                return null;
            }

            var fromJson = JsonConvert.DeserializeObject<ScenarioJsonSchema>(scenario.SerializedSettings);

            return new ScenarioState(
                fromJson.Fields
                    .Select(fs => new ForecastParameters(SeasonsProfile.FromDictionary(fromJson.Seasons), fs.Algorithm, fs.Settings)),
                fromJson.Fields.Select(x => x.IsVisible)
                );
        }

        private class ScenarioJsonSchema
        {
            public Dictionary<Season, string> Seasons { get; set; }
            public PerFieldJsonSchema[] Fields { get; set; }

            public class PerFieldJsonSchema
            {
                public bool IsVisible { get; set; }
                public string Algorithm { get; set; }
                public Dictionary<string, string> Settings { get; set; }
            }
        }

        private string Serialize(ScenarioState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            var settings = new ScenarioJsonSchema
            {
                Seasons = new Dictionary<Season, string>(state.Parameters.First().SeasonsProfile.ToCompactDictionary()),
                Fields = state.Parameters
                    .Zip(state.Visibility, (p, v) => new ScenarioJsonSchema.PerFieldJsonSchema
                    {
                        IsVisible = v,
                        Algorithm = p.AlgorithmName,
                        Settings = new Dictionary<string, string>(p.Settings)
                    })
                    .ToArray()
            };

            return JsonConvert.SerializeObject(settings);
        }
    }
}
