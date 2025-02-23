using System.ComponentModel.DataAnnotations;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Scenarios;

namespace Cpm.Core.Services.Profiles
{
    public class ProfileModel
    {
        public string Id { get; set; }
        [Required]
        public string ProfileName { get; set; }
        [Required]
        public int StartingWeek { get; set; }
        [Required]
        public decimal ExtraPotential { get; set; }
        [Required]
        public ForecastQuality Quality { get; set; }
        [Required]
        public Season Season { get; set; }
        [Required]
        public string SeasonType { get; set; }
        [Required]
        public decimal[] Weights { get; set; }
        [Required]
        public decimal[] Productivity { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
    }
}