using System;

namespace Cpm.Monitor
{
    public class ApixuMonitorOptions
    {
        public TimeSpan HeartbeatTimeout { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan RetryTimeout { get; set; } = TimeSpan.FromSeconds(1);
        public int CheckCount { get; set; } = 10;
        public int MinOkToDegraded { get; set; } = 3;
        public int MinNokToDegraded { get; set; } = 3;
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string Identifier { get; set; } = "Default";
    }
}