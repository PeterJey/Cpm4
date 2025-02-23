using System.Collections.Generic;

namespace Cpm.Core.Services.Forecast
{
    public class EvaluationResult
    {
        public IReadOnlyCollection<decimal> Factors { get; set; }
        public string Comment { get; set; }

        public static EvaluationResult None(string comment)
        {
            return new EvaluationResult
            {
                Factors = new decimal[0],
                Comment = comment
            };
        }
    }
}