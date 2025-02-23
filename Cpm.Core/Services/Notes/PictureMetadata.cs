using System;

namespace Cpm.Core.Services.Notes
{
    public class PictureMetadata
    {
        public string Id { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? TakenOn { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string UploadedBy { get; set; }
        public string OriginalName { get; set; }
        public string OriginalExtension { get; set; }
    }
}