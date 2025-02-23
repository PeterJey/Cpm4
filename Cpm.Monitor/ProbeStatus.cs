namespace Cpm.Monitor
{
    public class ProbeStatus
    {
        public bool IsOk { get; private set; }
        public string Message { get; private set; }

        public static ProbeStatus Ok = new ProbeStatus
        {
            IsOk = true
        };

        public static ProbeStatus Fail(string message) => new ProbeStatus
        {
            IsOk = false,
            Message = message
        };
    }
}