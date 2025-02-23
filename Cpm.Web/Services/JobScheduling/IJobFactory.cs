using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cpm.Web.Services.JobScheduling
{
    public interface IJobFactory
    {
        Task<IEnumerable<Job>> CreateJobs(CancellationToken cancellationToken);
    }
}