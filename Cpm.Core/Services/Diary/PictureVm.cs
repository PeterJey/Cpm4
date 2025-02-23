using System;

namespace Cpm.Core.Services.Diary
{
    public class PictureVm
    {
        public string ThumbUrl { get; set; }
        public string FullUrl { get; set; }
        public string UploadedBy { get; set; }
        public DateTime? TakenOn { get; set; }
        public string Id { get; set; }
        public bool HasLocation { get; set; }
        public decimal LocationLat { get; set; }
        public decimal LocationLon { get; set; }
    }
}