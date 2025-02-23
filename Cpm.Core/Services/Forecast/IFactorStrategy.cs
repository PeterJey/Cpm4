using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cpm.Core.Services.Profiles;

namespace Cpm.Core.Services.Forecast
{
    public interface IFactorStrategy
    {
        Task<FactorsResult> CalculateFactors(
            string location, 
            SeasonsProfile seasonsProfile,
            DateTime weekStarting, 
            IEnumerable<decimal> averageTemperatures
            );
    }
}