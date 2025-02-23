using Cpm.Core.Services.Fields;

namespace Cpm.Core.Services.Forecast
{
    public class ForecastInput
    {
        public FieldDetails FieldDetails { get; set; }
        public ForecastParameters Parameters { get; set; }
    }
}