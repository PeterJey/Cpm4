using System.Collections.Generic;
using Cpm.Core.Services.Scenarios;

namespace Cpm.Core.ViewModels
{
    public class SeasonScoreVm
    {
        public Season Season { get; set; }
        public string Score { get; set; }
        public ICollection<OptionVm> ScoresList { get; set; }
    }
}