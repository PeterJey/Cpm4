using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class SiteFieldsSummaryVm
    {
        public ICollection<FieldSummaryVm> Fields { get; set; }
        public string AreaUnit { get; set; }
        public string SiteId { get; set; }
        public ProfileOptionVm[] ExistingProfiles { get; set; }
    }
}