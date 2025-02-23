using System;

namespace Cpm.Web.Services.JobScheduling
{
    public class JobRunnerServiceOptions
    {
        public TimeSpan PollingDelay { get; set; }
        public TimeSpan FixedDelay { get; set; }
        public TimeSpan RandomDelay { get; set; }
        public TimeSpan RetryDelay { get; set; }
        public TimeSpan RestartDelay { get; set; }
    }
}