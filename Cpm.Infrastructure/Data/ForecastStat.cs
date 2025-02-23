using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;

namespace Cpm.Infrastructure.Data
{
    public class ForecastStat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public int SampleCount { get; set; }

        public string ScenarioId { get; set; }
        public string FieldId { get; set; }
        public string AlgorithmName { get; set; }

        public int StartingWeek { get; set; }
        public string SerializedStats { get; set; }
    }
}