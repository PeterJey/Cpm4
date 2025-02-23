using System.Diagnostics.CodeAnalysis;

namespace Cpm.Infrastructure.Apixu
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Condition
    {
        public string text { get; set; }
        public string icon { get; set; }
        public int code { get; set; }
    }
}