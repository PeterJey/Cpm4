using Cpm.Core.Services;
using Cpm.Core.Services.Forecast;

namespace Cpm.Core.ViewModels
{
    public class YieldVm
    {
        public YieldVm(Yield yield, IUserPreferences userPreferences)
        {
            Formatted = userPreferences.FormatYield(yield);

            if (yield is YieldPerPlant yieldPerPlant)
            {
                YieldPerArea = null;
                YieldPerPlant = yieldPerPlant.GramsPerPlant.ToString("N0");
                PlantsPerArea = userPreferences.ToAreaUnit(yieldPerPlant.PlantsPerHectare).ToString("N0");
            }
            else
            {
                YieldPerArea = userPreferences.ToAreaUnit(yield.KgPerHectare).ToString("N0");
                YieldPerPlant = null;
                PlantsPerArea = null;
            }
        }

        public string PlantsPerArea { get; set; }

        public string YieldPerArea { get; set; }

        public string YieldPerPlant { get; set; }

        public string Formatted { get; set; }
    }
}