using System;
using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Services.Scenarios;

namespace Cpm.Core.Services.Profiles
{
    public class SeasonsProfile
    {
        private IReadOnlyDictionary<Season, string> Scores { get; }
        public static IComparer<SeasonsProfile> DisplayComparer = new SimpleDisplayComparer();

        public static SeasonsProfile Empty = FromScores(
            Enumerable.Empty<SeasonScore>()
            );

        public static SeasonsProfile Default = FromScores(
            Enum.GetValues(typeof(Season))
                .Cast<Season>()
                .Select(x => new SeasonScore(x, SeasonScore.DefaultFor(x))));

        public static SeasonsProfile FromDictionary(IReadOnlyDictionary<Season, string> dict)
        {
            return FromScores(
                dict.Select(x => new SeasonScore(x.Key, x.Value))
                );
        }

        public static SeasonsProfile FromScores(IEnumerable<SeasonScore> scores)
        {
            return new SeasonsProfile(scores);
        }

        public static SeasonsProfile FromScores(params SeasonScore[] scores)
        {
            return new SeasonsProfile(scores);
        }

        public IReadOnlyDictionary<Season, string> ToDictionary() => Scores;

        public IReadOnlyDictionary<Season, string> ToCompactDictionary() => Scores
            .Where(x => !string.IsNullOrEmpty(x.Value))
            .OrderBy(x => x.Key)
            .ToDictionary(k => k.Key, v => v.Value);

        public bool Includes(SeasonsProfile profile)
        {
            var subMatches = Scores
                .OrderBy(x => x.Key)
                .Zip(
                    profile.Scores.OrderBy(x => x.Key), 
                    CheckSeason
                    );
            return subMatches.All(x => x);
        }

        private SeasonsProfile(IEnumerable<SeasonScore> scores)
        {
            Scores = Enum.GetValues(typeof(Season))
                .Cast<Season>()
                .ToDictionary(k => k, v => scores.FirstOrDefault(x => x.Season == v)?.Score);
        }

        private static bool CheckSeason(KeyValuePair<Season, string> me, KeyValuePair<Season, string> other)
        {
            return me.Key == other.Key && 
                   (
                       string.IsNullOrEmpty(me.Value) || 
                       string.Equals(me.Value, other.Value, StringComparison.OrdinalIgnoreCase)
                   );
        }

        public SeasonScore[] ToScores()
        {
            return Scores
                .Select(x => new SeasonScore(x.Key, x.Value))
                .ToArray();
        }

        public class SimpleDisplayComparer : IComparer<SeasonsProfile>
        {
            public int Compare(SeasonsProfile x, SeasonsProfile y)
            {
                var l = x?.ToCompactDictionary();
                var r = y?.ToCompactDictionary();

                if (l == null) return 1;
                if (r == null) return -1;

                var seasons = l.Keys
                    .Union(r.Keys)
                    .Distinct()
                    .OrderBy(key => key)
                    .ToArray();

                foreach (var season in seasons)
                {
                    if (!l.ContainsKey(season) && r.ContainsKey(season)) return 1;
                    if (l.ContainsKey(season) && !r.ContainsKey(season)) return -1;
                    if (l.ContainsKey(season) && r.ContainsKey(season))
                    {
                        var scores = SeasonScore.PossibleScoresFor(season)
                            .ToList();

                        var compareTo = scores.IndexOf(l[season]).CompareTo(scores.IndexOf(r[season]));

                        if (compareTo != 0) return compareTo;
                    };
                }

                return 0;
            }
        }
    }
}