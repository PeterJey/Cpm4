namespace Cpm.Core.Services.Forecast
{
    public class RelativeYieldVm
    {
        public string Relative { get; set; }

        public bool IsSignificantlyHigher { get; set; }

        public bool IsSignificantlyLower { get; set; }

        public static RelativeYieldVm Blank = new RelativeYieldVm
        {
        };
    }
}