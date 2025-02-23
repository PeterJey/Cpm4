using Cpm.Core.Services.Forecast;

namespace Cpm.Core.Services
{
    public interface IUserPreferences
    {
        string AreaUnit { get; set; }
        string AreaUnitName { get; set; }
        string AreaUnitNamePlural { get; set; }
        string FormatArea(decimal area);
        string FormatYield(Yield yield);
        decimal ToAreaUnit(decimal area);
        decimal FromAreaUnit(decimal area);
        string AllocationUnitName { get; set; }
    }
}