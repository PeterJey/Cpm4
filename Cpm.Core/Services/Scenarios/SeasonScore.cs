using System;
using System.Collections.Generic;
using System.Linq;

namespace Cpm.Core.Services.Scenarios
{
    public class SeasonScore
    {
        public Season Season { get; set; }
        public string Score { get; set; }

        public SeasonScore()
        {
        }

        public SeasonScore(Season season, string score)
        {
            Season = season;
            Score = score;
        }

        public static string DefaultFor(Season season)
        {
            switch (season)
            {
                case Season.Winter:
                    return DefaultWinterScore;
                case Season.Spring:
                    return DefaultSpringScore;
                case Season.Summer:
                    return DefaultSummerScore;
                case Season.Autumn:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(season), season, null);
            }
        }

        public static IEnumerable<string> PossibleScoresFor(Season season)
        {
            switch (season)
            {
                case Season.Winter:
                    return WinterScores;
                case Season.Spring:
                    return SpringScores;
                case Season.Summer:
                    return SummerScores;
                case Season.Autumn:
                    return Enumerable.Empty<string>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(season), season, null);
            }
        }

        private static readonly string[] WinterScores = { "Unfavourable", "Unfair", "Fair", "Favourable" };
        private static readonly string DefaultWinterScore = WinterScores[1];
        
        private static readonly string[] SpringScores = { "Late", "Below average", "Average", "Above average", "Good" };
        private static readonly string DefaultSpringScore = SpringScores[2];

        private static readonly string[] SummerScores = { "Unfavourable", "Unfair", "Fair", "Favourable" };
        private static readonly string DefaultSummerScore = SummerScores[1];
    }
}