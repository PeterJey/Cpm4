using System;
using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Services.Fields;
using Cpm.Core.Services.Profiles;
using Optional;

namespace Cpm.Core.Services.Forecast
{
    public class AlgorithmInput
    {
        public AlgorithmInput(FieldDetails field, ForecastParameters parameters)
        {
            ProfileName = field.ProfileName;
            SeasonsProfile = parameters.SeasonsProfile;
            AreaInHectares = field.AreaInHectares;
            HarvestWeights = field.HarvestHistory.Weekly.Where(x => x.IsCompleted).Select(x => x.Weight).ToArray();
            FirstWeekOfHarvest = field.HarvestHistory.StartingWeek;
            Budget = field.Budget;
            Settings = parameters.Settings;
            Postcode = field.Postcode;
            FirstWeekCommencing = field.FirstWeekCommencing;
        }

        public DateTime FirstWeekCommencing { get; set; }

        public string ProfileName { get; }
        public SeasonsProfile SeasonsProfile { get; }
        public decimal AreaInHectares { get; }
        public Option<int> FirstWeekOfHarvest { get; }
        public IReadOnlyCollection<decimal> HarvestWeights { get; }
        public Yield Budget { get; }
        public IReadOnlyDictionary<string, string> Settings { get; }
        public string Postcode { get; }

        public AlgorithmInput(string profileName, SeasonsProfile seasonsProfile, decimal areaInHectares, Option<int> firstWeekOfHarvest, IEnumerable<decimal> harvestWeights, Yield budget, IReadOnlyDictionary<string, string> settings, string postcode, DateTime firstWeekCommencing)
        {
            ProfileName = profileName;
            SeasonsProfile = seasonsProfile;
            AreaInHectares = areaInHectares;
            FirstWeekOfHarvest = firstWeekOfHarvest;
            HarvestWeights = (harvestWeights ?? Enumerable.Empty<decimal>()).ToArray();
            Budget = budget;
            Settings = settings;
            Postcode = postcode;
            FirstWeekCommencing = firstWeekCommencing;
        }
    }
}