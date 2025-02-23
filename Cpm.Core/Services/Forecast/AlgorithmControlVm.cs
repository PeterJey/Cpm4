using System.Collections.Generic;
using Cpm.Core.ViewModels;

namespace Cpm.Core.Services.Forecast
{
    public class AlgorithmControlVm
    {
        public string ContextId { get; set; }
        public string ScenarioName { get; set; }
        public SeasonScoresVm SeasonScoresVm { get; set; }
        public ICollection<AlgorithmControlInfo> Algorithms { get; set; }
        public uint Index { get; set; }
        public string FieldName { get; set; }
        public IEnumerable<string> NamesRow { get; set; }
        public IEnumerable<string> TargetRow { get; set; }
        public IEnumerable<RelativeYieldVm> RelativeYieldRow { get; set; }
        public IEnumerable<GridResultRowVm> Rows { get; set; }
        public string SelectedAlgorithm { get; set; }
        public IEnumerable<IReadOnlyCollection<string>> Comments { get; set; }
    }
}