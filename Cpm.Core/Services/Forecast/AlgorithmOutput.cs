using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;

namespace Cpm.Core.Services.Forecast
{
    public class AlgorithmOutput
    {
        private AlgorithmOutput()
        {
        }

        public static AlgorithmOutput NoProfile = new AlgorithmOutput
        {
            Quality = ForecastQuality.Unknown,
            AffectingSeasons = new HashSet<Season>(),
            Data = new ForecastData(),
            Comments = new[] {"No profile found"}
        };

        public ICollection<string> Comments { get; set; }

        public ISet<Season> AffectingSeasons { get; set; }

        public ForecastQuality Quality { get; set; }

        public static AlgorithmOutput FromProfile(MatchedProfile profile, decimal total, int offset)
        {
            var values = profile.Points
                .Select(x => new ForecastValue()
                {
                    Weight = x.Weight * total,
                    Type = HarvestValueType.Forecasted,
                    PerHour = x.PerHour
                })
                .ToList();

            var startingWeek = profile.StartingWeek + offset;

            return new AlgorithmOutput
            {
                Quality = profile.Quality,
                Data = new ForecastData(startingWeek, values),
                AffectingSeasons = profile.Seasons.ToHashSet(),
                Comments = profile.Comments.ToList()
            };
        }

        public ForecastData Data { get; set; }

        public static AlgorithmOutput FromValues(int startingWeek, IEnumerable<decimal> values)
        {
            return new AlgorithmOutput
            {
                Quality = ForecastQuality.Unknown,
                Data = new ForecastData(
                    startingWeek, 
                    values.Select(x => new ForecastValue
                        {
                            Weight = x,
                            Type = HarvestValueType.Forecasted
                        })
                    ),
                AffectingSeasons = new HashSet<Season>(),
                Comments = new List<string>()
            };
        }
    }
}