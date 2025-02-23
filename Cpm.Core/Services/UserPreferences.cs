using Cpm.Core.Services.Forecast;

namespace Cpm.Core.Services
{
    public class UserPreferences : IUserPreferences
    {
        public string AreaUnit { get; set; }

        public string AreaUnitName { get; set; }

        public decimal AreaUnitFactor { get; set; }
        
        public string AreaUnitNamePlural { get; set; }

        public string FormatArea(decimal area)
        {
            return ToAreaUnit(area).ToString("F3");
        }

        public string FormatYield(Yield yield)
        {
            return yield.ToString(AreaUnitFactor, AreaUnit);
        }

        public decimal ToAreaUnit(decimal area)
        {
            return area * AreaUnitFactor;
        }

        public decimal FromAreaUnit(decimal area)
        {
            return area / AreaUnitFactor;
        }

        public string AllocationUnitName { get; set; }
    }
}