using Cpm.Core.Services.Fields;

namespace Cpm.Core.Services.Forecast
{
    public class ForecastOutput
    {
        public FieldDetails FieldDetails { get; set; }
        public ForecastParameters Parameters { get; set; }
        public ForecastResult Result { get; set; }
    }
}