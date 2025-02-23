using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;

namespace Cpm.Core.ViewModels
{
    public class SeasonScoresVm
    {
        public SeasonScoresVm(SeasonsProfile seasonScores, IEnumerable<Season> seasons)
        {
            Scores = seasonScores
                .ToScores()
                .GroupJoin(
                    seasons,
                    score => score.Season,
                    season => season,
                    (score, s) => s.Any() ? ToSeasonScoreVm(score.Season, score.Score) : null)
                .Where(x => x != null)
                .ToList();
        }

        public ICollection<SeasonScoreVm> Scores { get; set; }

        private SeasonScoreVm ToSeasonScoreVm(Season season, string score)
        {
            return new SeasonScoreVm
            {
                Season = season,
                Score = score,
                ScoresList = GetScoreOptions(season),
            };
        }

        private static ICollection<OptionVm> GetScoreOptions(Season season)
        {
            return SeasonScore.PossibleScoresFor(season)
                .Select(x => new OptionVm(x, x))
                .ToList();
        }
    }
}