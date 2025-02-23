using System;

namespace Cpm.Infrastructure.Apixu
{
    public class ApixuOptions
    {
        public string ApiKey { get; set; }
        public TimeSpan WebRequestTimeout { get; set; } = TimeSpan.FromSeconds(3);
    }
}