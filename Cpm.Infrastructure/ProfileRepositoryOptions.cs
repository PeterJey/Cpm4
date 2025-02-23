using System;

namespace Cpm.Infrastructure
{
    public class ProfileRepositoryOptions
    {
        public TimeSpan Expiration { get; set; }
        public bool KeepUntilEndOfDay { get; set; }
    }
}