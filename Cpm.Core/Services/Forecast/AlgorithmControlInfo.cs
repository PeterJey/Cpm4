using System.Collections.Generic;

namespace Cpm.Core.Services.Forecast
{
    public class AlgorithmControlInfo
    {
        public string AlgorithmName { get; set; }
        public string Target { get; set; }
        public string Relative { get; set; }
        public bool IsSignificantlyHigher { get; set; }
        public bool IsSignificantlyLower { get; set; }
        public ICollection<HarvestValue> Values { get; set; }
    }
}