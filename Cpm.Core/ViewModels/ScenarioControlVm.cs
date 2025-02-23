using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class ScenarioControlVm
    {
        public string ContextId { get; set; }
        public string Name { get; set; }
        public ICollection<ScenarioControlFieldInfo> Fields { get; set; }
        public SeasonScoresVm SeasonScoresVm { get; set; }
        public string AreaUnit { get; set; }
    }
}