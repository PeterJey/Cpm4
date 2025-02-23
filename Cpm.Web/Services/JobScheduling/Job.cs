using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cpm.Web.Services.JobScheduling
{
    public class Job : IComparable<Job>
    {
        private readonly Func<CancellationToken, Task<JobResult>> _func;

        public DateTime Due { get; set; }
        public int Attempt { get; set; }

        public Task<JobResult> Run(CancellationToken stoppingToken) => _func(stoppingToken);

        public Job(Func<CancellationToken, Task<JobResult>> func)
        {
            _func = func;
        }

        public int CompareTo(Job other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var dueComparison = Due.CompareTo(other.Due);
            if (dueComparison != 0) return dueComparison;
            return Attempt.CompareTo(other.Attempt);
        }
    }
}