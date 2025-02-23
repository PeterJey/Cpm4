using System.Collections.Generic;
using Cpm.Core.Services.Profiles;

namespace Cpm.Core.Services.Forecast
{
    public class ForecastParameters
    {
        public SeasonsProfile SeasonsProfile { get; }
        public string AlgorithmName { get; }
        public IReadOnlyDictionary<string, string> Settings { get; }

        public ForecastParameters(SeasonsProfile seasonsProfile, string algorithmName, IReadOnlyDictionary<string, string> settings)
        {
            SeasonsProfile = seasonsProfile;
            AlgorithmName = algorithmName;
            Settings = new Dictionary<string, string>(settings);
        }
    }
}