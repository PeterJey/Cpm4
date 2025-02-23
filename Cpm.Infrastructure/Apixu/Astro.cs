using System.Diagnostics.CodeAnalysis;

namespace Cpm.Infrastructure.Apixu
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Astro
    {
        public string sunrise { get; set; }
        public string sunset { get; set; }
        public string moonrise { get; set; }
        public string moonset { get; set; }
    }
}