using System;
using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class NewDayVm
    {
        public ICollection<FieldAllocationVm> Fields { get; set; }
        public FieldAllocationVm Summary { get; set; }
        public DateTime? Date { get; set; }
    }
}