using System;
using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Models;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;
using Cpm.Core.Services.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Cpm.Core.Services.Context
{
    public class ScenarioContext
    {
        public string Id { get; private set; }
        public string ScenarioId { get; private set; }
        public string SiteId { get; private set; }
        public string Name { get; private set; }
        public SeasonsProfile SeasonsProfile { get; private set; }
        public IReadOnlyCollection<FieldState> FieldStates { get; private set; }
        public bool IsDirty { get; private set; }
        public bool IsDeleted { get; private set; }
        public string SiteName { get; private set; }
        public string FarmName { get; private set; }

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(),
                new JsonYieldConverter(),
                new JsonFieldStateConverter()
            },
        };

        public static ScenarioContext FromScenario(Scenario scenario)
        {
            if (scenario == null) throw new ArgumentNullException(nameof(scenario));

            var state = JsonConvert.DeserializeObject<ScenarioContextState>(
                scenario.SerializedSettings,
                Settings
            );

            return new ScenarioContext
            {
                Id = IdHelper.NewId(),
                ScenarioId = scenario.ScenarioId,
                SiteId = scenario.SiteId,
                Name = scenario.Name,
                IsDirty = false,
                IsDeleted = scenario.IsDeleted,
                SiteName = scenario.Site.Name,
                FarmName = scenario.Site.Farm.Name,
                SeasonsProfile = SeasonsProfile.FromDictionary(state.Seasons),
                FieldStates = state.Fields.ToArray(),
                Fields = scenario.Site.Fields
                    .OrderBy(x => x.Order)
                    .Select(x => x.FieldId)
                    .ToArray(),
            };
        }

        public IReadOnlyCollection<string> Fields { get; set; }

        private ScenarioContext()
        {
        }

        public static ScenarioContext CreateForSite(Site site)
        {
            return new ScenarioContext
            {
                Id = IdHelper.NewId(),
                SiteId = site.SiteId,
                SiteName = site.Name,
                FarmName = site.Farm.Name,
                Name = "New scenario",
                SeasonsProfile = SeasonsProfile.Default,
                FieldStates = Enumerable.Repeat(FieldState.Default, site.Fields.Count)
                    .ToArray(),
                Fields = site.Fields
                    .OrderBy(x => x.Order)
                    .Select(x => x.FieldId)
                    .ToArray(),
                IsDirty = true,
            };
        }

        private ScenarioContext CreateClone()
        {
            return new ScenarioContext
            {
                Id = Id,
                ScenarioId = ScenarioId,
                SiteId = SiteId,
                Name = Name,
                IsDirty = IsDirty,
                IsDeleted = IsDeleted,
                SiteName = SiteName,
                FarmName = FarmName,
                SeasonsProfile = SeasonsProfile,
                FieldStates = FieldStates,
                Fields = Fields,
            };
        }

        public ScenarioContext CreateDuplicate()
        {
            var context = CreateClone();
            context.IsDirty = true;
            context.Id = IdHelper.NewId();
            context.ScenarioId = null;
            context.Name = $"Duplicate of {context.Name}";
            return context;
        }

        public ScenarioContext MarkDeleted()
        {
            var context = CreateClone();
            context.IsDeleted = true;
            context.IsDirty = true;
            return context;
        }

        public void UpdateScenario(Scenario scenario)
        {
            scenario.Name = Name;
            scenario.SiteId = SiteId;
            scenario.IsDeleted = IsDeleted;
            scenario.SerializedSettings = JsonConvert.SerializeObject(
                new ScenarioContextState
                {
                    Seasons = SeasonsProfile.ToCompactDictionary(),
                    Fields = FieldStates.ToArray(),
                },
                Settings
            );
        }

        public ScenarioContext MarkSaved()
        {
            var context = CreateClone();
            context.IsDirty = false;
            return context;
        }

        public ScenarioContext Rename(string name) => 
            Modify(c => c.Name = name);

        private ScenarioContext Modify(Action<ScenarioContext> action)
        {
            var context = CreateClone();
            action(context);
            context.IsDirty = true;
            return context;
        }

        private ScenarioContext ModifyField(int index, Func<FieldState, FieldState> newState)
        {
            if (index< 0 || index >= FieldStates.Count) throw new ArgumentOutOfRangeException(nameof(index));

            var fields = FieldStates.ToList();

            fields[index] = newState(fields[index]);

            return Modify(c => c.FieldStates = fields);
        }

        public ScenarioContext ChangeSeasonScores(SeasonsProfile scores) =>
            Modify(c => c.SeasonsProfile = scores);

        public ScenarioContext ChangeAlgorithm(int index, string algorithm) =>
            ModifyField(index, f => f.ChangeAlgorithm(algorithm));

        public ScenarioContext ChangeSettings(int index, string name, string value) =>
            ModifyField(index, f => f.ChangeSettings(name, value));

        public ScenarioContext ChangeSelection(IEnumerable<bool> selection) =>
            Modify(c => 
                c.FieldStates = FieldStates
                        .Zip(selection, (state, b) => state.ChangeIsVisible(b))
                        .ToArray()
                );
    }
}