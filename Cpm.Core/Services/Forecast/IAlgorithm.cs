using System.Threading;
using System.Threading.Tasks;

namespace Cpm.Core.Services.Forecast
{
    public interface IAlgorithm
    {
        Task<AlgorithmOutput> Calculate(AlgorithmInput input, CancellationToken cancellationToken);
    }
}