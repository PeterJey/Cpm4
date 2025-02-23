using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class GridResultSummaryHeaderVm
    {
        public string GrandTotal { get; set; }
        public ICollection<string> Subtotals { get; set; }
    }
}