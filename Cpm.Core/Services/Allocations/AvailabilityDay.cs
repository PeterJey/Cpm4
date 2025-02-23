using System;
using System.Collections.Generic;
using Cpm.Core.Services.Fields;

namespace Cpm.Core.Services.Allocations
{
    public class AvailabilityDay
    {
        public DateTime? Date { get; set; }
        public Dictionary<FieldDetails, decimal?> ByField { get; set; }
    }
}