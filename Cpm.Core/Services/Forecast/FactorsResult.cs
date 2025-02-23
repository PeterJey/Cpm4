using System.Collections.Generic;

namespace Cpm.Core.Services.Forecast
{
    public class FactorsResult
    {
        public IReadOnlyCollection<decimal> Factors { get; set; }
        public string Comment { get; set; }
    }
}