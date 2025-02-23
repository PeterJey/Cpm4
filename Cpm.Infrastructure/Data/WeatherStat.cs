using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Cache;

namespace Cpm.Infrastructure.Data
{
    public class WeatherStat
    {
        public int Id { get; set; }
        public DateTime When { get; set; }
        [Required]
        public string Location { get; set; }
        public int SampleCount { get; set; }
        public decimal TempMin { get; set; }
        public decimal TempMax { get; set; }
        public string Log { get; set; }
    }
}