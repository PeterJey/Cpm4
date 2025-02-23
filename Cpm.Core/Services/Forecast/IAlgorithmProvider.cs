using System.Collections.Generic;

namespace Cpm.Core.Services.Forecast
{
    public interface IAlgorithmProvider
    {
        IEnumerable<AlgorithmDetails> GetAvailableAlgorithms();
        AlgorithmDetails GetAlgorithmByName(string name);
    }
}