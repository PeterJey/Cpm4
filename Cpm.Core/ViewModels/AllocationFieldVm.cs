namespace Cpm.Core.ViewModels
{
    public class AllocationFieldVm
    {
        public FieldVm FieldVm { get; set; }

        public decimal? Weight { get; set; }
        public decimal? Trays { get; set; }
        public decimal? Punnets { get; set; }
    }
}