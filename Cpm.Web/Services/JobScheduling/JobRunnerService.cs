using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cpm.Web.Services.JobScheduling
{
    public class JobRunnerService : BackgroundService
    {
        public IOptions<JobRunnerServiceOptions> Options { get; }
        public IJobFactory JobFactory { get; }
        public ILogger<JobRunnerService> Logger { get; }

        private readonly Random _random = new Random();

        public JobRunnerService(
            IOptions<JobRunnerServiceOptions> options,
            IJobFactory jobFactory,
            ILogger<JobRunnerService> logger
            )
        {
            Options = options;
            JobFactory = jobFactory;
            Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => Logger.LogInformation("Stopping scheduling service..."));

            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("Scheduling service started");
                try
                {
                    var jobs = (await JobFactory.CreateJobs(stoppingToken)).ToList();

                    if (!jobs.Any()) throw new InvalidOperationException("No jobs found, terminating service");

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        jobs.Sort();

                        var job = jobs.First();

                        await WhenDue(job, stoppingToken);

                        await HandleJob(job, stoppingToken);
                    }
                }
                catch (TaskCanceledException)
                {
                    Logger.LogInformation("Stopped as requested");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Service failed due to an unhandled exception");
                }

                await Task.Delay(Options.Value.RestartDelay, stoppingToken);
            }
        }

        private async Task HandleJob(Job job, CancellationToken stoppingToken)
        {
            job.Attempt++;

            var result = await job.Run(stoppingToken);

            if (result.IsSuccessful)
            {
                job.Due = DateTime.UtcNow + 
                          Options.Value.FixedDelay + 
                          Randomize(Options.Value.RandomDelay);

                job.Attempt = 0;
            }
            else
            {
                job.Due = DateTime.UtcNow + 
                          CalculateBackoff(
                              Options.Value.RetryDelay, 
                              Options.Value.FixedDelay, 
                              job.Attempt
                              );
            }
        }

        private TimeSpan CalculateBackoff(TimeSpan retryDelay, TimeSpan maxDelay, int attempt)
        {
            // prevent wrapping bits on int type (32bit)
            var multiplier = 1 << (Math.Min(attempt, 32) - 1);

            return TimeSpan.FromTicks(
                Math.Min(
                    retryDelay.Ticks * multiplier, 
                    maxDelay.Ticks
                    )
            );
        }

        private async Task WhenDue(Job job, CancellationToken stoppingToken)
        {
            while (job.Due > DateTime.UtcNow)
            {
                await Task.Delay(Options.Value.PollingDelay, stoppingToken);
            }
        }

        private TimeSpan Randomize(TimeSpan maxDelay)
        {
            return TimeSpan.FromTicks((long) (_random.NextDouble() * maxDelay.Ticks));
        }
    }
}