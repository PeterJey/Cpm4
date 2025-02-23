using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cpm.Infrastructure.Apixu
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Forecast
    {
        public List<Forecastday> forecastday { get; set; }
    }
}