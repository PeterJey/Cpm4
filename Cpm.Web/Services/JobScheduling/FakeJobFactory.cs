using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Cpm.Web.Services.JobScheduling
{
    public class FakeJobFactory : IJobFactory
    {
        public ILogger<FakeJobFactory> Logger { get; }

        public FakeJobFactory(ILogger<FakeJobFactory> logger)
        {
            Logger = logger;
        }

        public Task<IEnumerable<Job>> CreateJobs(CancellationToken cancellationToken)
        {
            return Task.FromResult(new[]
            {
                new Job((ct) =>
                    {
                        Logger.LogInformation("Action!");

                        return Task.FromResult(new JobResult(true));
                    }
                )
            }.AsEnumerable());
        }
    }
}