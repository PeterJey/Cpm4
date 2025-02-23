using System.Threading;
using System.Threading.Tasks;

namespace Cpm.Monitor
{
    public interface IMonitorProbe
    {
        Task<ProbeStatus> Check(CancellationToken cancellationToken);
    }
}