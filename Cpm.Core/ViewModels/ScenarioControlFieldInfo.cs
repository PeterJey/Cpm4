using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class ScenarioControlFieldInfo
    {
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public string Variety { get; set; }
        public string Area { get; set; }
        public string Algorithm { get; set; }
        public IEnumerable<OptionVm> Algorithms { get; set; }
        public string FieldId { get; set; }
        public string AffectingSeasons { get; set; }
        public string Quality { get; set; }
        public string Budget { get; set; }
        public string Target { get; set; }
        public string WeekOffset { get; set; }
        public bool IsSignificantlyHigher { get; set; }
        public bool IsSignificantlyLower { get; set; }
        public string Relative { get; set; }
        public IReadOnlyCollection<string> Comments { get; set; }
    }
}