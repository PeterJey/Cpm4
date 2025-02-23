using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class GridResultsVm
    {
        public GridResultTopHeaderVm Series { get; set; }
        public GridResultSummaryHeaderVm SummaryHeader { get; set; }
        public ICollection<GridResultRowVm> Rows { get; set; }
    }
}