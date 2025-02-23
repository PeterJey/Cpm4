namespace Cpm.Web.Services.JobScheduling
{
    public class JobResult
    {
        public JobResult(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }

        public bool IsSuccessful { get; }
    }
}