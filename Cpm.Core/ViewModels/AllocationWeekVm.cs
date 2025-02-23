using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class AllocationWeekVm
    {
        public int Week { get; set; }
        public ICollection<NewDayVm> Days { get; set; }
        public ICollection<string> UsedProducts { get; set; }
        public NewDayVm Summary { get; set; }
        public IEnumerable<OptionVm> AllProducts { get; set; }
        public string SiteId { get; set; }
        public string AllocationUnit { get; set; }
        public ICollection<OptionVm> AvailableUnits { get; set; }
    }
}