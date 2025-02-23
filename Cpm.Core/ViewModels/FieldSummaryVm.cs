using System.Collections.Generic;

namespace Cpm.Core.ViewModels
{
    public class FieldSummaryVm
    {
        public string FieldId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Variety { get; set; }
        public string Area { get; set; }
        public string Budget { get; set; }
        public string WeightToDate { get; set; }
        public string BudgetToDate { get; set; }
        public string FirstDay { get; set; }
        public string LastDay { get; set; }
        public int NumberOfPicks { get; set; }
        public int TotalDays { get; set; }
        public ICollection<ScenarioVm> AvailableScenarioVms { get; set; }
        public string ActiveScenarioId { get; set; }
        public decimal? YieldPerArea { get; set; }
        public decimal? PlantsPerArea { get; set; }
        public decimal? YieldPerPlant { get; set; }
        public string ProfileName { get; set; }
        public string TonnesPerAcre { get; set; }
    }
}