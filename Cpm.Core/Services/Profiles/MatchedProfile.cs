using System.Collections.Generic;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Scenarios;

namespace Cpm.Core.Services.Profiles
{
    public class MatchedProfile
    {
        public int StartingWeek { get; set; }
        public decimal ExtraPotential { get; set; }
        public ICollection<ProfilePoint> Points { get; set; }
        public ForecastQuality Quality { get; set; }
        public ISet<Season> Seasons { get; set; }
        public ICollection<string> Comments { get; set; }
        public SeasonsProfile SeasonsProfile { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }
}