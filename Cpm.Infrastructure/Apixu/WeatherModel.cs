using System.Diagnostics.CodeAnalysis;

namespace Cpm.Infrastructure.Apixu
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class WeatherModel
    {
        public Location location { get; set; }
        public Current current { get; set; }
        public Forecast forecast { get; set; }
    }
}
