using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class AllocationProductVm
    {
        public ICollection<AllocationFieldVm> Fields { get; set; }
        public string Label { get; set; }
        public string Key { get; set; }
    }
}